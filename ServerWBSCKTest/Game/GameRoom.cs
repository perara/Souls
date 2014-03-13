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


            List<Card> p1Cards = getRandomCards(3);
            List<Card> p2Cards = getRandomCards(3);
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
        public List<Card> getRandomCards(int amount)
        {
            using (var db = new Model.soulsEntities())
            {
                List<Card> cards = new List<Card>();
                Random rand = new Random();

                for (int i = 0; i < amount; i++)
                {
                    int toSkip = rand.Next(0, db.db_Card.Count());

                    var getCard = db.db_Card
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
                       ).Select(x => new
                        {
                            id = x.y.card.id,
                            cardId = cardCount,
                            name = x.y.card.name,
                            attack = x.y.card.attack,
                            health = x.y.card.health,
                            armor = x.y.card.armor,
                            cost = x.y.card.cost,
                            db_Ability = new
                            {
                                id = x.y.ability.id,
                                name = x.y.ability.name,
                                parameter = x.y.ability.parameter,
                            },
                            db_Card_Type = new
                            {
                                id = x.cType.id,
                                name = x.cType.name,
                            }
                        })
                        .OrderBy(x => x.id)
                        .Skip(toSkip).Take(1).FirstOrDefault();

                    cardCount++;

                    var jsonCard = JsonConvert.SerializeObject(getCard);

                    var newCard = new Card().toCard(jsonCard);


                    cards.Add(newCard);
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

        public GamePlayer GetOpponent(GamePlayer player)
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
            currentPlaying = GetOpponent(currentPlaying);
            return currentPlaying;
        }

        public void NextRound()
        {
            currentPlaying = (++round % 2 == 0) ? players.First : players.Second;

            // Add a new card to players

            Card p1Card = this.getRandomCard();
            Card p2Card = this.getRandomCard();
            players.First.handCards.Add(p1Card.cid, p1Card);
            players.Second.handCards.Add(p2Card.cid, p2Card);

            // Set mana equal to the round (unless +10)
            currentPlaying.mana = (this.round < 10) ? this.round : 10;
        }



    }

}
