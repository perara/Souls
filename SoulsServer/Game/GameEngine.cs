using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Souls.Server.Engine;
using Souls.Server.Game;
using Souls.Server.Tools;
using Newtonsoft.Json;
using Souls.Server.Objects;
using Newtonsoft.Json.Linq;
using NHibernate.Linq;
using Souls.Server.Network;
using Souls.Model.Helpers;
using Souls.Server.Chat;
using SoulsServer.Objects.Abilities;


namespace Souls.Server.Game
{

    public class GameEngine
    {
        public int gameCounter = 0;

        public static Dictionary<int, GameRoom> rooms { get; set; }
        public static List<Card> cards { get; set; }
        public static List<String> botNames { get; set; }

        public GameEngine()
        {
            SetupEngine();

            // Start Game Queue;
            QueueLoop();
        }


        public void SetupEngine()
        {
            GameEngine.rooms = new Dictionary<int, GameRoom>();
            GameLogger.GenerateLogTypes();
            GameEngine.cards = LoadCards();
            GameEngine.botNames = LoadBotNames();

        }

        public List<String> LoadBotNames()
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            List<String> botNames;
            using (var session = NHibernateHelper.OpenSession())
            {

                botNames = session.Query<Souls.Model.BotNames>()
                .Select(x => x.name)
                .ToList<String>();

            }
            w.Stop();
            Logging.Write(Logging.Type.GAME, "Loaded " + botNames.Count() + " bot names, Took: " + w.ElapsedMilliseconds);
            return botNames;
        }

        public List<Card> LoadCards()
        {

            Stopwatch w = new Stopwatch();
            w.Start();

            NHibernateHelper.OpenSession();
            using (var session = NHibernateHelper.OpenSession())
            {

                // Fetch the Card (ModelObject)
                var dbCards = session.Query<Souls.Model.Card>()
                    .Fetch(x => x.ability)
                    .Fetch(x => x.race)
                    .ToList<Souls.Model.Card>();

                // Create cards from the Database Model
                List<Card> cards = new List<Card>();
                dbCards.ForEach(x => cards.Add(new Card()
                {
                    id = x.id,
                    ability = x.ability,
                    race = x.race,
                    name = x.name,
                    attack = x.attack,
                    health = x.health,
                    armor = x.armor,
                    cost = x.cost,
                }));

                Logging.Write(Logging.Type.GAME, "Loaded " + cards.Count() + " cards, Took: " + w.ElapsedMilliseconds);
                w.Stop();

                return cards;
            }
        }

