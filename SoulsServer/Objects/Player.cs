using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Engine;
using Souls.Server.Objects;
using Souls.Server.Tools;
using SoulsModel;
using Souls.Model;
using NHibernate.Criterion;
using NHibernate;
using NHibernate.Linq;
using Souls.Server.Network;

namespace Souls.Server.Objects
{
    public class Player : Souls.Model.Player
    {
        public string hash { get; set; }
        /// <summary>
        ///  Flag which determines if the player is already in queue
        /// </summary>
        public bool inQueue { get; set; }
        /// <summary>
        /// Contains GamePlayer which controls all of the chat handling
        /// </summary>
        public GamePlayer gPlayer { get; set; }

        /// <summary>
        /// Contains chatPlayer which controls all of the chat handling
        /// </summary>
        public ChatPlayer chPlayer { get; set; }

        /// <summary>
        /// Context (Connection) to the GameServer
        /// </summary>
        public Client gameContext { get; set; }

        /// <summary>
        /// Context (Connection) to the ChatServer
        /// </summary>
        public Client chatContext { get; set; }

         public Player GetOpponent()
        {
            if (this.gPlayer.gameRoom.players.First.Equals(this))
            {
                return this.gPlayer.gameRoom.players.Second;
            }
            else if (this.gPlayer.gameRoom.players.Second.Equals(this))
            {
                return this.gPlayer.gameRoom.players.First;
            }
            return null; 
        }


        public void ConstructGamePlayer(bool playerOne)
        {
            this.gPlayer = new GamePlayer()
            {  // TODO missing any?
                hash = this.hash,
                name = this.name,
                mana = this.playerType.mana,
                attack = this.playerType.attack,
                health = this.playerType.health,
                rank = this.rank,
                isPlayerOne = playerOne,
                gameRoom = null,
                type = this.playerType.id
            };
        }

        public void DestructGamePlayer()
        {
            this.gPlayer = null; //TODO

        }

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

        /// <summary>
        /// Fetches all of the player information from the live database
        /// </summary>
        /// <returns>A boolean signaling success of this operation</returns>
        public bool FetchPlayerInfo()
        {
            using (var session = NHibernateHelper.OpenSession())
            {

                PlayerLogin pLoginRecord2 = session.Query<PlayerLogin>()
                    .Where(x => x.hash == this.hash)

                    .Fetch(x => x.player)
                    .ThenFetch(x => x.playerType)
                    .ThenFetch(x => x.ability)

                    .Fetch(x => x.player)
                    .ThenFetch(x => x.playerType)
                    .ThenFetch(x => x.race)

                    .SingleOrDefault();

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
                    this.playerType = pType;


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
        /// Fetches new hash from the Database
        /// </summary>
        /// <returns>The hash</returns>
        public string UpdateHash()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {

                    PlayerLogin pLoginRecord = session.Query<PlayerLogin>()
                        .Where(x => x.player.id == this.id)
                        .SingleOrDefault();

                    if (pLoginRecord != null)
                    {
                        if (pLoginRecord.hash != this.hash)
                        {
                            Logging.Write(Logging.Type.GENERAL, "New hash found, updating!");
                            this.hash = pLoginRecord.hash;
                        }
                    }
                    else
                    {
                        Logging.Write(Logging.Type.ERROR, "It would have crashed :D");
                    }
                    return this.hash; // This will always be either the same or a new one (Same would be "updated" if no new was found)


                }
            }

        }
    }
}
