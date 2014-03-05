using ServerWBSCKTest.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Engine
{

    public class GamePlayer : Player
    {
        public LimitedList<Card> handCards { get; set; }
        public LimitedList<Card> boardCards { get; set; }

        public int health { get; set; }
        public int mana { get; set; }

        public bool isDead = false;

        public GamePlayer()
        {
            handCards = new LimitedList<Card>(10);
            boardCards = new LimitedList<Card>(10); //TODO REMEMBER 10 (Should be a static value in a "config class orsmthing")
        }

        public GamePlayer(string hash)
        {
            this.hash = hash;
            handCards = new LimitedList<Card>(10);
            boardCards = new LimitedList<Card>(10); //TODO REMEMBER 10 (Should be a static value in a "config class orsmthing")
        }

        public Player toPlayer()
        {
            Player player = new Player();

            player.id = this.id;
            player.name = this.name;
            player.rank = this.rank;
            player.fk_type = this.fk_type;
            player.timestamp = this.timestamp;

            return player;
        }

        public bool Equals(string hash)
        {
            return (this.hash == hash) ? true : false;
        }


        public void Attack(Card defCard)
        {
            defCard.health -= this.db_Player_Type.attack;
            this.health -= defCard.attack;

            if (this.health < 1) this.playerDie();
            if (defCard.health < 1) defCard.cardDie();
        }

        public void playerDie()
        {
            isDead = true;
        }
    }
}
