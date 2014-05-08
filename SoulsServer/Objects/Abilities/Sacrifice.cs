using Souls.Server.Network;
using Souls.Server.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Souls.Server.Game;

namespace SoulsServer.Objects.Abilities
{
    public class Sacrifice : AbilityBase
    {
        public Sacrifice(Player p) : base(p) { this.abilityId = 2; }


        new public void Use(Card src, Player tar)
        {
            int type = 2;
            src.CardDie();
            p.gPlayer.RemoveBoardCard(src);

            tar.gPlayer.health += src.health;
            tar.gPlayer.attack += src.attack;
            //tar.gPlayer.armor += src.armor;

            p.gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", false),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.id),
                        new JProperty("parameter", tar.gPlayer.health + ":" + tar.gPlayer.attack)
                    )
                )
            );

            p.GetOpponent().gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", true),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.id),
                        new JProperty("parameter", tar.gPlayer.health + ":" + tar.gPlayer.attack)
                    )
                )
            );

            p.gPlayer.gameRoom.logger.Add(
            GameLogger.logTypes[GameLogger.LogTypes.ABILITY_SACRIFICE],
            src.id,
            tar.id,
            "Card",
            "Player"
            );




        }

        new public void Use(Card src, Card tar)
        {
            int type = 1; // Card on Card
            src.CardDie();
            p.gPlayer.RemoveBoardCard(src);

            tar.health += src.health;
            tar.attack += src.attack;
            tar.armor += src.armor;

            p.gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", false),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.cid),
                        new JProperty("parameter", tar.health + ":" + tar.attack + ":" + tar.armor)
                    )
                )
            );

            p.GetOpponent().gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", true),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.cid),
                        new JProperty("parameter", tar.health + ":" + tar.attack + ":" + tar.armor)
                    )
                )
            );

            p.gPlayer.gameRoom.logger.Add(
            GameLogger.logTypes[GameLogger.LogTypes.ABILITY_SACRIFICE],
            src.id,
            tar.id,
            "Card",
            "Card"
            );
        }

    }
}
