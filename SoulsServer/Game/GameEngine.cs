﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoulsServer.Engine;
using SoulsServer.Game;
using SoulsServer.Tools;
using Newtonsoft.Json;
using SoulsServer.Controller;
using Newtonsoft.Json.Linq;

namespace SoulsServer
{

    public class GameEngine
    {
        public int gameCounter = 0;
        //public static Dictionary<int, GameRoom> gameRooms = new Dictionary<int, GameRoom>();
        public static List<Card> cards { get; set; }

        public GameEngine()
        {

            SetupEngine();

            // Start Game Queue;
            pollQueue();
        }

        // Checks the queue for players, sorts them after rank and matchmake them
        public void pollQueue()
        {
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    bool matchMaked = GameQueue.GetInstance().matchPlayers(Request_CreateGame);
                    // Console.WriteLine("\t\t\t\t\t\t\t\tQueue: " + GameQueue.GetInstance().queue.Count());

                    Thread.Sleep(500);
                }
            });

            pollThread.Start();
        }

        public void SetupEngine()
        {
            GameEngine.cards = LoadCards();

        }

        public List<Card> LoadCards()
        {
            using (var db = new Model.soulsEntities())
            {
                Stopwatch w = new Stopwatch();
                w.Start();
                List<Card> cards = db.db_Card
                       .Join(
                       db.db_Ability,
                       card => card.fk_ability,
                       ability => ability.id,
                       (card, ability) => new { card, ability }
                       )
                       .Join(
                       db.db_Card_Type,
                       y => y.card.fk_type,
                       cType => cType.id,
                       (y, cType) => new { y, cType }
                      ).Select(x => new Card()
                      {
                          id = x.y.card.id,
                          name = x.y.card.name,
                          attack = x.y.card.attack,
                          health = x.y.card.health,
                          armor = x.y.card.armor,
                          cost = x.y.card.cost,
                          ability = new Ability()
                          {
                              id = x.y.ability.id,
                              name = x.y.ability.name,
                              parameter = x.y.ability.parameter
                          },
                          cardType = new CardType()
                          {
                              id = x.y.ability.id,
                              name = x.y.ability.name,
                          },
                      }).AsParallel().ToList();


                Console.WriteLine(">[GAME] Loaded " + cards.Count() + " cards, Took: " + w.ElapsedMilliseconds);
                w.Stop();

                return cards;
            }

        }

        public void Request_MoveCard(Player player)
        {


            GamePlayer requestPlayer = player.gPlayer;

            JObject retData = new JObject(
                new JProperty("cid", player.gameContext.data.Payload.cid),
                new JProperty("x", player.gameContext.data.Payload.x),
                new JProperty("y", player.gameContext.data.Payload.y)
                );



            Response response = new Response(
                GameService.GameResponseType.GAME_OPPONENT_MOVE,
                retData
                );


            requestPlayer.GetOpponent().playerContext.SendTo(response);



        }

        public void Request_ReleaseCard(Player player)
        {
            GamePlayer requestPlayer = player.gPlayer;

            JObject retData = new JObject(
             new JProperty("cid", player.gameContext.data.Payload.cid));

            Response response = new Response(
                GameService.GameResponseType.GAME_PLAYER_RELEASE,
                retData
                );


            requestPlayer.playerContext.SendTo(response);

            response.Type = GameService.GameResponseType.GAME_OPPONENT_RELEASE;
            requestPlayer.GetOpponent().playerContext.SendTo(response);



        }

        // Starts the game from the matchmaked players in a new gameroom
        public void Request_CreateGame(Pair<Player> players)
        {

            GameRoom newRoom = new GameRoom();

            GamePlayer p1 = new GamePlayer(players.First.gameContext)
            {
                hash = players.First.hash,
                name = players.First.name,
                rank = players.First.rank,
                isPlayerOne = true,
                gameRoom = newRoom,
            };

            GamePlayer p2 = new GamePlayer(players.Second.gameContext)
            {
                hash = players.Second.hash,
                name = players.Second.name,
                rank = players.Second.rank,
                isPlayerOne = false,
                gameRoom = newRoom,
            };

            // Add GamePlayer objects to the Player contexts
            players.First.gPlayer = p1;
            players.Second.gPlayer = p2;


            Pair<GamePlayer> playerPair = new Pair<GamePlayer>(p1, p2);

            // Create a game room
            newRoom.AddGamePlayers(playerPair);


            Pair<Response> response = newRoom.GenerateGameUpdate(true);
            players.First.gameContext.SendTo(response.First);
            players.Second.gameContext.SendTo(response.Second);


            // Send "Its your turn to the start player"
            newRoom.currentPlaying.playerContext.SendDebug("Its your turn (DEBUG)");

        }

        public void Request_QueuePlayer(Player player)
        {

            // Check if the player already have a game player
            if (player.gPlayer == null)
            {

                if (!player.inQueue)
                {
                    GameQueue.GetInstance().AddPlayer(player);
                    Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " queued!");
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_OK, "You are now in queue!"));
                }
                else
                {
                    Console.WriteLine("\t\t\t\t\t\t\t" + player.name + " tried to queue twice!");
                    player.gameContext.SendTo(new Response(GameService.GameResponseType.QUEUE_ALREADY_IN, "You are already in queue!")); //todo better response type
                }

            }
            else
            {
                Console.WriteLine("> [GAME] Player was already in game, giving gameUpdate (Create)");

                // Send the gamestate to the player (As create since its the first state of this override player)
                Pair<Response> response = player.gPlayer.gameRoom.GenerateGameUpdate(true);
                if (player.gPlayer.isPlayerOne)
                {
                    response.First.Type = GameService.GameResponseType.GAME_RECOVER;
                    player.gPlayer.playerContext.SendTo(response.First);
                }
                else
                {
                    response.Second.Type = GameService.GameResponseType.GAME_RECOVER;
                    player.gPlayer.playerContext.SendTo(response.Second);
                }



            }
        }


        public void Request_UseCard(Player player)
        {
            int slot = player.gameContext.data.Payload.slotId; //  This is the slot which is the cards destination
            int card = player.gameContext.data.Payload.cid; // This is the card which the player has on hand


            GamePlayer requestPlayer = player.gPlayer;

            // If opposite players turn
            if (!requestPlayer.IsPlayerTurn())
            {
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_NOT_YOUR_TURN,
                new Dictionary<string, object> 
                        { 
                            {"card",card},
                            {"error","Not your turn!"} 
                        }));
                Console.WriteLine(">[GAME] Not your turn!");
                return;
            }

            // If cardslot is not empty
            else if (requestPlayer.boardCards[slot] != null)
            {
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_USECARD_OCCUPIED, "Not enough mana!"));
                this.Request_ReleaseCard(player);
                Console.WriteLine(">[GAME] Slot occupied!");
                return;
            }

            // Not enough mana to use card
            else if (!requestPlayer.HasEnoughMana(card))
            {
                requestPlayer.playerContext.SendTo(new Response(GameService.GameResponseType.GAME_USECARD_OOM, "Not enough mana!"));
                Console.WriteLine(">[GAME] Not enough mana!");
                return;
            }

            // Use card in defined slot
            else
            {
                // Move a card to the board
                Card c;
                requestPlayer.handCards.TryGetValue(card, out c);
                requestPlayer.handCards.Remove(card);
                requestPlayer.boardCards.Add(c.cid, c);


                //Next turn
                requestPlayer.gameRoom.NextTurn();

                // Send Reply
                Response response = new Response(GameService.GameResponseType.GAME_USECARD_PLAYER_OK, new Dictionary<string, int> 
                        { 
                            {"cid",card},
                            {"slotId",slot} 
                        });

                requestPlayer.playerContext.SendTo(response);
                response.Type = GameService.GameResponseType.GAME_USECARD_OPPONENT_OK;
                requestPlayer.GetOpponent().playerContext.SendTo(response);
                Console.WriteLine(">[GAME] Sent!");
            }
        }

        public void Request_NextRound(Player player)
        {
            GamePlayer requestPlayer = player.gPlayer;

            // Validate player turn
            if (!requestPlayer.IsPlayerTurn()) return;

            // Run next round
            requestPlayer.gameRoom.NextRound();

            // Send back game state (update)

            Pair<Response> response = requestPlayer.gameRoom.GenerateGameUpdate();
            requestPlayer.GetOpponent().playerContext.SendTo(response.First);
            requestPlayer.playerContext.SendTo(response.Second);

        }


        public void Request_CardAttack(Player player)
        {
            int attacker = player.gameContext.data.attacker;
            int defender = player.gameContext.data.defender;

            bool cardAttackPlayer = player.gameContext.data.cardAttackPlayer; //NB MIGHT BE NULL IF NOT USED
            bool playerAttackCard = player.gameContext.data.playerAttackCard; //NB MIGHT BE NULL IF NOT USED



            GamePlayer requestPlayer = player.gPlayer;

            if (!requestPlayer.IsPlayerTurn())
            {
                return;
            }


            GamePlayer opponent = requestPlayer.GetOpponent();
            if (opponent != null)
            {
                // Attack Card and shaise, must authenticate the Move
                Card atkCard = requestPlayer.handCards[attacker];
                Card defCard = opponent.handCards[defender];


                if (cardAttackPlayer)
                {
                    atkCard.Attack(ref opponent); // Card on Player attack
                }
                else if (playerAttackCard)
                {
                    requestPlayer.Attack(defCard); // Player on Card attack
                }
                else
                {
                    atkCard.Attack(ref defCard); // Card on Card
                }

                // Send back game update
                Pair<Response> response = requestPlayer.gameRoom.GenerateGameUpdate();
                opponent.playerContext.SendTo(response.First);
                requestPlayer.playerContext.SendTo(response.Second);

            }
            else
            {
                player.gameContext.SendTo(new Response(GameService.GameResponseType.GAME_OPPONENT_NOEXIST, "Opponent does not exist!"));
            }
        }

    }
}
