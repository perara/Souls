using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Engine;
using SoulsServer.Controller;
using SoulsServer.Tools;

namespace SoulsServer
{
    public class Player
    {
        /// <summary>
        /// Player information from database
        /// </summary>
        public int id { get; set; }
        public int rank { get; set; }
        public string hash { get; set; }

        public string name { get; set; }
        public int attack { get; set; }
        public int mana { get; set; }
        public int health { get; set; }
        public int armor { get; set; }


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

                var dbPlayer_hash = db.db_Player_Hash.FirstOrDefault(x => x.hash == hash);

                if (dbPlayer_hash != null)
                {
                    var dbPlayer = dbPlayer_hash.db_Player;
                    var dbPlayerType = dbPlayer.db_Player_Type;
                    this.id = dbPlayer.id;
                    this.name = dbPlayer.name;
                    this.rank = dbPlayer.rank;
                    this.health = dbPlayer.db_Player_Type.health;
                    this.mana = 1;//TODO should be contained in player type?
                    this.armor = dbPlayer.db_Player_Type.armor;
                    this.attack = dbPlayer.db_Player_Type.attack;

                    return true;
                }
                else
                {
                    Logging.Write(Logging.Type.GAME, "db_PHash was null! (NOT LOGGED IN?)");

                    return false;
                }

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

                if (!this.hash.Equals(dbPlayer.hash))
                {
                    Logging.Write(Logging.Type.GENERAL, "New hash found, updating!");
                    this.hash = dbPlayer.hash;
                }
            }

            return this.hash; // This will always be either the same or a new one (Same would be "updated" if no new was found)
        }
    }
}
