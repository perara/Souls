﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Souls.Server.Tools;
using Souls.Server.Engine;
using Souls.Server.Objects;

namespace Souls.Server.Objects
{
    public class Card : Souls.Model.Card, ICloneable
    {
        private static int idCounter { get; set; }
        public bool isDead { get; set; }
        public int cid { get; set; }

        // Which slot the card resides in.
        public int slotId { get; set; }

        public bool hasAttacked { get; set; }


        /*  public Card(Souls.Model.Card modelCard) : base(modelCard)
          {
          }*/

        ~Card()  // destructor
        {
            //Console.WriteLine("Destructing Card: " + cid);
        }

        public void SetId()
        {
            cid = ++Card.idCounter;
        }


        // A card attacks another card (and lowering the hp by the amount of the defenders damage)
        public void Attack(Card defender)
        {
            defender.health -= this.attack;
            this.health -= defender.attack;

            if (this.health < 1)
            {
                this.CardDie();
            }

            if (defender.health < 1)
            {
                defender.CardDie();
            }

            this.hasAttacked = true;
        }

        public void Attack(GamePlayer defender)
        {
            defender.health -= this.attack;
            this.health -= defender.attack;

            if (this.health < 1) this.CardDie();
            if (defender.health < 1) defender.playerDie();

            this.hasAttacked = true;
        }

        // A card attacks another collection of cards
        public void Attack(ref LinkedList<Card> defenders)
        {
            foreach (Card card in defenders)
            {
                card.health -= this.attack;
                if (card.health < 1) card.CardDie();
            }

            this.hasAttacked = true;
        }

        // Removes a specific card only if used
        public void CardDie()
        {
            this.isDead = true;
        }



        public override string ToString()
        {
            return this.cid + ", " + this.name + ", " + this.attack + ", " + this.health + ", " + this.armor + ", " + this.ability.id + ", " + this.race.id;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }


    }
}
