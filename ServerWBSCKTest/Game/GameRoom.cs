using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;

namespace ServerWBSCKTest
{
    public class GameRoom
    {
        public Pair<GamePlayer> players;
        public int gameId { get; set; }
        public int round { get; set; }

        // Contains the Player which is currently playing (Player's turn);
        public GamePlayer currentPlaying { get; set; }

        // Makes the players gameplayers and deals 3 cards to each player
        public GameRoom(Pair<GamePlayer> players, int gameId)
        {

            currentPlaying = players.First;

            this.players = new Pair<GamePlayer>(players.First, players.Second);
            this.gameId = gameId;


            List<Card> atta = getRandomCards(3);

            this.players.First.handCards.TryAddRange(getRandomCards(3));
            this.players.Second.handCards.TryAddRange(getRandomCards(3));

        }

        // Gets a specific number of random cards from the card database
        public List<Card> getRandomCards(int amount)
        {
            using (var db = new Model.soulsEntities())
            {
                List<Card> cards = new List<Card>();
                Random rand = new Random();

                for (int i = 0; i < amount; i++)
                {
                    int toSkip = rand.Next(0, db.db_Card.Count());
                    var getCard = db.db_Card.OrderBy(x => x.id).Skip(toSkip).Take(1).First();
                    var jsonCard = JsonConvert.SerializeObject(getCard);

                    //Console.WriteLine(jsonCard);
                    cards.Add(new Card().toCard(jsonCard));
                }
                return cards;
            };
        }
        // Gets one random card from the card database
        public Card getRandomCard()
        {
            using (var db = new Model.soulsEntities())
            {
                Random rand = new Random();

                int toSkip = rand.Next(0, db.db_Card.Count());
                var getCard = db.db_Card.OrderBy(x => x.id).Skip(toSkip).Take(1).First();
                var jsonCard = JsonConvert.SerializeObject(getCard);
                
                return new Card().toCard(jsonCard);
            };
        }

        public GamePlayer getOpponent(GamePlayer player)
        {
            if (this.players.First.Equals(player))
            {
                return this.players.Second;
            }
            else if (this.players.Second.Equals(player))
            {
                return this.players.First;

            }
            return null; //TODO? 
        }


        // Returns the players of this gameroom
        public Pair<Player> getPlayers()
        {
            return new Pair<Player>(players.First.toPlayer(), players.Second.toPlayer());
        }

        public void nextRound()
        {
            currentPlaying = (++round % 2 == 0) ? players.First : players.Second;

            // Add a new card to players
            players.First.handCards.Add(this.getRandomCard());
            players.Second.handCards.Add(this.getRandomCard());
            
            // Set mana equal to the round (unless +10)
            currentPlaying.mana = (this.round < 10) ? this.round : 10;
        }
    }

}
