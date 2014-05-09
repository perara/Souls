using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Souls.Server.Engine;
using Souls.Server.Tools;
using Newtonsoft.Json.Linq;
using Souls.Server.Objects;
using Souls.Server.Game;
using Souls.Server.Network;
using Souls.Model.Helpers;
using System.Diagnostics;
using Souls.Server.Chat;


namespace Souls.Server.Game
{
    public class GameRoom
    {
        public static int gameCounter { get; set; }

        public Pair<Player> players;
        public int gameId { get; set; }
        public int turn { get; set; }
        public int round { get; set; }

        Random rand = new Random((int)DateTime.Now.Millisecond);

        /// <summary>
        /// Count number or cards in the room. is used as identifier on cards with gameId (gameId * cardCount)
        /// </summary>
        public int cardCount { get; set; }
        // Contains the Player which is currently playing (Player's turn);
        public Player currentPlaying { get; set; }
        public bool isEnded { get; set; }
        public Player winner { get; set; }

        public GameLogger logger { get; set; }
        public Stopwatch watch { get; set; }


        public GameRoom()
        {
            gameId = ++GameRoom.gameCounter;
            turn = 0;
            round = 1;
            isEnded = false;
            watch = new Stopwatch();
            watch.Start();
        }

        ~GameRoom()  // destructor
        {
            Console.WriteLine("Destructing GameRoom: " + gameId);
        }


        public void AddGamePlayers(Pair<Player> players)
        {
            this.players = players;

            currentPlaying = players.First;

            players.First.gPlayer.mana += this.round;
            players.Second.gPlayer.mana += this.round;


            List<Card> p1Cards = GetRandomCards(players.First, 3);
            List<Card> p2Cards = GetRandomCards(players.Second, 3);
            players.First.gPlayer.AddCardToHand(p1Cards);
            players.Second.gPlayer.AddCardToHand(p2Cards);

            logger = new GameLogger(this);
        }

        // Gets a specific number of random cards from the card database
        public List<Card> GetRandomCards(Player p, int amount = 1)
        {
            List<Card> getCard = new List<Card>();

            for (var i = 0; i < amount; i++)
            {
                Card c = (Card)p.owningCards[rand.Next(p.owningCards.Count())].Clone();
                c.SetId();
                getCard.Add(c);
            }
            return getCard;
        }

        // Return GamePlayers of the game troom
        public Pair<Player> getPlayers()
        {
            return new Pair<Player>(players.First, players.Second);
        }

        /// <summary>
        /// Prepares the next turn
        /// </summary>
        public void NextTurn()
        {
            // Checks if it's player one or player 2's turn. Increments round on each of player 1's turn.
            if (++turn % 2 == 0)
            {
                round++;
                currentPlaying = players.First;
            }
            else
            {
                currentPlaying = players.Second;
            }

            // Add a new card to player
            currentPlaying.gPlayer.AddCard(currentPlaying);

            // Set mana equal to the round (unless +10)
            currentPlaying.gPlayer.mana = (this.round < 10) ? this.round : 10;

            // Reset hasAttacked (Attack once per turn limit)
            players.First.gPlayer.ResetAttacked();
            players.Second.gPlayer.ResetAttacked();
        }

        /// <summary>
        /// This object sends current game state to the game players, This happens every turn.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public Pair<Response> GenerateGameUpdate(bool create = false)
        {

            var p1Data = GameData.Get(this, true);
            var p2Data = GameData.Get(this, false);

            if (!create)
            {
                p1Data.Add(new JProperty("create", false));
                p2Data.Add(new JProperty("create", false));
            }
            else
            {
                p1Data.Add(new JProperty("create", true));
                p2Data.Add(new JProperty("create", true));
            }

            Pair<Response> gUpdates = new Pair<Response>(
                new Response(GameService.GameResponseType.GAME_CREATE, p1Data),
                new Response(GameService.GameResponseType.GAME_CREATE, p2Data)
                );

            // Sends player turn update
            p1Data.Add(new JProperty("yourTurn", players.First.gPlayer.IsPlayerTurn()));
            p2Data.Add(new JProperty("yourTurn", players.Second.gPlayer.IsPlayerTurn()));

            return gUpdates;
        }

