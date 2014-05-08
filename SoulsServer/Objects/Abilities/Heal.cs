using Souls.Server.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Souls.Server.Network;
using Souls.Server.Game;

namespace SoulsServer.Objects.Abilities
{
    public class Heal : AbilityBase
    {
        public Heal(Player p) : base(p) { this.abilityId = 1; }


        new public void Use(Card src, Player tar)
        {
            // Base + Rank
            int type = 1;
            int healAmount = 2 + p.rank;
            src.hasAttacked = true;

            tar.gPlayer.health += healAmount;

            p.gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", false),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.id),
                        new JProperty("parameter", tar.gPlayer.health)
                )));

            p.GetOpponent().gameContext.SendTo(
                new Response(
                 GameService.GameResponseType.GAME_USE_ABILITY,
            new JObject(
                new JProperty("abilityId", this.abilityId),
                new JProperty("type", type),
                new JProperty("wasOpponent", true),
                new JProperty("source", src.cid),
                new JProperty("target", tar.id),
                new JProperty("parameter", tar.gPlayer.health)
            )));

            p.gPlayer.gameRoom.logger.Add(
            GameLogger.logTypes[GameLogger.LogTypes.ABILITY_HEAL],
            src.id,
            tar.id,
            "Card",
            "Player"
            );

        }

        new public void Use(Card src, Card tar)
        {
            // Base + Rank
            int type = 0;
            int healAmount = 2 + p.rank;
            src.hasAttacked = true;

            tar.health += healAmount;

            p.gameContext.SendTo(
                new Response(
                    GameService.GameResponseType.GAME_USE_ABILITY,
                    new JObject(
                        new JProperty("abilityId", this.abilityId),
                        new JProperty("type", type),
                        new JProperty("wasOpponent", false),
                        new JProperty("source", src.cid),
                        new JProperty("target", tar.cid),
                        new JProperty("parameter", tar.health)
                )));

            p.GetOpponent().gameContext.SendTo(
                new Response(
                 GameService.GameResponseType.GAME_USE_ABILITY,
            new JObject(
                new JProperty("abilityId", this.abilityId),
                new JProperty("type", type),
                new JProperty("wasOpponent", true),
                new JProperty("source", src.cid),
                new JProperty("target", tar.cid),
                new JProperty("parameter", tar.health)
            )));

            p.gPlayer.gameRoom.logger.Add(
            GameLogger.logTypes[GameLogger.LogTypes.ABILITY_HEAL],
            src.id,
            tar.id,
            "Card",
            "Card"
            );


        }



    }
}
