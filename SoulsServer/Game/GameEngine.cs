using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoulsServer.Engine;
using SoulsServer.Game;
using SoulsServer.Tools;
using Newtonsoft.Json;
using SoulsServer.Controller;
using Newtonsoft.Json.Linq;

namespace SoulsServer
{

    public class GameEngine
    {
        public int gameCounter = 0;
        //public static Dictionary<int, GameRoom> gameRooms = new Dictionary<int, GameRoom>();
        public static List<Card> cards { get; set; }

        public GameEngine()
        {

            SetupEngine();

            // Start Game Queue;
            pollQueue();
        }

        // Checks the queue for players, sorts them after rank and matchmake them
        public void pollQueue()
        {
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    bool matchMaked = GameQueue.GetInstance().matchPlayers(Request_CreateGame);
                    // Console.WriteLine("\t\t\t\t\t\t\t\tQueue: " + GameQueue.GetInstance().queue.Count());

                    Thread.Sleep(500);
                }
            });

            pollThread.Start();
        }

        public void SetupEngine()
        {
            GameEngine.cards = LoadCards();

        }

        public List<Card> LoadCards()
        {
            using (var db = new Model.soulsEntities())
            {
                Stopwatch w = new Stopwatch();
                w.Start();
                List<Card> cards = db.db_Card
                       .Join(
                       db.db_Ability,
                       card => card.fk_ability,
                       ability => ability.id,
                       (card, ability) => new { card, ability }
                       )
                       .Join(
                       db.db_Card_Type,
                       y => y.card.fk_type,
                       cType => cType.id,
                       (y, cType) => new { y, cType }
                      ).Select(x => new Card()
                      {
                          id = x.y.card.id,
                          name = x.y.card.name,
                          attack = x.y.card.attack,
                          health = x.y.card.health,
                          armor = x.y.card.armor,
                          cost = x.y.card.cost,
                          ability = new Ability()
                          {
                              id = x.y.ability.id,
                              name = x.y.ability.name,
                              parameter = x.y.ability.parameter
                          },
                          cardType = new CardType()
                          {
                              id = x.y.ability.id,
                              name = x.y.ability.name,
                          },
                      }).AsParallel().ToList();


                Logging.Write(Logging.Type.GAME, "Loaded " + cards.Count() + " cards, Took: " + w.ElapsedMilliseconds);
                w.Stop();

                return cards;
            }

        }

        public void Request_MoveCard(Player player)
        {


            GamePlayer requestPlayer = player.gPlayer;

            JObject retData = new JObject(
                new JProperty("cid", player.gameContext.data.Payload.cid),
                new JProperty("x", player.gameContext.data.Payload.x),
                new JProperty("y", player.gameContext.data.Payload.y)
                );



            Response response = new Response(
                GameService.GameResponseType.GAME_OPPONENT_MOVE,
                retData
                );


            requestPlayer.GetOpponent().playerContext.SendTo(response);



        }

        public void Request_OpponentReleaseCard(Player player)
        {
            GamePlayer requestPlayer = player.gPlayer;

            JObject retData = new JObject(
             new JProperty("cid", player.gameContext.data.Payload.cid));

            Response response = new Response(
                GameService.GameResponseType.GAME_RELEASE,
                retData
                );


            requestPlayer.playerContext.SendTo(response);

            response.Type = GameService.GameResponseType.GAME_OPPONENT_RELEASE;
            requestPlayer.GetOpponent().playerContext.SendTo(response);



        }

        // Starts the game from the matchmaked players in a new gameroom
        public void Request_CreateGame(Pair<Player> players)
        {

            GameRoom newRoom = new GameRoom();

            GamePlayer p1 = new GamePlayer(players.First.gameContext)
            {  // TODO missing any?
                hash = players.First.hash,
                name = players.First.name,
                mana = players.First.mana,
                attack = players.First.attack,
                health = players.First.health,
                rank = players.First.rank,
                isPlayerOne = true,
                gameRoom = newRoom,
            };

            GamePlayer p2 = new GamePlayer(players.Second.gameContext)
            { // TODO missing any?
                hash = players.Second.hash,
                name = players.Second.name,
                mana = players.Second.mana,
                attack = players.Second.attack,
                health = players.Second.health,
                rank = players.Second.rank,
                isPlayerOne = false,
                gameRoom = newRoom,
            };

            // Add GamePlayer objects to the Player contexts
            players.First.gPlayer = p1;
            players.Second.gPlayer = p2;


            Pair<GamePlayer> playerPair = new Pair<GamePlayer>(p1, p2);

            // Create a game room
            newRoom.AddGamePlayers(playerPair);


            Pair<Response> response = newRoom.GenerateGameUpdate(true);
            players.First.gameContext.SendTo(response.First);
            players.Second.gameContext.SendTo(response.Second);


            // Send "Its your turn to the start player"
            newRoom.currentPlaying.playerContext.SendDebug("Its your turn (DEBUG)");

        }

        public void Request_QueuePlayer(Player player)
        {

            // Check if the player already have a game player
            if (player.gPlayer == null)
            {

                if (!player.inQueue)
                {
                    GameQueue.GetInstance().AddPlayer(player);
                    Logging.Write(Logging.Type.GAMEQUEUE, player.name + " queued!");
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
                }
                else
                {
                    Logging.Write(Logging.Type.GAMEQUEUE, player.name + " tried to queue twice!");
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
                }

            }
            else
            {
                Console.WriteLine("[GAME] Player was already in game, giving gameUpdate (Create)");

                // Send the gamestate to the player (As create since its the first state of this override player)
                Pair<Response> response = player.gPlayer.gameRoom.GenerateGameUpdate(true);
                if (player.gPlayer.isPlayerOne)
                {
                    response.First.Type = GameService.GameResponseType.GAME_RECOVER;
                    player.gPlayer.playerContext.SendTo(response.First);
                }
                else
                {
                    response.Second.Type = GameService.GameResponseType.GAME_RECOVER;
                    player.gPlayer.playerContext.SendTo(response.Second);
                }



            }
        }


        public void Request_UseCard(Player player)
        {
            int slot = player.gameContext.data.Payload.slotId; //  This is the slot which is the cards destination
            int card = player.gameContext.data.Payload.cid; // This is the card which the player has on hand


            GamePlayer requestPlayer = player.gPlayer;

            Card c;
            // Check if the card exists or not
            if (!requestPlayer.handCards.TryGetValue(card, out c))
            {
                // Card does not exist
                // TODO
                Logging.Write(Logging.Type.GAME, "Card does not exist!");
                return;

            }
            // If opposite players turn
            else if (!requestPlayer.IsPlayerTurn())
            {
                // Send a error message, that its not players turn
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                new Dictionary<string, object> 
                        { 
                            {"card",card},
                            {"error","Not your turn!"} 
                        }));

                // Fire releaseCard (to recall the card to origin pos)
                this.Request_OpponentReleaseCard(player);
                Logging.Write(Logging.Type.GAME, "Not " + player.name + "'s turn!");
                return;
            }

            // Check if the card slot is empty
            else if (requestPlayer.boardCards.ContainsKey(slot))
            {
                // Send Release Card request (AnimateBack)
                this.Request_OpponentReleaseCard(player);

                // Send error message
                requestPlayer.playerContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_USECARD_OCCUPIED, new JObject(
                        new JProperty("slot", slot),
                        new JProperty("message", "Slot is already occupied!")
                )));

                Logging.Write(Logging.Type.GAME, "Slot occupied!");
                return;
            }

            // Not enough mana to use card
            else if (!requestPlayer.HasEnoughMana(c))
            {
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_USECARD_OOM, "Not enough mana!"));
                Logging.Write(Logging.Type.GAME, "Not enough mana!");
                return;
            }


            else
            {
                // Everything is OK, move the card to the board
                requestPlayer.handCards.Remove(card);
                requestPlayer.boardCards.Add(slot, c);

                // Set the slotId on the card.
                c.slotId = slot;

                //Next turn
                requestPlayer.gameRoom.NextTurn();


                // Send Reply
                Response response = new Response(GameService.GameResponseType.GAME_USECARD_OK,
                    new JObject(new JProperty("card", JObject.FromObject(c))));

                requestPlayer.playerContext.SendTo(response);
                response.Type = GameService.GameResponseType.GAME_OPPONENT_USECARD_OK;
                requestPlayer.GetOpponent().playerContext.SendTo(response);
                Logging.Write(Logging.Type.GAME, player.name + "Used a card!");
            }
        }

        public void Request_NextTurn(Player player)
        {
            GamePlayer requestPlayer = player.gPlayer;

            // Validate player turn
            if (!requestPlayer.IsPlayerTurn()) return;

            // Run next round
            requestPlayer.gameRoom.NextTurn();


            // Send response
            requestPlayer.playerContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(new JProperty("yourTurn", false)
                    )));

            requestPlayer.GetOpponent().playerContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(new JProperty("yourTurn", true)
                    )));

        }

        public void Request_Attack(Player player)
        {
            // Get data
            int source = player.gameContext.payload.source;
            int target = player.gameContext.payload.target;
            int type = player.gameContext.payload.type;

            // Fetch GamePlayers
            GamePlayer requestPlayer = player.gPlayer;
            GamePlayer opponent = requestPlayer.GetOpponent();

            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            // Check criterias

            // Check that none of the data is NULL or default values!
            if (source != null && target != null && (type != null || type == -1))
            {
                // TODO missing data!
            }

            // Players turn?
            if (!requestPlayer.IsPlayerTurn())
            {
                // TODO send error NOT TURN
            }

            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////

            if (type == 0) // Card on Card
            {
                // Fetch cards from the CID's
                Card sourceCard = requestPlayer.boardCards.FirstOrDefault(x => x.Value.cid == source).Value;
                Card targetCard = opponent.boardCards.FirstOrDefault(x => x.Value.cid == target).Value;

                sourceCard.Attack(targetCard);

                // Requester's Card
                JObject reqObj = new JObject(
                            new JProperty("cid", sourceCard.cid),
                            new JProperty("dmgTaken", targetCard.attack),
                            new JProperty("dmgDone", sourceCard.attack),
                            new JProperty("attacker", true),
                            new JProperty("isDead", sourceCard.isDead));

                // Requesters Opponent's Card
                JObject oppObj = new JObject(
                            new JProperty("cid", targetCard.cid),
                            new JProperty("dmgTaken", sourceCard.attack),
                            new JProperty("dmgDone", targetCard.attack),
                            new JProperty("attacker", false),
                            new JProperty("isDead", targetCard.isDead));

                // Send Response to Requester
                requestPlayer.playerContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", reqObj),
                        new JProperty("opponent", oppObj),
                        new JProperty("type", type)
                        )));

                // Send Response to Opponent
                opponent.playerContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", oppObj),
                        new JProperty("opponent", reqObj),
                        new JProperty("type", type)
                        )));

            }
            else if (type == 1) // Card on Hero
            {
                Card sourceCard = requestPlayer.boardCards[source];
                sourceCard.Attack(opponent);



            }
            else if (type == 2) // Hero on Card
            {
                Card targetCard = opponent.boardCards[target];


            }

        }

    }

}

