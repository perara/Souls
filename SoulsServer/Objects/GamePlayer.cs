using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Souls.Server.Network;
using Souls.Server.Game;

namespace Souls.Server.Objects
{

    public class GamePlayer
    {

        /// <summary>
        /// Contains the hash which comes from the player client (See: Player)
        /// </summary>
        public string hash { get; set; }

        /// <summary>
        /// If its PLAYER 1 or 2
        /// </summary>
        public bool isPlayerOne { get; set; }


        public Dictionary<int, Card> handCards { get; set; }
        public Dictionary<int, Card> boardCards { get; set; }

        public bool isDead = false;
        public int attack { get; set; }
        public int health { get; set; }
        public int mana { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public GameRoom gameRoom { get; set; }
        public int type { get; set; }


        public General playerContext { get; set; }

        public GamePlayer(General playerContext)
        {
            this.playerContext = playerContext;
            handCards = new Dictionary<int, Card>(10);
            boardCards = new Dictionary<int, Card>(10); //TODO REMEMBER 10 (Should be a static value in a "config class orsmthing")
        }


        public Dictionary<string, string> GetPlayerData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("health", health.ToString());
            data.Add("attack", attack.ToString());
            data.Add("mana", mana.ToString());
            data.Add("name", name);
            data.Add("type", type.ToString());
            
            return data;
        }

        public bool HasEnoughMana(Card c)
        {
            return this.mana >= c.cost;
        }




        public void Attack(Card defCard)
        {
            defCard.health -= this.attack;
            this.health -= defCard.attack;

            if (this.health < 1) this.playerDie();
            if (defCard.health < 1) defCard.cardDie();
        }

        public void playerDie()
        {
            isDead = true;
        }

        public Player GetOpponent()
        {
            if (this.gameRoom.players.First.gPlayer.Equals(this))
            {
                return this.gameRoom.players.Second;
            }
            else if (this.gameRoom.players.Second.gPlayer.Equals(this))
            {
                return this.gameRoom.players.First;
            }
            return null; //TODO? 
        }


        public bool IsPlayerTurn()
        {
            return (this.Equals(this.gameRoom.currentPlaying.gPlayer)) ? true : false;
        }

        public List<Card> AddCard(int amount = 1)
        {
            return gameRoom.GetRandomCards(amount);
        }

        public void AddCardToHand(List<Card> cards)
        {
            foreach (var c in cards)
            {
                this.handCards.Add(c.cid, c);
            }

        }


        public void RemoveHandCard(Card c)
        {
            this.handCards.Remove(c.slotId);
        }

        public void RemoveBoardCard(Card c)
        {
            this.boardCards.Remove(c.slotId);
        }


    }
}
