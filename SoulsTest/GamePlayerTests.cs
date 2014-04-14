using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Souls.Server.Objects;
using Souls.Server.Network;



namespace Souls.Test
{
    [TestClass]
    public class GamePlayerTests
    {
        GamePlayer gp1 { get; set; }
        GamePlayer gp2 { get; set; }

        public GamePlayerTests()
        {
            InitGamePlayers();

        }


        public void InitGamePlayers()
        {
            gp1 = new GamePlayer(new GameService(new Server.Game.GameEngine()));
            gp2 = new GamePlayer(new GameService(new Server.Game.GameEngine()));

            gp1.health = 10;
            gp1.name = "Hansel";
            gp1.rank = 10;
            gp1.mana = 0;
            gp1.attack = 5;

            gp2.health = 10;
            gp2.name = "Gretel";
            gp2.rank = 10;
            gp2.mana = 0;
            gp2.attack = 10;
        }

        [TestMethod]
        public void PlayerAttackCard()
        {
            Card card = new Card()
            {
                attack = 10,
                health = 10,
                cid = 1,
                armor = 10,
                id = 1,
                name = "CardTest :D",
                cost = 10
            };

            int cardStartHealth = card.health;
            int gpStartHealth = gp1.health;

            gp1.Attack(card);

            // Asset health loss on GamePlayer
            Assert.AreEqual(gpStartHealth - card.attack, gp1.health);

            // Assert health loss on Card
            Assert.AreEqual(cardStartHealth - gp1.attack, card.health);


        }
    }
}
