using Souls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoulsClient.Classes
{
    public class PlayerClass
    {
        public Race race { get; set; }
        public List<PlayerType> types { get; set; }

        public PlayerClass(Race race)
        {
            this.types = new List<PlayerType>();
            this.race = race;
        }


        public void AddPlayerType(PlayerType type)
        {
            this.types.Add(type);
        }


    }
}