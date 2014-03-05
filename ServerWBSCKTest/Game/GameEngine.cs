using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerWBSCKTest.Game;
using Alchemy.Classes;
using ServerWBSCKTest.Tools;
using ServerWBSCKTest.Engine;

namespace ServerWBSCKTest
{

    class GameEngine
    {
        public int gameCounter = 0;
        public static Dictionary<int, GameRoom> games = new Dictionary<int, GameRoom>();
        public GameQueue gameQueue;

        public Action<Pair<GamePlayer>, Server.Response> cbkSendGame = null; //cbk Callback
        public Action<int, Server.Response> cbkSendPlayer = null; //cbk Callback
        public Action<GamePlayer, string> cbkSendError = null;

        public void addCallbacks(Action<Pair<GamePlayer>, Server.Response> cbkSendGame)
        {
            this.cbkSendGame = cbkSendGame;
        }
        public void addCallbacks(Action<int, Server.Response> cbkSendPlayer)
        {
            this.cbkSendPlayer = cbkSendPlayer;
        }

        public void addErrorCallback(Action<GamePlayer,string> cbkSendError)
        {
            this.cbkSendError = cbkSendError;
        }

        public GameEngine()
        {
            // Initialize game queue
            this.gameQueue = new GameQueue();
        }

        // Checks the queue for players, sorts them after rank and matchmake them
        public void pollQueue()
        {
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    bool matchMaked = gameQueue.matchPlayers(initGame);
                    Console.WriteLine("\t\t\t\t\t\t\tCurrent in queue: " + gameQueue.queue.Count());
                    Thread.Sleep(5000);
                }
            });

            pollThread.Start();
        }

        // Starts the game from the matchmaked players in a new gameroom
        public void initGame(Pair<Player> players)
        {
            GamePlayer p1 = JSONHelper.SeDerialize<Player, GamePlayer>(players.First);
            GamePlayer p2 = JSONHelper.SeDerialize<Player, GamePlayer>(players.Second);
            Pair<GamePlayer> gPlayers = new Pair<GamePlayer>(p1, p2);

            // Create a game
            games.Add(gameCounter, new GameRoom(gPlayers, gameCounter));

            Server.Response response = generateGameUpdate(gameCounter);
            cbkSendGame(gPlayers, response);
            gameCounter++;

        }
   
        public void QueuePlayer(Player player)
        {
            gameQueue.addPlayer(player);
            cbkSendPlayer(player.id, new Server.Response(Server.ResponseType.QUEUED_OK, "Queued player!"));
        }


        public void RequestUseCard(dynamic clientData)
        {
            int gameId = clientData.gameId;
            string hash = clientData.hash;
            int slot = clientData.slot; //  This is the slot which is the cards destination
            int card = clientData.card; // This is the card which the player has on hand

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = authenticateHash(gameId, hash)) != null)
            {
                if (!checkTurn(gameId, authPlayer)) return;




                // Check if TURN player can use selected card (mana cost)
                if (authPlayer.mana >= authPlayer.handCards[card].cost)
                {
                    // Move a card to the board
                    authPlayer.boardCards.TryAdd(authPlayer.handCards.RemoveAndReturn(card));
                }
                else
                {
                    this.cbkSendPlayer(authPlayer.id, new Server.Response(Server.ResponseType.GAME_NOT_ENOUGH_MANA, "Not enough mana!"));
                    return;
                }

                // Generate a server response with new GAME INFO
                this.cbkSendPlayer(authPlayer.id, generateGameUpdate(gameId));

            }

        }

        public bool checkTurn(int gameId, GamePlayer player)
        {
            // Check that the authenticated player is the current playing Player
            if (!player.Equals(games[gameId].currentPlaying))
            {
                cbkSendError(player, "Its not your turn, cheaters deserve to die btw :>");

                return false;
            }
            return true;
        }

        public void RequestNextRound(dynamic clientData)
        {
            int gameId = clientData.gameId;
            string hash = clientData.hash;

             // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = authenticateHash(gameId, hash)) != null)
            {
                // Validate player turn
                if (!checkTurn(gameId, authPlayer)) return;

                // Run next round
                games[gameId].nextRound();

                // Send back game state (update)
                this.cbkSendGame(new Pair<GamePlayer>(authPlayer, games[gameId].getOpponent(authPlayer)), generateGameUpdate(gameId));
            }
            else
            {
                cbkSendError(authPlayer, "Error!, Authentication failed (HASH) (RequestCardAttack)");
            }
        }

        public void RequestCardAttack(dynamic clientData)
        {
            int gameId = clientData.gameId;
            string hash = clientData.hash;
            int attacker = clientData.attacker;
            int defender = clientData.defender;

            bool cardAttackPlayer = clientData.cardAttackPlayer; //NB MIGHT BE NULL IF NOT USED
            bool playerAttackCard = clientData.playerAttackCard; //NB MIGHT BE NULL IF NOT USED

            // Authenticate the player
            GamePlayer authPlayer = null;
            if ((authPlayer = authenticateHash(gameId, hash)) != null )
            {
                if (!checkTurn(gameId, authPlayer)) return;
              

                GamePlayer opponent = games[gameId].getOpponent(authPlayer);
                if (opponent != null)
                {


                    // Attack Card and shaise, must authenticate the Move
                    Card atkCard = authPlayer.handCards[attacker];
                    Card defCard = opponent.handCards[defender];

                    // Check if the defender is a card or a player (20 = Player, everytghing below 'might' be a card)
                    if (cardAttackPlayer) atkCard.Attack(ref opponent); // Card on Player attack
                    else if (playerAttackCard) authPlayer.Attack(defCard); // Player on Card attack
                    else atkCard.Attack(ref defCard); // Card on Card

                    this.cbkSendGame(new Pair<GamePlayer>(authPlayer, opponent), generateGameUpdate(gameId));
                }
                else
                {
                    cbkSendError(authPlayer, "Error!, Opponent did NOT EXIST  (RequestCardAttack)");
                }
            }
            else
            {
                cbkSendError(authPlayer, "Error!, Authentication failed (HASH) (RequestCardAttack)");
            }
        }



        public Server.Response generateGameUpdate(int gameId)
        {
            // Send entire game
            GameData gd = new GameData(games[gameId]);
            
            return new Server.Response(Server.ResponseType.GAME_UPDATE, gd);
        }

        public GamePlayer authenticateHash(int gameId, string hash)
        {
            GamePlayer player = null;
            if (games[gameId].players.First.Equals(hash))
            {
                player = games[gameId].players.First;
            }
            else if (games[gameId].players.Second.Equals(hash))
            {
                player = games[gameId].players.Second;

            }

            if (player != null)
            {
                return player;
            }
            else
            {
                // Player does not exists in this game. (with taht hash nanyyotkwe)
                return null;
            }
        }
    }
}
