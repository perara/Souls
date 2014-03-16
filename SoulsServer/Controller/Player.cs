using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Engine;

namespace SoulsServer
{
    public class Player
    {
        /// <summary>
        /// Player information from database
        /// </summary>
        public int rank { get; set; }
        public string name { get; set; }
        public string hash { get; set; }

        /// <summary>
        /// Variables which is used to determine state of the player in the server
        /// </summary>
        public bool inQueue { get; set; }
        public General gameContext { get; set; }
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

                this.name = dbPlayer.name;
                this.rank = dbPlayer.rank;

                if (dbPlayer != null) return true;
                else return false;
            }
        }
    }
}
