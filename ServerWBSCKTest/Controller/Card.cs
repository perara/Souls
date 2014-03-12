using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerWBSCKTest.Tools;
using ServerWBSCKTest.Model;
using ServerWBSCKTest.Engine;

namespace ServerWBSCKTest
{
    public class Card : Model.db_Card
    {

        public bool isDead { get; set; }
        public int cardId { get; set; }

        public Card()
        {

        }
        public Card toCard(string json)
        {
            //dynamic jsonCard = JsonConvert.DeserializeObject(json);
            Card card = JSONHelper.Deserialize<Card>(json);

            Console.WriteLine(card.ToString());
            return card;
        }


        // A card attacks another card (and lowering the hp by the amount of the defenders damage)
        public void Attack(ref Card defender)
        {
            defender.health -= this.attack;
            this.health -= defender.attack;

            if (this.health < 1) this.cardDie();
            if (defender.health < 1) defender.cardDie();
        }

        public void Attack(ref GamePlayer defender)
        {
            defender.health -= this.attack;
            this.health -= defender.attack;

            if (this.health < 1) this.cardDie();
            if (defender.health < 1) defender.playerDie();
        }

        // A card attacks another collection of cards
        public void Attack(ref LinkedList<Card> defenders)
        {
            foreach (Card card in defenders)
            {
                card.health -= this.attack;
                if (card.health < 1) card.cardDie();
            }
        }

        // Removes a specific card only if used
        public void cardDie()
        {
            this.isDead = true;
        }



        public override string ToString()
        {
            return this.id + ", " + this.name + ", " + this.attack + ", " + this.health + ", " + this.armor + ", " + this.fk_ability + ", " + this.fk_type;
        }
    }
}
