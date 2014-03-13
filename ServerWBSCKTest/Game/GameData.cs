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

        public Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

        /// <summary>
        /// Its important that the PREFIX p1_ or p2_ is specified on those fields which is seperate from both players. this will be used to determine the final structure of the JSON
        /// </summary>
        /// <param name="room"></param>
        public GameData(GameRoom room)
        {
            data.Add("p1_hand", room.players.First.handCards.Values);
            data.Add("p2_hand", room.players.Second.handCards.Values);
            data.Add("p1_board", room.players.First.boardCards.Values);
            data.Add("p2_board", room.players.Second.boardCards.Values);
            data.Add("gameId", room.gameId);
            data.Add("round", room.round);
            data.Add("p1_hand_count", room.players.First.handCards.Count);
            data.Add("p2_hand_count", room.players.Second.handCards.Count);
            data.Add("p1", room.players.First.GetPlayerData());
            data.Add("p2", room.players.Second.GetPlayerData());
            data.Add("p1_ident", 1);
            data.Add("p2_ident", 2);
        }



        /// <summary>
        ///  data.Add("p1_hand", room.players.First.handCards);
        ///   data.Add("p2_hand", room.players.Second.handCards);
        ///data.Add("p1_board", room.players.First.boardCards);
        ///data.Add("p2_board", room.players.Second.boardCards);
        ///data.Add("gameId", this.gameId = room.gameId);
        ///data.Add("round", room.round);
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Dictionary<string, dynamic> Get(string[] values, bool playerOne)
        {
            Dictionary<string, dynamic> retData = new Dictionary<string, dynamic>();
            foreach (string i in values)
            {
                dynamic value;
                if (data.TryGetValue(i, out value))
                {
                    retData.Add((playerOne) ?
                        i.Replace("p1_", "").Replace("p1", "player").Replace("p2", "opponent") :
                        i.Replace("p2_", "").Replace("p2", "player").Replace("p1", "opponent"), value);
                }
                else
                {
                    Console.WriteLine("Wrong INDEX");
                }
            }
            return retData;
        }
    }
}
