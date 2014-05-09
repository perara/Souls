using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Objects;
using System.Collections.Concurrent;

namespace SoulsServer.Objects
{
    public class Bot : GamePlayer
    {
        public ConcurrentDictionary<int, Card> handCards { get; set; }
        public ConcurrentDictionary<int, Card> boardCards { get; set; }
        
        public Bot()
        {
            handCards = new ConcurrentDictionary<int, Card>();
            boardCards = new ConcurrentDictionary<int, Card>();
        }


    }
}
