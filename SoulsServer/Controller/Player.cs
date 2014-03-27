using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Engine;
using SoulsServer.Controller;
using SoulsServer.Tools;
using SoulsModel;
using Souls.Model;
using NHibernate.Criterion;
using NHibernate;

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
            using (var session = NHibernateHelper.OpenSession())
            {
                PlayerLogin pLoginRecord = session.CreateCriteria<PlayerLogin>()
                    .Add(Restrictions.Eq("hash", this.hash))
                    .UniqueResult<PlayerLogin>();

                // If a record was found, the Hash is validated
                if (pLoginRecord != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool fetchPlayerInfo()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                PlayerLogin pLoginRecord = session.CreateCriteria<PlayerLogin>()
                    .Add(Restrictions.Eq("hash", this.hash))
                    .UniqueResult<PlayerLogin>();


                if (pLoginRecord != null)
                {
                    Souls.Model.Player pObj = pLoginRecord.player;
                    Souls.Model.PlayerType pType = pObj.playerType;

                    this.id = pObj.id;
                    this.name = pObj.name;
                    this.rank = pObj.rank;
                    this.health = pType.health;
                    this.mana = pType.mana;
                    this.armor = pType.armor;
                    this.attack = pType.attack;
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
        /// This fetches the newest hash available for the player 
        /// </summary>
        /// <returns></returns>
        public string UpdateHash()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {

                    PlayerLogin pLoginRecord = session.CreateCriteria<PlayerLogin>()
                    .Add(Restrictions.Eq(Projections.Property<PlayerLogin>(x => x.player.id), this.id))
                    .UniqueResult<PlayerLogin>();

                    if (pLoginRecord.hash != this.hash)
                    {
                        Logging.Write(Logging.Type.GENERAL, "New hash found, updating!");
                        this.hash = pLoginRecord.hash;
                    }
                    return this.hash; // This will always be either the same or a new one (Same would be "updated" if no new was found)


                }
            }

        }
    }
}
