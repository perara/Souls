using Souls.Server.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsServer.Objects.Abilities
{
    public class AbilityBase
    {
        public Player p { get; set; }

        public AbilityBase(Player p)
        {
            this.p = p;
        }


        /// <summary>
        /// Should match the Ability ID from DB
        /// </summary>
        public int abilityId { get; set; }

        /// <summary>
        /// Processes the usage of the Ability
        /// </summary>
        /// <param name="c">The card (Source)</param>
        /// <param name="p">The player (Target)</param>
        /// <returns></returns>
        virtual public bool Use(Card c, Player p)
        {
            throw new NotImplementedException("Ability function not implemented.");
        }

        /// <summary>
        /// Processes the usage of the Ability
        /// </summary>
        /// <param name="p">The Card (Source)</param>
        /// <param name="c">The Card (Target)</param>
        /// <returns></returns>
        virtual public bool Use(Card c, Card c2)
        {
            throw new NotImplementedException("Ability function not implemented.");
        }

        /// <summary>
        /// Simple check to see if this ability matches the ID to the Card ability ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AbilityMatch(Card c)
        {
            return (c.ability.id == this.abilityId) ? true : false;
        }





    }
}
