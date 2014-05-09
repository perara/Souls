using Souls.Model;
using Souls.Server.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Tools;
using System.Diagnostics;
using Souls.Model.Helpers;

namespace Souls.Server.Game
{
    public class GameLogger
    {
        List<GameLog> records;
        public static Dictionary<LogTypes, GameLogType> logTypes { get; set; }
        Souls.Model.Game game { get; set; }

        GameRoom gameRoom { get; set; }

        public GameLogger(GameRoom room)
        {


            records = new List<GameLog>();

            Souls.Model.Game game = new Souls.Model.Game();
            game.id = room.gameId;
            game.player1 = room.players.First;
            game.player2 = room.players.Second;
            this.game = game;

            this.gameRoom = room;
        }

        public enum LogTypes
        {
            WON = 1,
            DEFEAT,
            DRAW,
            CARD_ATTACK_PLAYER,
            CARD_ATTACK_CARD,
            PLAYER_ATTACK_PLAYER,
            PLAYER_ATTACK_CARD,
            GAME_CREATED,
            USE_CARD,
            NEXT_TURN,
            NEW_CARD,
            KICKED,
            ABILITY_HEAL,
            ABILITY_SACRIFICE
        }


        public static void GenerateLogTypes()
        {
            Dictionary<LogTypes, GameLogType> logTypes = new Dictionary<LogTypes, GameLogType>();


            logTypes.Add(LogTypes.WON, new GameLogType()
            {
                id = (int)LogTypes.WON,
                title = "WON",
                description = "Player Won",
                text = "{0} wins against {1}"
            });

            logTypes.Add(LogTypes.DEFEAT, new GameLogType()
            {
                id = (int)LogTypes.DEFEAT,
                title = "DEFEAT",
                description = "Player defeat",
                text = "{0} loses against {1}"
            });

            logTypes.Add(LogTypes.DRAW, new GameLogType()
            {
                id = (int)LogTypes.DRAW,
                title = "DRAW",
                description = "Draw between players",
                text = "{0} plays a draw against {1}"
            });

            logTypes.Add(LogTypes.CARD_ATTACK_PLAYER, new GameLogType()
            {
                id = (int)LogTypes.CARD_ATTACK_PLAYER,
                title = "CARD_ATTACK_PLAYER",
                description = "A card attacks a player (opponent)",
                text = "{0} attacks {1} for {2} damage."
            });

            logTypes.Add(LogTypes.CARD_ATTACK_CARD, new GameLogType()
            {
                id = (int)LogTypes.CARD_ATTACK_CARD,
                title = "CARD_ATTACK_CARD",
                description = "A card attacks a card ",
                text = "{0} attacks {1} for {2} damage."
            });

            logTypes.Add(LogTypes.PLAYER_ATTACK_PLAYER, new GameLogType()
           {
               id = (int)LogTypes.PLAYER_ATTACK_PLAYER,
               title = "PLAYER_ATTACK_PLAYER",
               description = "A player attack the opponent",
               text = "{0} attacks {1} for {2} damage."
           });


            logTypes.Add(LogTypes.PLAYER_ATTACK_CARD, new GameLogType()
           {
               id = (int)LogTypes.PLAYER_ATTACK_CARD,
               title = "PLAYER_ATTACK_CARD",
               description = "A player attack a card",
               text = "{0} attacks {1} for {2} damage."
           });

            logTypes.Add(LogTypes.GAME_CREATED, new GameLogType()
            {
                id = (int)LogTypes.GAME_CREATED,
                title = "GAME_CREATED",
                description = "A game was created where X player begins",
                text = "The battle begins, {0} starts the game against {1}"
            });

            logTypes.Add(LogTypes.USE_CARD, new GameLogType()
            {
                id = (int)LogTypes.USE_CARD,
                title = "USE_CARD",
                description = "A player uses a card",
                text = "{0} uses {1}"
            });

            logTypes.Add(LogTypes.NEXT_TURN, new GameLogType()
            {
                id = (int)LogTypes.NEXT_TURN,
                title = "NEXT_TURN",
                description = "a player initiates next turn",
                text = "{0} gives turn to {1}"
            });

            logTypes.Add(LogTypes.NEW_CARD, new GameLogType()
            {
                id = (int)LogTypes.NEW_CARD,
                title = "NEW_CARD",
                description = "a player draws a new card",
                text = "{0} draws a card. The card was {1}"
            });

            logTypes.Add(LogTypes.KICKED, new GameLogType()
            {
                id = (int)LogTypes.KICKED,
                title = "KICKED",
                description = "a player is kicked",
                text = "{0} was kicked from the game by [ADMIN]"
            });

            logTypes.Add(LogTypes.ABILITY_HEAL, new GameLogType()
            {
                id = (int)LogTypes.ABILITY_HEAL,
                title = "ABILITY_HEAL",
                description = "A card heals X",
                text = "{0} heals {1} for {2} hp"
            });

            logTypes.Add(LogTypes.ABILITY_SACRIFICE, new GameLogType()
            {
                id = (int)LogTypes.ABILITY_SACRIFICE,
                title = "ABILITY_SACRIFICE",
                description = "A card sacrifices itself to X",
                text = "{0} sacrifices itself, strengthening {1} for {2}."
            });



            GameLogger.logTypes = logTypes;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var i in GameLogger.logTypes)
                    {

                        GameLogType item = session.Get<GameLogType>(i.Value.id);
                        if (item == null)
                        {
                            session.Save(i.Value, i.Value.id);
                        }
                        else
                        {
                            item.id = i.Value.id;
                            item.text = i.Value.text;
                            item.title = i.Value.title;
                            item.description = i.Value.description;
                            session.Update(item);
                        }


                    }

                    transaction.Commit();
                    Logging.Write(Logging.Type.GENERAL, "Updated LogTypes, took: " + watch.ElapsedMilliseconds + "ms");
                    watch.Stop();
                }
            }


        }


        public void Add(GameLogType type, int attackerId, int defenderId, string attackerType, string defenderType)
        {
            GameLog record = new GameLog();
            record.game = game;
            record.gameLogType = type;
            record.obj1id = attackerId;
            record.obj2id = defenderId;
            record.obj1type = attackerType;
            record.obj2type = defenderType;

            records.Add(record);
        }


        public void Publish()
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    foreach (var i in new List<GameLog>(records))
                    {
                        i.game.id = gameRoom.gameId;
                        session.Save(i);
                    }
                    transaction.Commit();
                }
            }




        }

    }
}
