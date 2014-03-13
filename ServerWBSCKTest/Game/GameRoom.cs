using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;
using Newtonsoft.Json.Linq;
using ServerWBSCKTest.Model;
using ServerWBSCKTest.Controller;

namespace ServerWBSCKTest
{
    public class GameRoom
    {
        public static int gameCounter { get; set; }

        public Pair<GamePlayer> players;
        public int gameId { get; set; }
        public int round { get; set; }

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

        }

        public void AddGamePlayer(Pair<GamePlayer> players)
        {
            this.players = players;


            currentPlaying = players.First;

            // this.players = new Pair<GamePlayer>(players.First, players.Second);


            List<Card> p1Cards = GetRandomCards(3);
            List<Card> p2Cards = GetRandomCards(3);
            foreach (var c in p1Cards)
            { 
                this.players.First.handCards.Add(c.cid, c);
            }
            foreach (var c in p2Cards)
            {
                this.players.Second.handCards.Add(c.cid, c);
            }

        }

        // Gets a specific number of random cards from the card database
        public List<Card> GetRandomCards(int amount = 1)
        {
                List<Card> getCard = new List<Card>();
                Random rand = new Random();

                for (var i = 0; i < amount; i++ )
                {
                    Card c = (Card)GameEngine.cards[rand.Next(GameEngine.cards.Count() - 1)].Clone();
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
        /// Runs next round (Changes currentlyPlayer object to opposite player) 
        /// Returns the new player
        /// </summary>
        public GamePlayer NextTurn()
        {
            currentPlaying = currentPlaying.GetOpponent();
            return currentPlaying;
        }

        public void NextRound()
        {
            currentPlaying = (++round % 2 == 0) ? players.First : players.Second;
            
            // Add a new card to players

            Card p1Card = this.GetRandomCards()[0];
            Card p2Card = this.GetRandomCards()[0];
            players.First.handCards.Add(p1Card.cid, p1Card);
            players.Second.handCards.Add(p2Card.cid, p2Card);

            // Set mana equal to the round (unless +10)
            currentPlaying.mana = (this.round < 10) ? this.round : 10;
        }



    }

}
