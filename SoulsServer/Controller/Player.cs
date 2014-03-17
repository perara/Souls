using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Engine;
using SoulsServer.Controller;

namespace SoulsServer
{
    public class Player
    {
        /// <summary>
        /// Player information from database
        /// </summary>
        public int id { get; set; }
        public int rank { get; set; }
        public string name { get; set; }
        public string hash { get; set; }


        /// <summary>
        ///  Flag which determines if the player is already in queue
        /// </summary>
        public bool inQueue { get; set; }

        /// <summary>
        /// Contains the game player corresponding to this player
        /// </summary>
        public GamePlayer gPlayer { get; set; }

        /// <summary>
        /// Contains chatPlayer which controls all of the chat handling
        /// </summary>
        public ChatPlayer chPlayer { get; set; }

        /// <summary>
        /// Context (Connection) to the GameServer
        /// </summary>
        public General gameContext { get; set; }

        /// <summary>
        /// Context (Connection) to the ChatServer
        /// </summary>
        public General chatContext { get; set; }


        public bool ValidateHash()
        {
            using (var db = new Model.soulsEntities())
            {
                var hashExists = (from b in db.db_Player_Hash where b.hash == hash select b).FirstOrDefault();
                if (hashExists != null) return true;
                else return false;
            }
        }

        public bool fetchPlayerInfo()
        {

            using (var db = new Model.soulsEntities())
            {

                // TODO this.
                var dbPlayer = db.db_Player_Hash.FirstOrDefault(x => x.hash == hash).db_Player;
                this.id = dbPlayer.id;
                this.name = dbPlayer.name;
                this.rank = dbPlayer.rank;

                if (dbPlayer != null) return true;
                else return false;
            }
        }

        /// <summary>
        /// This fetches the newest hash available for the player //TODO this may fail? Make a gameList with all the hashes ? NEED TEST
        /// </summary>
        /// <returns></returns>
        public string UpdateHash()
        {
            using (var db = new Model.soulsEntities())
            {
                var dbPlayer = db.db_Player_Hash.FirstOrDefault(x => x.fk_player_id == id);
                
                if(!this.hash.Equals(dbPlayer.hash))
                {
                    Console.WriteLine("New hash found, updating!");
                    this.hash = dbPlayer.hash;
                }
            }

            return this.hash; // This will always be either the same or a new one (Same would be "updated" if no new was found)
        }
    }
}