        public void SaveGameRoom()
        {
            using (var session = NHibernateHelper.OpenSession())
            {


                using (var transaction = session.BeginTransaction())
                {
                    Souls.Model.Game g = new Souls.Model.Game();
                    g.player1 = this.players.First;
                    g.player2 = this.players.Second;
                    session.Save(g);
                    transaction.Commit();
                    this.gameId = g.id;
                }

            }
        }

        /// <summary>
        /// Determines weither the game is running or not. If its not running, a Message will be sent to the requester with Victory or Defeat
        /// </summary>
        /// <param name="gPlayer"> The gme player</param>
        /// <returns></returns>
        public void EndGame(bool isDraw)
        {
            Player player = players.First;
            Player opponent = players.Second;

            // Log winner
            GameLogger.LogTypes gameEndType = (isDraw) ? GameLogger.LogTypes.DRAW : GameLogger.LogTypes.WON;
            player.gPlayer.gameRoom.logger.Add(
                GameLogger.logTypes[gameEndType],
                player.gPlayer.gameRoom.winner.id,
                player.gPlayer.gameRoom.winner.GetOpponent().id,
                "Player",
                "Player"
                );

            // Publish to DB
            player.gPlayer.gameRoom.SaveGameRoom();
            player.gPlayer.gameRoom.logger.Publish();

            // Add Winner Points
            if (!isDraw)
            {
                player.gPlayer.gameRoom.winner.GiveWinnerPoints(1);
            }


            // Create Responses
            Response pResponse = new Response((player.gPlayer.gameRoom.winner == player) ?
                    GameService.GameResponseType.GAME_VICTORY :
                    GameService.GameResponseType.GAME_DEFEAT,
                    new JObject(
                        new JProperty("statistics", player.gPlayer.gameRoom.gameId)
                    )
                );

            Response oppResponse = new Response((opponent.gPlayer.gameRoom.winner == opponent) ?
                    GameService.GameResponseType.GAME_VICTORY :
                    GameService.GameResponseType.GAME_DEFEAT,
                    new JObject(
                        new JProperty("statistics", opponent.gPlayer.gameRoom.gameId)
                    )
                );

            // Check if draw
            if (isDraw)
            {
                pResponse.Type = GameService.GameResponseType.GAME_DRAW;
                oppResponse.Type = GameService.GameResponseType.GAME_DRAW;
            }


            // Send Responses
            player.gameContext.SendTo(
                pResponse
            );

            opponent.gameContext.SendTo(
                oppResponse
            );

            if (opponent.isBot) opponent.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_BOT_DISCONNECT, "xD Bot disconnect"));
            if (player.isBot) player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_BOT_DISCONNECT, "xD Bot disconnect"));



            //////////////////////////////////////////////////////////////////////////
            // CLEANUP 
            //////////////////////////////////////////////////////////////////////////
            player.gPlayer.gameRoom.isEnded = true;
            player.gPlayer = null;
            opponent.gPlayer = null;
            Player trash;

            if (player.chPlayer != null) // If chat failed to initate ignore! (Will be GCd automaticly.)
            {
                foreach (var room in player.chPlayer.memberRooms)
                {
                    ChatEngine.chatRooms.Remove(room.Key);
                }

                player.chPlayer.memberRooms.Clear();
                Clients.GetInstance().chatList.TryRemove(player.chatContext, out trash);
            }

            if (opponent.chPlayer != null) // If chat failed to initate ignore! (Will be GCd automaticly.)
            {
                opponent.chPlayer.memberRooms.Clear();
                Clients.GetInstance().chatList.TryRemove(opponent.chatContext, out trash);
            }

            player.chPlayer = null;
            opponent.chPlayer = null;

            Clients.GetInstance().gameList.TryRemove(player.gameContext, out trash);
            Clients.GetInstance().gameList.TryRemove(opponent.gameContext, out trash);


            Console.WriteLine("----------SUMMARY---------");
            Console.WriteLine("Game Rooms: " + GameEngine.rooms.Count);
            Console.WriteLine("Chat Rooms: " + ChatEngine.chatRooms.Count);
            Console.WriteLine("Chat Clients:" + Clients.GetInstance().chatList.Count);
            Console.WriteLine("Game Clients: " + Clients.GetInstance().gameList.Count);
        }


    }

}
