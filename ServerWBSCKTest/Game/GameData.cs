using ServerWBSCKTest.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServerWBSCKTest.Game
{
    public class GameData
    {
        public int gameId;
        public List<Card> player_one_hand;
        public List<Card> player_one_board;

        public List<Card> player_two_hand;
        public List<Card> player_two_board;

        public int round;

        public GameData(GameRoom room)
        {
            player_one_hand = new List<Card>();
            player_one_board = new List<Card>();

            player_two_hand = new List<Card>();
            player_two_board = new List<Card>();

            this.gameId = room.gameId;

            this.player_one_hand = room.players.First.handCards;
            this.player_one_board = room.players.First.boardCards;

            this.player_two_hand = room.players.Second.handCards;
            this.player_two_board = room.players.Second.boardCards;
            this.round = room.round;
        }
    }
}
