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
using SoulsServer.Network;
using SoulsModel;

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


        public GameRoom()
        {
            gameId = ++GameRoom.gameCounter;
            turn = 0;
            round = 1;
            isEnded = false;
        }

        public void AddGamePlayers(Pair<Player> players)
        {
            this.players = players;

            currentPlaying = players.First;

            players.First.gPlayer.mana += this.round;
            players.Second.gPlayer.mana += this.round;


            List<Card> p1Cards = GetRandomCards(3);
            List<Card> p2Cards = GetRandomCards(3);
            players.First.gPlayer.AddCardToHand(p1Cards);
            players.Second.gPlayer.AddCardToHand(p2Cards);

            logger = new GameLogger(this);
        }

        // Gets a specific number of random cards from the card database
        public List<Card> GetRandomCards(int amount = 1)
        {
            List<Card> getCard = new List<Card>();

            for (var i = 0; i < amount; i++)
            {
                Card c = (Card)GameEngine.cards[rand.Next(GameEngine.cards.Count())].Clone();
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
            currentPlaying.gPlayer.AddCard();

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
            using(var session = NHibernateHelper.OpenSession())
            {


                using(var transaction = session.BeginTransaction())
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


    }

}
