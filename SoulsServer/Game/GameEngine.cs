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
        public static Dictionary<int, GameRoom> gameRooms = new Dictionary<int, GameRoom>();
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
                    bool matchMaked = GameQueue.GetInstance().matchPlayers(initGame);
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


                Console.WriteLine(">[GAME] Loaded " + cards.Count() + " cards, Took: " + w.ElapsedMilliseconds);
                w.Stop();

                return cards;
            }

        }

        public void MovedCard(Player player)
        {

            JObject retData = new JObject(
                new JProperty("cid", player.context.data.Payload.cid),
                new JProperty("x", player.context.data.Payload.x),
                new JProperty("y", player.context.data.Payload.y)
                );

           

            Response response = new Response(
                GameService.GameResponseType.GAME_OPPONENT_MOVE,
                retData
                );

            // Authenticate the player
            GamePlayer requestPlayer = null;
            if ((requestPlayer = AuthenticatePlayer(player)) != null)
            {
                requestPlayer.GetOpponent().playerContext.SendTo(response);
            }


        }


        // Starts the game from the matchmaked players in a new gameroom
        public void initGame(Pair<Player> players)
        {


            GameRoom newRoom = new GameRoom();

            GamePlayer p1 = new GamePlayer(players.First.context)
            {
                hash = players.First.hash,
                name = players.First.name,
                rank = players.First.rank,
                playernum = 1,
                gameRoom = newRoom,
            };

            GamePlayer p2 = new GamePlayer(players.Second.context)
            {
                hash = players.Second.hash,
                name = players.Second.name,
                rank = players.Second.rank,
                playernum = 2,
                gameRoom = newRoom,
            };

            // Create a game room
            newRoom.AddGamePlayer(new Pair<GamePlayer>(p1, p2));
            gameRooms.Add(newRoom.gameId, newRoom);

            // Send a full game update
            Pair<Response> response = GenerateGameUpdate(newRoom, true);

            players.First.context.SendTo(response.First);
            players.Second.context.SendTo(response.Second);


            // Send "Its your turn to the start player"
            newRoom.currentPlaying.playerContext.SendDebug("Its your turn (DEBUG)");

        }

        public void QueuePlayer(Player player)
        {
            if (!player.inQueue)
            {
                GameQueue.GetInstance().AddPlayer(player);
                Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " queued!");
                player.context.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
            }
            else
            {
                Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " tried to queue twice!");
                player.context.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
            }
        }


        public void UseCardRequest(Player player)
        {
            int slot = player.context.data.Payload.slotId; //  This is the slot which is the cards destination
            int card = player.context.data.Payload.cid; // This is the card which the player has on hand


            // Authenticate the player
            GamePlayer requestPlayer = null;
            if ((requestPlayer = AuthenticatePlayer(player)) != null)
            {

                if (!IsPlayerTurn(requestPlayer))
                {
                    requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                    new Dictionary<string, object> 
                        { 
                            {"card",card},
                            {"error","Not your turn!"} 
                        }));
                    Console.WriteLine("Not your turn!");
                    return;
                }


                if (!requestPlayer.HasEnoughMana(card))
                {
                    requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_USECARD_OOM, "Not enough mana!"));
                    Console.WriteLine("Not enough mana!");
                    return;
                }
                else
                {



                    // Move a card to the board
                    Card c;
                    requestPlayer.handCards.TryGetValue(card, out c);
                    requestPlayer.handCards.Remove(card);
                    requestPlayer.boardCards.Add(c.cid, c);


                    //Next turn
                    requestPlayer.gameRoom.NextTurn();

                    // Send Reply
                    requestPlayer.playerContext.SendTo(new Response(
                        GameService.GameResponseType.GAME_USECARD_OK, new Dictionary<string, int> 
                        { 
                            {"card",card},
                            {"slot",slot} 
                        }));
                    Console.WriteLine("Sent!");



                }




            }

        }

        public bool IsPlayerTurn(GamePlayer player)
        {

            if (!player.Equals(player.gameRoom.currentPlaying))
            {
                return false;
            }
            return true;
        }

        public void NextRoundRequest(Player context)
        {
            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = AuthenticatePlayer(context)) != null)
            {
                // Validate player turn
                if (!IsPlayerTurn(authPlayer)) return;

                // Run next round
                authPlayer.gameRoom.NextRound();

                // Send back game state (update)

                Pair<Response> response = GenerateGameUpdate(authPlayer.gameRoom);
                authPlayer.GetOpponent().playerContext.SendTo(response.First);
                authPlayer.playerContext.SendTo(response.Second);
            }
        }

        public void NextRoundResponse()
        {

        }

        public void RequestCardAttack(Player player)
        {
            int attacker = player.context.data.attacker;
            int defender = player.context.data.defender;

            bool cardAttackPlayer = player.context.data.cardAttackPlayer; //NB MIGHT BE NULL IF NOT USED
            bool playerAttackCard = player.context.data.playerAttackCard; //NB MIGHT BE NULL IF NOT USED

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = AuthenticatePlayer(player)) != null)
            {
                if (!IsPlayerTurn(authPlayer))
                {
                    return;
                }


                GamePlayer opponent = authPlayer.GetOpponent();
                if (opponent != null)
                {
                    // Attack Card and shaise, must authenticate the Move
                    Card atkCard = authPlayer.handCards[attacker];
                    Card defCard = opponent.handCards[defender];


                    if (cardAttackPlayer)
                    {
                        atkCard.Attack(ref opponent); // Card on Player attack
                    }
                    else if (playerAttackCard)
                    {
                        authPlayer.Attack(defCard); // Player on Card attack
                    }
                    else
                    {
                        atkCard.Attack(ref defCard); // Card on Card
                    }

                    // Send back game update
                    Pair<Response> response = GenerateGameUpdate(authPlayer.gameRoom);
                    opponent.playerContext.SendTo(response.First);
                    authPlayer.playerContext.SendTo(response.Second);

                }
                else
                {
                    player.context.SendTo(new Response(GameService.GameResponseType.GAME_OPPONENT_NOEXIST, "Opponent does not exist!"));
                }
            }
        }

        /// <summary>
        /// This object sends current game state to the game players, This happens every turn.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public Pair<Response> GenerateGameUpdate(GameRoom room, bool create = false)
        {

            // Game Data player 1
            GameData gData = new GameData(room);


            var p1Data = gData.Get(true);
            var p2Data = gData.Get(false);


            GameService.GameResponseType responseType = GameService.GameResponseType.GAME_UPDATE;
            if (create)
            {
                responseType = GameService.GameResponseType.GAME_CREATE;
            }

            Pair<Response> gUpdates = new Pair<Response>(
                new Response(responseType, p1Data),
                new Response(responseType, p2Data)
                );

            return gUpdates;
        }

        public GamePlayer AuthenticatePlayer(Player player)
        {
            int gameId = player.context.payload.gameId;

            GamePlayer gPlayer = null;
            if (gameRooms[gameId].players.First.hash == player.hash)
            {
                gPlayer = gameRooms[gameId].players.First;
            }
            else if (gameRooms[gameId].players.Second.hash == player.hash)
            {
                gPlayer = gameRooms[gameId].players.Second;

            }

            if (gPlayer != null)
            {
                return gPlayer;
            }
            else
            {
                player.context.SendTo(new Response(GameService.GameResponseType.GAME_AUTH_FAIL, "Authentication failed!"));
                return null;
            }
        }
    }
}
