using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using SoulsServer.Engine;
using SoulsServer.Tools;
using Newtonsoft.Json.Linq;
using SoulsServer.Controller;
using SoulsServer.Game;

namespace SoulsServer
{
    public class GameRoom
    {
        public static int gameCounter { get; set; }

        public Pair<GamePlayer> players;
        public int gameId { get; set; }
        public int turn { get; set; }
        public int round { get; set; }

        Random rand = new Random((int)DateTime.Now.Millisecond);

        /// <summary>
        /// Count number or cards in the room. is used as identifier on cards with gameId (gameId * cardCount)
        /// </summary>
        public int cardCount { get; set; }
        // Contains the Player which is currently playing (Player's turn);
        public GamePlayer currentPlaying { get; set; }

        // Makes the players gameplayers and deals 3 cards to each player
        public GameRoom()
        {
            gameId = GameRoom.gameCounter++;
            turn = 0;
            round = 1;
        }

        public void AddGamePlayers(Pair<GamePlayer> players)
        {
            this.players = players;

            currentPlaying = players.First;


            List<Card> p1Cards = GetRandomCards(3);
            List<Card> p2Cards = GetRandomCards(3);
            players.First.AddCardToHand(p1Cards);
            players.Second.AddCardToHand(p2Cards);
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
        public Pair<GamePlayer> getPlayers()
        {
            return new Pair<GamePlayer>(players.First, players.Second);
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
            currentPlaying.AddCard();

            // Set mana equal to the round (unless +10)
            currentPlaying.mana = (this.round < 10) ? this.round : 10;
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
            p1Data.Add(new JProperty("yourTurn", players.First.IsPlayerTurn()));
            p2Data.Add(new JProperty("yourTurn", players.Second.IsPlayerTurn()));

            return gUpdates;
        }
    }

}
