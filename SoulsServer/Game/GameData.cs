using SoulsServer.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace SoulsServer.Game
{
    public class GameData
    {
       
        private GameData()
        {
        }

        public static JObject Get(GameRoom room, bool playerOne)
        {
            GamePlayer player;
            GamePlayer opponent;
            var ident = -1;
            if (playerOne)
            {
                ident = 1;
                player = room.players.First;
                opponent = room.players.Second;
            }
            else
            {
                ident = 2;
                player = room.players.Second;
                opponent = room.players.First;
            }

            // Construct a game update object 
            JObject obj = new JObject(
                 new JProperty("gameId", room.gameId),
                 new JProperty("round", room.round),
                 new JProperty("ident", ident),
                 new JProperty("player", new JObject(
                    new JProperty("info", JObject.FromObject(player.GetPlayerData())),
                    new JProperty("board", JObject.FromObject(player.boardCards)),
                    new JProperty("hand", JObject.FromObject(player.handCards))
                    )),
                new JProperty("opponent", new JObject(
                    new JProperty("info", JObject.FromObject(opponent.GetPlayerData())),
                    new JProperty("board", JObject.FromObject(opponent.boardCards)),
                    new JProperty("hand", from h in opponent.handCards
                                          select
                                              new JObject(
                                                   new JObject(
                                                    new JProperty("cid", h.Value.cid))
                                       ))
                    )
                ));

            return obj;
        }

    }
}
