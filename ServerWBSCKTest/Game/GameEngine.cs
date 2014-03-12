using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Game;
using ServerWBSCKTest.Tools;
using Newtonsoft.Json;

namespace ServerWBSCKTest
{

    public class GameEngine
    {
        public int gameCounter = 0;
        public static Dictionary<int, GameRoom> gameRooms = new Dictionary<int, GameRoom>();

        public GameEngine()
        {
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
                    Console.WriteLine("\t\t\t\t\t\t\t\tQueue: " + GameQueue.GetInstance().queue.Count());

                    Thread.Sleep(5000);
                }
            });

            pollThread.Start();
        }

        // Starts the game from the matchmaked players in a new gameroom
        public void initGame(Pair<Player> players)
        {

            gameCounter++;

            GamePlayer p1 = new GamePlayer(players.First.playerContext)
            {
                hash = players.First.hash,
                name = players.First.name,
                gameId = gameCounter,
                rank = players.First.rank,
                playernum = 1,
            };

            GamePlayer p2 = new GamePlayer(players.Second.playerContext)
            {
                hash = players.Second.hash,
                name = players.Second.name,
                gameId = gameCounter,
                rank = players.Second.rank,
                playernum = 2,
            };

            // Create a game room
            GameRoom newRoom = new GameRoom(new Pair<GamePlayer>(p1, p2), gameCounter);
            gameRooms.Add(gameCounter, newRoom);

            // Send a full game update
            Pair<Response> response = GenerateGameUpdate(newRoom, true);
            players.First.playerContext.SendTo(response.First);
            players.Second.playerContext.SendTo(response.Second);

            // Send "Its your turn to the start player"
            newRoom.currentPlaying.playerContext.SendDebug("Its your turn (DEBUG)");

        }

        public void QueuePlayer(Player player)
        {
            if (!player.inQueue)
            {
                GameQueue.GetInstance().AddPlayer(player);
                Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " queued!");
                player.playerContext.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
            }
            else
            {
                Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " tried to queue twice!");
                player.playerContext.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
            }
        }


        public void RequestUseCard(Player player)
        {
            int slot = player.playerContext.data.Payload.slotId; //  This is the slot which is the cards destination
            int card = player.playerContext.data.Payload.cardId; // This is the card which the player has on hand

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = AuthenticatePlayer(player)) != null)
            {

                if (!IsPlayerTurn(authPlayer))
                {
                    return;
                }


                // Evaluate Mana cost
                if (authPlayer.mana >= authPlayer.handCards[card].cost)
                {
                    // Move a card to the board
                    authPlayer.boardCards.TryAdd(authPlayer.handCards.RemoveAndReturn(card));
                    authPlayer.playerContext.SendTo(
                        new Response(GameService.GameResponseType.GAME_USECARD_OK,
                            GenerateGameUpdate(gameRooms[authPlayer.gameId]))
                            );
                }
                else
                {
                    authPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_USECARD_OOM, "Not enough mana!"));
                    return;
                }
            }

        }

        public bool IsPlayerTurn(GamePlayer player)
        {

            if (!player.Equals(gameRooms[player.gameId].currentPlaying))
            {
                player.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN, "Its not your turn!")); //TODO better format
                return false;
            }
            return true;
        }

        public void RequestNextRound(Player context)
        {

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = AuthenticatePlayer(context)) != null)
            {
                // Validate player turn
                if (!IsPlayerTurn(authPlayer)) return;

                // Run next round
                gameRooms[authPlayer.gameId].NextRound();

                // Send back game state (update)

                Pair<Response> response = GenerateGameUpdate(gameRooms[authPlayer.gameId]);
                gameRooms[authPlayer.gameId].GetOpponent(authPlayer).playerContext.SendTo(response.First);
                authPlayer.playerContext.SendTo(response.Second);
            }
        }

        public void RequestCardAttack(Player player)
        {
            int attacker = player.playerContext.data.attacker;
            int defender = player.playerContext.data.defender;

            bool cardAttackPlayer = player.playerContext.data.cardAttackPlayer; //NB MIGHT BE NULL IF NOT USED
            bool playerAttackCard = player.playerContext.data.playerAttackCard; //NB MIGHT BE NULL IF NOT USED

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = AuthenticatePlayer(player)) != null)
            {
                if (!IsPlayerTurn(authPlayer))
                {
                    return;
                }


                GamePlayer opponent = gameRooms[authPlayer.gameId].GetOpponent(authPlayer);
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
                    Pair<Response> response = GenerateGameUpdate(gameRooms[authPlayer.gameId]);
                    opponent.playerContext.SendTo(response.First);
                    authPlayer.playerContext.SendTo(response.Second);

                }
                else
                {
                    player.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_OPPONENT_NOEXIST, "Opponent does not exist!"));
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


            var p1Data = gData.Get(new string[] { "p1_hand", "p1_board", "p2_hand_count", "p2_board", "gameId", "round", "p1", "p2","p1_ident"}, true);
            var p2Data = gData.Get(new string[] { "p2_hand", "p2_board", "p1_hand_count", "p1_board", "gameId", "round", "p1", "p2", "p2_ident"}, false);


            GameService.GameResponseType responseType = GameService.GameResponseType.GAME_UPDATE;
            if(create)
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
            int gameId = player.playerContext.payload.gameId;

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
                player.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_AUTH_FAIL, "Authentication failed!"));
                return null;
            }
        }
    }
}
