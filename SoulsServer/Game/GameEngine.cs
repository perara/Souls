using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Souls.Server.Engine;
using Souls.Server.Game;
using Souls.Server.Tools;
using Newtonsoft.Json;
using Souls.Server.Objects;
using Newtonsoft.Json.Linq;
using NHibernate.Linq;
using Souls.Server.Network;
using SoulsModel;

namespace Souls.Server.Game
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
                    Console.WriteLine("\t\t\t\t\t\t\t\tQueue: " + GameQueue.GetInstance().queue.Count());

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

            Stopwatch w = new Stopwatch();
            w.Start();

            NHibernateHelper.OpenSession();
            using (var session = NHibernateHelper.OpenSession())
            {

                // Fetch the Card (ModelObject)
                var dbCards = session.Query<Souls.Model.Card>()
                    .Fetch(x => x.ability)
                    .Fetch(x => x.race)
                    .ToList<Souls.Model.Card>();

                // Create cards from the Database Model
                List<Card> cards = new List<Card>();
                dbCards.ForEach(x => cards.Add(new Card()
                {
                    id = x.id,
                    ability = x.ability,
                    race = x.race,
                    name = x.name,
                    attack = x.attack,
                    health = x.health,
                    armor = x.armor,
                    cost = x.cost
                }));

                Logging.Write(Logging.Type.GAME, "Loaded " + cards.Count() + " cards, Took: " + w.ElapsedMilliseconds);
                w.Stop();

                return cards;
            }
        }

        public void Request_MoveCard(Player player)
        {

            // Check that the game is actually ONGOING
            if (!this.GameRoomRunning(player)) return;

            JObject retData = new JObject(
                new JProperty("cid", player.gameContext.data.Payload.cid),
                new JProperty("x", player.gameContext.data.Payload.x),
                new JProperty("y", player.gameContext.data.Payload.y)
                );



            Response response = new Response(
                GameService.GameResponseType.GAME_OPPONENT_MOVE,
                retData
                );


            player.gPlayer.GetOpponent().gameContext.SendTo(response);



        }

        public void Request_OpponentReleaseCard(Player player)
        {
            // Check that the game is actually ONGOING
            if (!this.GameRoomRunning(player)) return;

            JObject retData = new JObject(
             new JProperty("cid", player.gameContext.data.Payload.cid));

            Response response = new Response(
                GameService.GameResponseType.GAME_RELEASE,
                retData
                );


            player.gameContext.SendTo(response);

            response.Type = GameService.GameResponseType.GAME_OPPONENT_RELEASE;
            player.gPlayer.GetOpponent().gameContext.SendTo(response);



        }

        // Starts the game from the matchmaked players in a new gameroom
        public void Request_CreateGame(Pair<Player> players)
        {

            players.First.ConstructGamePlayer(true);
            players.Second.ConstructGamePlayer(false);

            // Create the GameRoom
            GameRoom newRoom = new GameRoom();
            newRoom.AddGamePlayers(players);

            players.First.gPlayer.gameRoom = newRoom;
            players.Second.gPlayer.gameRoom = newRoom;

            // Generate and send a response
            Pair<Response> response = newRoom.GenerateGameUpdate(true);
            players.First.gameContext.SendTo(response.First);
            players.Second.gameContext.SendTo(response.Second);

            // Send "Its your turn to the start player"
            newRoom.currentPlaying.gameContext.SendDebug("Its your turn (DEBUG)");

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

                // Check that the game is actually ONGOING
                if (!this.GameRoomRunning(player)) return;

                // Send the gamestate to the player (As create since its the first state of this override player)
                Pair<Response> response = player.gPlayer.gameRoom.GenerateGameUpdate();
                if (player.gPlayer.isPlayerOne)
                {
                    player.gPlayer.playerContext.SendTo(response.First);
                }
                else
                {
                    player.gPlayer.playerContext.SendTo(response.Second);
                }



            }
        }


        public void Request_UseCard(Player player)
        {
            int slot = player.gameContext.data.Payload.slotId; //  This is the slot which is the cards destination
            int card = player.gameContext.data.Payload.cid; // This is the card which the player has on hand


            GamePlayer requestPlayer = player.gPlayer;

            // Check that the game is actually ONGOING
            if (!this.GameRoomRunning(player)) return;

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
               new JObject(
                   new JProperty("card", card),
                   new JProperty("error", "Not your turn!")
                   )));




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

                // Fire releaseCard (to recall the card to origin pos)
                this.Request_OpponentReleaseCard(player);
                return;
            }


            else
            {
                // Everything is OK, move the card to the board
                requestPlayer.handCards.Remove(card);
                requestPlayer.boardCards.Add(slot, c);


                // Subtract mana from player
                requestPlayer.mana -= c.cost;

                // Set the slotId on the card.
                c.slotId = slot;

                //Next turn
                //requestPlayer.gameRoom.NextTurn();


                // Send Reply
                Response response = new Response(GameService.GameResponseType.GAME_USECARD_OK,
                    new JObject(
                        new JProperty("card", JObject.FromObject(c)),
                        new JProperty("pInfo", JObject.FromObject(requestPlayer.GetPlayerData()))
                    ));

                requestPlayer.playerContext.SendTo(response);
                response.Type = GameService.GameResponseType.GAME_OPPONENT_USECARD_OK;
                player.gPlayer.GetOpponent().gameContext.SendTo(response);
                Logging.Write(Logging.Type.GAME, player.name + "Used a card!");
            }
        }

        public void Request_NextTurn(Player player)
        {
            Logging.Write(Logging.Type.GAME, player.name + " initiated next turn");

            GamePlayer requestPlayer = player.gPlayer;

            // Check that the game is actually ONGOING
            if (!this.GameRoomRunning(player)) return;

            // Validate player turn
            if (!requestPlayer.IsPlayerTurn()) return;

            // Run next round
            requestPlayer.gameRoom.NextTurn();

            // Give a new card to the Opponent
            this.Request_NewCard(player.gPlayer.GetOpponent());

            // Send response
            requestPlayer.playerContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(
                    new JProperty("yourTurn", false),
                    new JProperty("playerInfo", JObject.FromObject(requestPlayer.GetPlayerData())),
                    new JProperty("opponentInfo", JObject.FromObject(player.gPlayer.GetOpponent().gPlayer.GetPlayerData()))
                    )));

            player.gPlayer.GetOpponent().gameContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(
                    new JProperty("yourTurn", true),
                    new JProperty("playerInfo", JObject.FromObject(player.gPlayer.GetOpponent().gPlayer.GetPlayerData())),
                    new JProperty("opponentInfo", JObject.FromObject(requestPlayer.GetPlayerData()))
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
            GamePlayer opponent = player.gPlayer.GetOpponent().gPlayer;

            // Check that the game is actually ONGOING
            if (!this.GameRoomRunning(player)) return;


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
                // Send a error message, that its not players turn
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
               new JObject(
                   new JProperty("error", "Not your turn!")
                   )));

                return;
            }

            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////

            if (type == 0) // Card on Card
            {
                // Fetch cards from the CID's
                Card sourceCard = requestPlayer.boardCards.FirstOrDefault(x => x.Value.cid == source).Value;
                Card targetCard = opponent.boardCards.FirstOrDefault(x => x.Value.cid == target).Value;

                // Ignore if one of the card did not exist
                if (sourceCard == null || targetCard == null) return;


                sourceCard.Attack(targetCard);

                // Check if attackers card is dead
                if (sourceCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + sourceCard.cid + " died.");
                    // Remove the card
                    requestPlayer.RemoveBoardCard(sourceCard);
                }

                // Check if defenders card is dead
                if (targetCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + targetCard.cid + " died.");
                    // Remove the card
                    opponent.RemoveBoardCard(targetCard);
                }

                // Requester's Card
                JObject reqObj = new JObject(
                            new JProperty("cid", sourceCard.cid),
                            new JProperty("dmgTaken", targetCard.attack),
                            new JProperty("dmgDone", sourceCard.attack),
                            new JProperty("health", sourceCard.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", sourceCard.isDead));

                // Requesters Opponent's Card
                JObject oppObj = new JObject(
                            new JProperty("cid", targetCard.cid),
                            new JProperty("dmgTaken", sourceCard.attack),
                            new JProperty("dmgDone", targetCard.attack),
                            new JProperty("health", targetCard.health),
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
                // Do attack
                Card sourceCard = requestPlayer.boardCards.FirstOrDefault(x => x.Value.cid == source).Value;
                sourceCard.Attack(opponent);

                if (opponent.isDead)
                {
                    // Requester won
                    requestPlayer.gameRoom.winner = player;
                    requestPlayer.gameRoom.isEnded = true;

                    // Check that the game is actually ONGOING
                    this.GameRoomRunning(player);
                    this.GameRoomRunning(player.gPlayer.GetOpponent());
                                    
                }

                if (sourceCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + sourceCard.cid + " died.");
                    // Remove the card
                    requestPlayer.RemoveBoardCard(sourceCard);
                }

                // Requester's Card
                JObject reqObj = new JObject(
                            new JProperty("cid", sourceCard.cid),
                            new JProperty("dmgTaken", opponent.attack),
                            new JProperty("dmgDone", sourceCard.attack),
                            new JProperty("health", sourceCard.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", sourceCard.isDead));

                // Requesters Opponent's Card
                JObject oppObj = new JObject(
                            new JProperty("dmgTaken", sourceCard.attack),
                            new JProperty("dmgDone", opponent.attack),
                            new JProperty("health", opponent.health),
                            new JProperty("attacker", false),
                            new JProperty("isDead", opponent.isDead));

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
            else if (type == 2) // Hero on Card
            {
                Card targetCard = opponent.boardCards[target];


            }

        }

        /// <summary>
        ///  Sends a new card to the specified GamePlayer
        /// </summary>
        /// <param name="player">The game player</param>
        public void Request_NewCard(Player player, int num = 1)
        {
            // Do not allow more than 10 Cards
            if (player.gPlayer.handCards.Count() >= 10) return;

            // Create the new card
            List<Card> newCard = player.gPlayer.AddCard(num);

            player.gPlayer.AddCardToHand(newCard);

            // Create a response with the new card
            Response ret = new Response(GameService.GameResponseType.GAME_NEWCARD, new JObject(
                new JProperty("card", JArray.FromObject(newCard))
                ));

            // Send to the player
            player.gameContext.SendTo(ret);


            // Create a response with the CID to the opponent
            Response retOpponent = new Response(GameService.GameResponseType.GAME_OPPONENT_NEWCARD, new JObject(
              new JProperty("card", from h in newCard
                                    select
                                        new JObject(
                                             new JObject(
                                              new JProperty("cid", h.cid))
                                 ))
               ));

            // Send to the player

            player.gPlayer.GetOpponent().gameContext.SendTo(retOpponent);
        }


        /// <summary>
        /// Determines weither the game is running or not. If its not running, a Message will be sent to the requester with Victory or Defeat
        /// </summary>
        /// <param name="gPlayer"> The gme player</param>
        /// <returns></returns>
        public bool GameRoomRunning(Player player)
        {
            if (player.gPlayer.gameRoom.isEnded)
            {
                player.gPlayer.playerContext.SendTo(
                    new Response((player.gPlayer.gameRoom.winner == player) ?
                            GameService.GameResponseType.GAME_VICTORY :
                            GameService.GameResponseType.GAME_DEFEAT,
                            new JObject(
                                new JProperty("statistics", null)
                            ))
                     );

                return false;
            }
            return true;


        }

    }

}

