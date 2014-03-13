using ServerWBSCKTest.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace ServerWBSCKTest.Game
{
    public class GameData
    {

        public Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
        public GameRoom room { get; set; }
        /// <summary>
        /// Its important that the PREFIX p1_ or p2_ is specified on those fields which is seperate from both players. this will be used to determine the final structure of the JSON
        /// </summary>
        /// <param name="room"></param>
        public GameData(GameRoom room)
        {
            this.room = room;


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

        public JObject Get(bool playerOne)
        {
            GamePlayer player;
            GamePlayer opponent;
            if (playerOne)
            {
                player = room.players.First;
                opponent = room.players.Second;
            }
            else
            {
                player = room.players.Second;
                opponent = room.players.First;
            }

            JObject obj = new JObject(
                 new JProperty("gameId", room.gameId),
                 new JProperty("round", room.round),
                 new JProperty("ident", 1),
                 new JProperty("player", new JObject(
                    new JProperty("info", JObject.FromObject(player.GetPlayerData())),
                    new JProperty("board", JObject.FromObject(player.boardCards)),
                    new JProperty("hand", JObject.FromObject(player.handCards))
                    )),
                new JProperty("opponent", new JObject(
                    new JProperty("info", JObject.FromObject(opponent.GetPlayerData())),
                    new JProperty("board", JObject.FromObject(opponent.boardCards)),
                    new JProperty("hand", opponent.handCards.Keys)
                    )
                ));

            return obj;
        }

    }
}
