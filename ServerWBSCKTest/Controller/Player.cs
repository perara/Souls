using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy.Classes;
using ServerWBSCKTest.Engine;

namespace ServerWBSCKTest
{
    public class Player : Model.db_Player
    {
        public string hash { get; set; }
        public bool chatActive { get; set; }
        public bool inQueue { get; set; }
        public bool validateHash()
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
                var dbPlayer = db.db_Player_Hash.FirstOrDefault(x => x.hash == hash).db_Player;

                this.id = dbPlayer.id;
                this.name = dbPlayer.name;
                this.rank = dbPlayer.rank;

                if (dbPlayer != null) return true;
                else return false;
            }
        }

        public GamePlayer toGamePlayer()
        {
            GamePlayer gameplayer = new GamePlayer();

            gameplayer.id = this.id;
            gameplayer.name = this.name;
            gameplayer.rank = this.rank;
            gameplayer.fk_type = this.fk_type;
            gameplayer.timestamp = this.timestamp;

            return gameplayer;
        }


    }
}