        /// <summary>
        /// A threaded queue loop which calls callback @see Callback_CreateGame when two or more players are in the queue
        /// </summary>
        public void QueueLoop()
        {
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    bool matchMaked = GameQueue.GetInstance().MatchPlayersNormal(Callback_CreateGame);
                    bool matchMaked2 = GameQueue.GetInstance().MatchPlayersPractice(Callback_CreateGame);
                    Console.WriteLine("\t\t\t\t\t\t\t\tQueue: " + GameQueue.GetInstance().normalQueue.Count());

                    Thread.Sleep(5000);
                }
            });

            pollThread.Start();
        }


        /// <summary>
        /// Init a new Game on a pair of Players
        /// </summary>
        /// <param name="players">Pair of players</param>
        public void Callback_CreateGame(Pair<Player> players)
        {

            players.First.ConstructGamePlayer(true);
            players.Second.ConstructGamePlayer(false);

            // Create the GameRoom
            GameRoom newRoom = new GameRoom();
            newRoom.AddGamePlayers(players);

            players.First.gPlayer.gameRoom = newRoom;
            players.Second.gPlayer.gameRoom = newRoom;

            // Generate and send a response
            Pair<Response> response = newRoom.GenerateGameUpdate(true);
            players.First.gameContext.SendTo(response.First);
            players.Second.gameContext.SendTo(response.Second);

            newRoom.logger.Add(
                   GameLogger.logTypes[GameLogger.LogTypes.GAME_CREATED],
                   players.First.id,
                   players.Second.id,
                   "Player",
                   "Player"
                   );
        }




        public void Request_MoveCard(Player player)
        {
            JObject retData = new JObject(
                new JProperty("cid", player.gameContext.payload["cid"]),
                new JProperty("x", player.gameContext.payload["x"]),
                new JProperty("y", player.gameContext.payload["y"])
            );

            Response response = new Response(
                GameService.GameResponseType.GAME_OPPONENT_MOVE,
                retData
            );

            player.GetOpponent().gameContext.SendTo(response);

        }

        public void Request_OpponentReleaseCard(Player player)
        {

            JObject retData = new JObject(
                new JProperty("cid", player.gameContext.payload["cid"])
            );

            Response response = new Response(
                GameService.GameResponseType.GAME_RELEASE,
                retData
            );


            player.gameContext.SendTo(response);

            response.Type = GameService.GameResponseType.GAME_OPPONENT_RELEASE;
            player.GetOpponent().gameContext.SendTo(response);



        }

        public void Request_Queue(Player player, bool normal = true)
        {

            // Check if the player already have a game player
            if (player.gPlayer == null)
            {

                if (normal)
                {

                    if (!player.inQueue)
                    {
                        GameQueue.GetInstance().AddPlayerNormal(player);
                        Logging.Write(Logging.Type.GAMEQUEUE, player.name + " queued!");
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
                    }
                    else
                    {
                        Logging.Write(Logging.Type.GAMEQUEUE, player.name + " tried to queue twice!");
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
                    }

                }
                else
                {

                    if (!player.inQueue)
                    {
                        GameQueue.GetInstance().AddPlayerPractice(player);
                        Logging.Write(Logging.Type.GAMEQUEUE, player.name + " queued!");
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
                    }
                    else
                    {
                        Logging.Write(Logging.Type.GAMEQUEUE, player.name + " tried to queue twice!");
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
                    }


                }

            }
            else
            {
                Logging.Write(Logging.Type.GAME, "Player was already in game, giving gameUpdate (Create)");

                // Send the gamestate to the player (As create since its the first state of this override player)
                Pair<Response> response = player.gPlayer.gameRoom.GenerateGameUpdate();
                if (player.gPlayer.isPlayerOne)
                {
                    player.gameContext.SendTo(response.First);
                }
                else
                {
                    player.gameContext.SendTo(response.Second);
                }

            }
        }


        public void Request_UseCard(Player player)
        {
            int slot = int.Parse(player.gameContext.payload["slotId"].ToString()); //  This is the slot which is the cards destination
            int card = int.Parse(player.gameContext.payload["cid"].ToString()); // This is the card which the player has on hand

            GamePlayer requestPlayer = player.gPlayer;
            Card c;

            //////////////////////////////////////////////////////////////////////////
            // Check if card exists
            //////////////////////////////////////////////////////////////////////////
            if (!requestPlayer.handCards.TryGetValue(card, out c))
            {
                Logging.Write(Logging.Type.GAME, "Card does not exist!");
                return;
            }


            //////////////////////////////////////////////////////////////////////////
            // Check if player's turn
            //////////////////////////////////////////////////////////////////////////
            if (!requestPlayer.IsPlayerTurn())
            {

                player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                    new JObject(
                        new JProperty("card", card),
                        new JProperty("error", "Not your turn!")
                    )
                ));

                // Fire Card release, (Recalls to original position)
                this.Request_OpponentReleaseCard(player);

                Logging.Write(Logging.Type.GAME, "Not " + player.name + "'s turn!");
                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // Check if card slot is occupied
            //////////////////////////////////////////////////////////////////////////
            if (requestPlayer.boardCards.ContainsKey(slot))
            {
                // Fire Card release, (Back to original hand position)
                this.Request_OpponentReleaseCard(player);

                // Send Response
                player.gameContext.SendTo(
                new Response(GameService.GameResponseType.GAME_USECARD_OCCUPIED,
                    new JObject(
                        new JProperty("slot", slot),
                        new JProperty("message", "Slot is already occupied!")
                    )
                ));

                Logging.Write(Logging.Type.GAME, player.name + " tried to use occupied slot (" + slot + ")");
                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // Check if enough mana
            //////////////////////////////////////////////////////////////////////////
            if (!requestPlayer.HasEnoughMana(c))
            {
                player.gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_USECARD_OOM, "Not enough mana!")
                );

                Logging.Write(Logging.Type.GAME, player.name + " has not enough mana to use " + c.name);

                // Fire releaseCard (to recall the card to origin pos)
                this.Request_OpponentReleaseCard(player);

                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // No constricts was met, run USE_CARD code
            //////////////////////////////////////////////////////////////////////////

            // Move card to Board
            requestPlayer.handCards.Remove(card);
            requestPlayer.boardCards.Add(slot, c);

            // Subtract mana from player
            requestPlayer.mana -= c.cost;

            // Set the slotId on the card.
            c.slotId = slot;

            // Send Response
            Response response = new Response(GameService.GameResponseType.GAME_USECARD_OK,
                new JObject(
                    new JProperty("card", JObject.FromObject(c)),
                    new JProperty("pInfo", JObject.FromObject(requestPlayer.GetPlayerData()))
                )
            );

            player.gameContext.SendTo(response);

            response.Type = GameService.GameResponseType.GAME_OPPONENT_USECARD_OK;
            player.GetOpponent().gameContext.SendTo(response);

            requestPlayer.gameRoom.logger.Add(
               GameLogger.logTypes[GameLogger.LogTypes.USE_CARD],
               player.id,
               c.id,
               "Player",
               "Card"
               );

            Logging.Write(Logging.Type.GAME, player.name + " used " + c.name);

        }

        public void Request_NextTurn(Player player)
        {
            Logging.Write(Logging.Type.GAME, player.name + " initiated next turn");

            GamePlayer requestPlayer = player.gPlayer;

            // Validate player turn
            if (!requestPlayer.IsPlayerTurn()) return;

            requestPlayer.gameRoom.logger.Add(
            GameLogger.logTypes[GameLogger.LogTypes.NEXT_TURN],
            player.id,
            player.GetOpponent().id,
            "Player",
            "Player"
            );


            // Run next round
            requestPlayer.gameRoom.NextTurn();

            // Give a new card to the Opponent
            this.Request_NewCard(player.GetOpponent());

            // Send response
            player.gameContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(
                    new JProperty("yourTurn", false),
                    new JProperty("playerInfo", JObject.FromObject(requestPlayer.GetPlayerData())),
                    new JProperty("opponentInfo", JObject.FromObject(player.GetOpponent().gPlayer.GetPlayerData()))
                 )
            ));

            player.GetOpponent().gameContext.SendTo(new Response(
                GameService.GameResponseType.GAME_NEXT_TURN,
                new JObject(
                    new JProperty("yourTurn", true),
                    new JProperty("playerInfo", JObject.FromObject(player.GetOpponent().gPlayer.GetPlayerData())),
                    new JProperty("opponentInfo", JObject.FromObject(requestPlayer.GetPlayerData()))
                 )
            ));


        }

        public void Request_Attack(Player player)
        {

            int source = int.Parse(player.gameContext.payload["source"].ToString());
            int target = int.Parse(player.gameContext.payload["target"].ToString());
            int type = int.Parse(player.gameContext.payload["type"].ToString());

            // Fetch GamePlayers
            GamePlayer requestPlayer = player.gPlayer;
            GamePlayer opponent = player.GetOpponent().gPlayer;

            //////////////////////////////////////////////////////////////////////////
            // Check players turn
            //////////////////////////////////////////////////////////////////////////
            if (!requestPlayer.IsPlayerTurn())
            {
                // Send a error message, that its not players turn
                player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                new JObject(
                    new JProperty("error", "Not your turn!")
                    )));

                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // Everything OK, do attack
            //////////////////////////////////////////////////////////////////////////
            if (type == 0) // Card on Card
            {
                // Fetch cards from the CID's
                Card sourceCard = requestPlayer.boardCards.FirstOrDefault(x => x.Value.cid == source).Value;
                Card targetCard = opponent.boardCards.FirstOrDefault(x => x.Value.cid == target).Value;
                if (sourceCard == null || targetCard == null) return; // Client sends faulty data, ignore.


                // Ignore if one of the card did not exist
                if (sourceCard == null || targetCard == null) return;

                // Ensure that the entity has not attacked this round
                if (sourceCard.hasAttacked)
                {
                    this.CannotAttackTwice(player);
                    return;
                }


                sourceCard.Attack(targetCard);

                // Check if attackers card is dead
                if (sourceCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + sourceCard.cid + " died.");
                    // Remove the card
                    requestPlayer.RemoveBoardCard(sourceCard);
                }

                // Check if defenders card is dead
                if (targetCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + targetCard.cid + " died.");
                    // Remove the card
                    opponent.RemoveBoardCard(targetCard);
                }

                // requesters Card
                JObject reqObj = new JObject(
                            new JProperty("cid", sourceCard.cid),
                            new JProperty("dmgTaken", targetCard.attack),
                            new JProperty("dmgDone", sourceCard.attack),
                            new JProperty("health", sourceCard.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", sourceCard.isDead));

                // Requesters Opponent's Card
                JObject oppObj = new JObject(
                            new JProperty("cid", targetCard.cid),
                            new JProperty("dmgTaken", sourceCard.attack),
                            new JProperty("dmgDone", targetCard.attack),
                            new JProperty("health", targetCard.health),
                            new JProperty("attacker", false),
                            new JProperty("isDead", targetCard.isDead));

                // Send Response to Requester
                player.gameContext.SendTo(
                     new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                         new JProperty("player", reqObj),
                         new JProperty("opponent", oppObj),
                         new JProperty("type", type)
                         )));

                // Send Response to Opponent
                player.GetOpponent().gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", oppObj),
                        new JProperty("opponent", reqObj),
                        new JProperty("type", type)
                        )));

                // Log the event
                player.gPlayer.gameRoom.logger.Add(
                    GameLogger.logTypes[GameLogger.LogTypes.CARD_ATTACK_CARD],
                    sourceCard.id,
                    targetCard.id,
                    "Card",
                    "Card"
                    );


            }
            else if (type == 1) // Card on Hero
            {
                // Do attack
                Card sourceCard = requestPlayer.boardCards.FirstOrDefault(x => x.Value.cid == source).Value;
                if (sourceCard == null) return; // Client sends faulty data, ignore.
                
                // Ensure that the entity has not attacked this round
                if (sourceCard.hasAttacked)
                {
                    this.CannotAttackTwice(player);
                    return;
                }

                sourceCard.Attack(opponent);

                if (sourceCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + sourceCard.cid + " died.");
                    // Remove the card
                    requestPlayer.RemoveBoardCard(sourceCard);
                }

                // Players Card
                JObject reqObj = new JObject(
                            new JProperty("cid", sourceCard.cid),
                            new JProperty("dmgTaken", opponent.attack),
                            new JProperty("dmgDone", sourceCard.attack),
                            new JProperty("health", sourceCard.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", sourceCard.isDead));

                // Opponent Hero
                JObject oppObj = new JObject(
                            new JProperty("dmgTaken", sourceCard.attack),
                            new JProperty("dmgDone", opponent.attack),
                            new JProperty("health", opponent.health),
                            new JProperty("attacker", false),
                            new JProperty("isDead", opponent.isDead));

                // Send Response to Requester
                player.gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", reqObj),
                        new JProperty("opponent", oppObj),
                        new JProperty("type", type)
                        )));

                // Send Response to Opponent
                player.GetOpponent().gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", oppObj),
                        new JProperty("opponent", reqObj),
                        new JProperty("type", type)
                        )));


                // Log the event
                player.gPlayer.gameRoom.logger.Add(
                    GameLogger.logTypes[GameLogger.LogTypes.CARD_ATTACK_PLAYER],
                    sourceCard.id,
                    player.GetOpponent().id,
                    "Card",
                    "Player"
                    );


            }
            else if (type == 2) // Player on Card
            {
                Card targetCard = opponent.boardCards.FirstOrDefault(x => x.Value.cid == target).Value;
                if (targetCard == null) return; // Client sends faulty data, ignore.

                // Ensure that the entity has not attacked this round
                if (requestPlayer.hasAttacked)
                {
                    this.CannotAttackTwice(player);
                    return;
                }

                requestPlayer.Attack(targetCard);


                if (targetCard.isDead)
                {
                    Logging.Write(Logging.Type.GAME, "Card: " + targetCard.cid + " died.");
                    // Remove the card
                    opponent.RemoveBoardCard(targetCard);
                }

                // Player's Response
                JObject playerObj = new JObject(
                            new JProperty("dmgTaken", targetCard.attack),
                            new JProperty("dmgDone", requestPlayer.attack),
                            new JProperty("health", requestPlayer.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", requestPlayer.isDead));

                // Opponent's Response
                JObject oppObj = new JObject(
                            new JProperty("cid", targetCard.cid),
                            new JProperty("dmgTaken", requestPlayer.attack),
                            new JProperty("dmgDone", targetCard.attack),
                            new JProperty("health", targetCard.health),
                            new JProperty("attacker", false),
                            new JProperty("isDead", targetCard.isDead));

                // Send Response to Player
                player.gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", playerObj),
                        new JProperty("opponent", oppObj),
                        new JProperty("type", type)
                        )));

                // Send Response to Opponent
                player.GetOpponent().gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", oppObj),
                        new JProperty("opponent", playerObj),
                        new JProperty("type", type)
                        )));

                // Log the event
                player.gPlayer.gameRoom.logger.Add(
                    GameLogger.logTypes[GameLogger.LogTypes.PLAYER_ATTACK_CARD],
                    player.id,
                    targetCard.id,
                    "Player",
                    "Player"
                    );

            }
            else if (type == 3) // Player on Opponent
            {

                // Ensure that the entity has not attacked this round
                if (requestPlayer.hasAttacked)
                {
                    this.CannotAttackTwice(player);
                    return;
                }



                requestPlayer.Attack(opponent);

                // Player's Response
                JObject playerObj = new JObject(
                            new JProperty("dmgTaken", opponent.attack),
                            new JProperty("dmgDone", requestPlayer.attack),
                            new JProperty("health", requestPlayer.health),
                            new JProperty("attacker", true),
                            new JProperty("isDead", requestPlayer.isDead));

                // Opponent's Response
                JObject oppObj = new JObject(
                            new JProperty("dmgTaken", requestPlayer.attack),
                            new JProperty("dmgDone", opponent.attack),
                            new JProperty("health", opponent.health),
                            new JProperty("attacker", false),
                            new JProperty("isDead", opponent.isDead));

                // Send Response to Player
                player.gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", playerObj),
                        new JProperty("opponent", oppObj),
                        new JProperty("type", type)
                        )));

                // Send Response to Opponent
                player.GetOpponent().gameContext.SendTo(
                    new Response(GameService.GameResponseType.GAME_ATTACK, new JObject(
                        new JProperty("player", oppObj),
                        new JProperty("opponent", playerObj),
                        new JProperty("type", type)
                        )));

                // Log the event
                player.gPlayer.gameRoom.logger.Add(
                    GameLogger.logTypes[GameLogger.LogTypes.PLAYER_ATTACK_PLAYER],
                    player.id,
                    player.GetOpponent().id,
                    "Player",
                    "Card"
                    );

            }

            //////////////////////////////////////////////////////////////////////////
            // Check winner
            //////////////////////////////////////////////////////////////////////////
            if (requestPlayer.isDead || opponent.isDead)
            {
                if (requestPlayer.isDead) requestPlayer.gameRoom.winner = player.GetOpponent();
                if (opponent.isDead) requestPlayer.gameRoom.winner = player;


                bool isDraw = (requestPlayer.isDead && opponent.isDead);


                player.gPlayer.gameRoom.EndGame(isDraw);
            }



        }

        /// <summary>
        /// Requests a ability usage
        /// </summary>
        /// <param name="player">The Request Player</param>
        public void Request_UseAbility(Player player)
        {

            //////////////////////////////////////////////////////////////////////////
            // Check players turn
            //////////////////////////////////////////////////////////////////////////
            if (!player.gPlayer.IsPlayerTurn())
            {
                // Send a error message, that its not players turn
                player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                new JObject(
                    new JProperty("error", "Not your turn!")
                    )));

                return;
            }

            // Card on Card = 0
            // Card on Player = 1 (Friendly Player)
            // Card on Opponent = 2 (Opponent)
            int source = int.Parse(player.gameContext.payload["source"].ToString());
            int target = int.Parse(player.gameContext.payload["target"].ToString());
            int type = int.Parse(player.gameContext.payload["type"].ToString());
            int abilityId = int.Parse(player.gameContext.payload["abilityId"].ToString());

            Heal heal = new Heal(player);
            Sacrifice sacrifice = new Sacrifice(player);
            Consume consume = new Consume(player);

            Card card = player.gPlayer.boardCards.Where(x => x.Value.cid == source).FirstOrDefault().Value;



            // Ensure that the entity has not attacked this round
            if (card.hasAttacked)
            {
                this.CannotAttackTwice(player);
                return;
            }



            if (abilityId == 1) // Heal
            {

                if (type == 0)
                {
                    Card tar = player.gPlayer.boardCards.Where(x => x.Value.cid == target).FirstOrDefault().Value;

                    if (tar == null)
                    {
                        // Error, cannot heal Opponent
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot heal opponent!"));
                        return;
                    }

                    heal.Use(card, tar);
                }

                else if (type == 1)
                {
                    heal.Use(card, player);


                }
                else if (type == 2)
                {
                    // Error, Cannot heal Opponent
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot heal opponent!"));
                    return;
                }


            }
            else if (abilityId == 2) // Sacrifice - Sacrifices "this" and gives stats to "other"
            {

                if (type == 0)
                {
                    Card tar = player.gPlayer.boardCards.Where(x => x.Value.cid == target).FirstOrDefault().Value;
                    if (tar == null)
                    {
                        // Error, cannot heal Opponent
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Can only target friendly!"));
                        return;
                    }

                    if (card.Equals(tar))
                    {
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot cast on itself!"));
                        return;
                    }


                    sacrifice.Use(card, tar);
                }

                else if (type == 1)
                {
                    sacrifice.Use(card, player);
                }
                else if (type == 2)
                {
                    // Error, Cannot heal Opponent
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Can only target friendly!"));
                    return;
                }

            }
            else if (abilityId == 3) // Consume 
            {


                if (type == 0)
                {
                    // Only one of these should be set
                    Card tar1 = player.gPlayer.boardCards.Where(x => x.Value.cid == target).FirstOrDefault().Value;
                    Card tar2 = player.gPlayer.boardCards.Where(x => x.Value.cid == target).FirstOrDefault().Value;

                    if (tar1 != null && tar2 != null)
                    {
                        player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot consume two cards"));
                        return;
                    }

                    if (tar1 != null)
                        consume.Use(card, tar1);

                    if (tar2 != null)
                        consume.Use(card, tar2);

                }

                else if (type == 1 || type == 2) // Should not be able to do it on heroes
                {
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot use on hero!"));
                    return;
                }


            }
            else
            {
                // NO ability
            }










            Console.WriteLine("--------------");
            Console.WriteLine(source);
            Console.WriteLine(target);
            Console.WriteLine(type);
            Console.WriteLine(abilityId);


        }


        /// <summary>
        ///  Sends a new card to the specified GamePlayer
        /// </summary>
        /// <param name="player">The game player</param>
        public void Request_NewCard(Player player, int num = 1)
        {
            // Do not allow more than 10 Cards
            if (player.gPlayer.handCards.Count() >= 10) return;

            // Create the new card
            List<Card> newCard = player.gPlayer.AddCard(player, num);

            player.gPlayer.gameRoom.logger.Add(
           GameLogger.logTypes[GameLogger.LogTypes.NEW_CARD],
           player.id,
           newCard[0].id,
           "Player",
           "Card"
           );

            player.gPlayer.AddCardToHand(newCard);

            // Create a response with the new card
            Response ret = new Response(GameService.GameResponseType.GAME_NEWCARD, new JObject(
                new JProperty("card", JArray.FromObject(newCard))
                ));

            // Send to the player
            player.gameContext.SendTo(ret);


            // Create a response with the CID to the opponent
            Response retOpponent = new Response(GameService.GameResponseType.GAME_OPPONENT_NEWCARD, new JObject(
              new JProperty("card", from h in newCard
                                    select
                                        new JObject(
                                             new JObject(
                                              new JProperty("cid", h.cid))
                                 ))
               ));

            // Send to the opponent
            player.GetOpponent().gameContext.SendTo(retOpponent);


        }




        public void CannotAttackTwice(Player p)
        {
            p.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_GENERAL_MESSAGE, "Cannot do that yet!"));
        }


    }

}

