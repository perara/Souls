using Newtonsoft.Json.Linq;
using Souls.Server.Network;
using Souls.Server.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Threading;
using Souls.Server.Game;
using System.Collections;
using System.Collections.Concurrent;

namespace SoulsServer.Objects
{
    public class AI
    {
        private WebSocket ws { get; set; }
        private WebSocket wschat { get; set; }

        public int gameId { get; set; }
        public int round { get; set; }
        public int ident { get; set; }
        public bool yourTurn { get; set; }

        public Bot p { get; set; }
        public Bot opp { get; set; }


        public AI()
        {
        }


        public void Connect(string hash = "BOT")
        {
            ws = new WebSocket("ws://localhost:8140/game");

            ws.OnMessage += (sender, e) =>
                this.Progress(e);
            ws.Connect();

            wschat = new WebSocket("ws://localhost:8140/chat");

            wschat.OnMessage += (sender, e) =>
                this.Progress(e);

            wschat.Connect();

            this.SendTo(
               new Response(
                   GameService.SERVICE.LOGIN,
                           new JObject(new JProperty("hash", hash)))
                   );

            this.SendTo(
                new Response(
                    GameService.GameType.QUEUE_PRACTICE,
                    new JObject(
                        new JProperty("Type", 200),
                        new JProperty("Payload",
                            new JObject(
                            new JProperty("hash", hash)))))
                    );

        }

        ~AI()  // destructor
        {
            Console.WriteLine("Destructing BOT: " + p.name);
        }


        public void Progress(MessageEventArgs e)
        {
            /* if (p != null && opp != null)
             {
                 Console.WriteLine("Player Board: " + string.Join(",", p.boardCards.Select(x => new { x.Key })));
                 Console.WriteLine("Player Hand: " + string.Join(",", p.handCards.Select(x => new { x.Key })));
                 Console.WriteLine("--------------------------------------------------------------------------");
                 Console.WriteLine("Opponent Board: " + string.Join(",", opp.boardCards.Select(x => new { x.Key })));
                 Console.WriteLine("Opponent Hand: " + string.Join(",", opp.handCards.Select(x => new { x.Key })));

                 Console.WriteLine("--------------------------------------------------------------------------");
                 Console.WriteLine("--------------------------------------------------------------------------");
                 Console.WriteLine("--------------------------------------------------------------------------");
             }*/
            // Process the JSON
            JObject pl = JObject.Parse(e.Data);
            var payload = pl["Payload"];
            var type = int.Parse(pl.GetValue("Type").ToString());

            //Console.WriteLine("RESPONSE TYPE: " + type);
            //Console.WriteLine(payload.ToString());

            if (type == (int)GameService.GameResponseType.GAME_CREATE) // Create Game
            {
                Request_CreateGame(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_NEXT_TURN) // Next_Turn (Player)
            {
                Request_NextTurn(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_NEWCARD) // Next_Turn (PLAYER)
            {
                Request_NewCard(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_OPPONENT_NEWCARD) // UseCard Opponent
            {
                Request_OppNewCard(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_ATTACK) // Attack
            {
                Request_Attack(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_OPPONENT_USECARD_OK) // Attack
            {
                Request_OppUseCard(payload);
            }
            else if (type == (int)GameService.GameResponseType.GAME_BOT_DISCONNECT) // Attack
            {
                ws.Close(CloseStatusCode.Away);
            }
            else if (type == (int)GameService.GameResponseType.GAME_USE_ABILITY) // Ability
            {

                // TODO not covered completly!
                int abilityType = int.Parse(payload["type"].ToString());
                int ability = int.Parse(payload["abilityId"].ToString());
                int src = int.Parse(payload["source"].ToString());
                int tar = int.Parse(payload["target"].ToString());

                Card c;
                if (opp.boardCards.TryGetValue(src, out c))
                {

                    if (ability == 2)// Sacrifice
                    {


                        Card trash;
                        opp.boardCards.TryRemove(c.cid, out trash);

                        if (abilityType == 2)
                        {
                            opp.attack += c.attack;
                            opp.health += c.health;
                        }
                        if (abilityType == 1)
                        {
                            Card target;
                            if (opp.boardCards.TryGetValue(tar, out target))
                            {
                                target.attack += c.attack;
                                target.health += c.health;


                            }

                        }
                    }

                    if (ability == 1) // Heal
                    {

                        if (abilityType == 1) // Card-->Card
                        {
                            Card target;
                            if (opp.boardCards.TryGetValue(tar, out target))
                            {

                                target.health += c.health;


                            }
                        }
                        else if (abilityType == 2) // CARD--> HERO
                        {

                            opp.health += c.health;
                        }



                    }
                }


            }


            // Process The game
            this.ProcessGame();

            /* if (p != null && opp != null)
             {
                 Console.WriteLine("Player Board: " + string.Join(",", p.boardCards.Select(x => new { x.Key })));
                 Console.WriteLine("Player Hand: " + string.Join(",", p.handCards.Select(x => new { x.Key })));
                 Console.WriteLine("--------------------------------------------------------------------------");
                 Console.WriteLine("Opponent Board: " + string.Join(",", opp.boardCards.Select(x => new { x.Key })));
                 Console.WriteLine("Opponent Hand: " + string.Join(",", opp.handCards.Select(x => new { x.Key })));
             }*/

        }

        public void Request_Ability()
        {

        }



        private void Request_OppUseCard(JToken payload)
        {
            var cardData = payload["card"];
            int cardId = int.Parse(cardData["id"].ToString());
            int cid = int.Parse(cardData["cid"].ToString());
            int slotId = int.Parse(cardData["slotId"].ToString());

            Card c = (Card)GameEngine.cards.Where(x => x.id == cardId).FirstOrDefault().Clone();
            c.cid = cid;
            c.slotId = slotId;

            Card trash;
            this.opp.handCards.TryRemove(cid, out trash);
            this.opp.boardCards.TryAdd(cid, c);

            /************************************************************************/
            /* RESPONSE TYPE: 211
                {
                  "card": {
                    "isDead": false,
                    "cid": 6,
                    "slotId": 2,
                    "hasAttacked": false,
                    "id": 6,
                    "ability": {
                      "id": 1,
                      "name": "Taunt",
                      "parameter": "0"
                    },
                    "race": {
                      "id": 1,
                      "name": "Darkness"
                    },
                    "name": "Bobby",
                    "attack": 2,
                    "health": -5,
                    "armor": 2,
                    "cost": 1,
                    "vendor_price": 0,
                    "level": 0
                  },
                  "pInfo": {
                    "health": "1",
                    "attack": "5",
                    "mana": "2",
                    "name": "per",
                    "type": "1"
                  }
                }                                                                     */
            /************************************************************************/

        }


        private void Request_OppNewCard(JToken payload)
        {
            var cardValues = payload["card"].First();
            int cid = int.Parse(cardValues["cid"].ToString());
            this.opp.handCards.TryAdd(cid, null);


            /************************************************************************/
            /*RESPONSE TYPE: 223   
             * 
            {
                "card": [
                    {
                        "cid": 8
                    }
                 ]
                }
            }
             */
            /************************************************************************/
        }
        private void Request_Attack(JToken payload)
        {
            var playerData = payload["player"];
            var opponentData = payload["opponent"];
            int type = int.Parse(payload["type"].ToString());


            if (type == 0) // Card on Card
            {
                // Player data
                int pCardCid = int.Parse(playerData["cid"].ToString());
                int pCardHealth = int.Parse(playerData["health"].ToString());
                bool pCardIsDead = bool.Parse(playerData["isDead"].ToString());

                // Opponent data
                int oppCardCid = int.Parse(opponentData["cid"].ToString());
                int oppCardHealth = int.Parse(opponentData["health"].ToString());
                bool oppCardIsDead = bool.Parse(opponentData["isDead"].ToString());

                // Get player's source card
                Card pCard;
                p.boardCards.TryGetValue(pCardCid, out pCard);
                if (pCard != null)
                {
                    pCard.health = pCardHealth;

                    if (pCardIsDead)
                    {
                        Card trash;
                        this.p.boardCards.TryRemove(pCard.cid, out trash);
                    }
                }

                // Do stuff for opponent
                Card oppCard;
                opp.boardCards.TryGetValue(oppCardCid, out oppCard);
                if (oppCard != null)
                {
                    oppCard.health = oppCardHealth;
                    if (oppCardIsDead)
                    {
                        Card trash;
                        this.opp.boardCards.TryRemove(oppCard.cid, out trash);
                    }
                }

            }



            // Request
            /************************************************************************/
            /* {
                   "player": {
                     "cid": 2,
                     "dmgTaken": 2,
                     "dmgDone": 2,
                     "health": -7,
                     "attacker": false,
                     "isDead": true
                   },
                   "opponent": {
                     "cid": 12,
                     "dmgTaken": 2,
                     "dmgDone": 2,
                     "health": -7,
                     "attacker": true,
                     "isDead": true
                   },
                   "type": 0
                 }                                                                     */
            /************************************************************************/
        }

        private void Request_NewCard(JToken payload)
        {
            var cardValues = payload["card"].First();
            int cardId = int.Parse(cardValues["id"].ToString());

            Card c = (Card)GameEngine.cards.Where(x => x.id == cardId).FirstOrDefault().Clone();
            c.isDead = bool.Parse(cardValues["isDead"].ToString());
            c.cid = int.Parse(cardValues["cid"].ToString());
            c.slotId = int.Parse(cardValues["slotId"].ToString());
            c.hasAttacked = bool.Parse(cardValues["hasAttacked"].ToString());

            this.p.handCards.TryAdd(c.cid, c);
            // Response
            /************************************************************************/
            /*         {
              "card": [
                {
                  "isDead": false,
                  "cid": 10,
                  "slotId": 0,
                  "hasAttacked": false,
                  "id": 6,
                  "ability": {
                    "id": 1,
                    "name": "Taunt",
                    "parameter": "0"
                  },
                  "race": {
                    "id": 1,
                    "name": "Darkness"
                  },
                  "name": "Bobby",
                  "attack": 2,
                  "health": -5,
                  "armor": 2,
                  "cost": 1,
                  "vendor_price": 0,
                  "level": 0
                }
              ]
            }                                                             */
            /************************************************************************/

        }

        private void Request_NextTurn(JToken payload)
        {



            this.yourTurn = bool.Parse(payload["yourTurn"].ToString());

            p.health = int.Parse(payload["playerInfo"]["health"].ToString());
            p.mana = int.Parse(payload["playerInfo"]["mana"].ToString());

            opp.health = int.Parse(payload["opponentInfo"]["health"].ToString());
            opp.mana = int.Parse(payload["opponentInfo"]["mana"].ToString());

            // TODO Set all parameters
            /************************************************************************/
            /* {
              "yourTurn": false,
              "playerInfo": {
                "health": "1",
                "attack": "5",
                "mana": "0",
                "name": "BOT",
                "type": "1"
              },
              "opponentInfo": {
                "health": "1",
                "attack": "5",
                "mana": "1",
                "name": "per",
                "type": "1"
              }                                                                     */
            /************************************************************************/


        }

        /// <summary>
        /// Create Game Request
        /// </summary>
        public void Request_CreateGame(JToken item)
        {
            /************************************************************************/
            /* {
  "gameId": 2,
  "round": 0,
  "ident": 1,
  "player": {
    "info": {
      "health": "30",
      "attack": "3",
      "mana": "3",
      "name": "paul",
      "type": "4"
    },
    "board": {},
    "hand": {
      "9": {
        "isDead": false,
        "cid": 9,
        "slotId": 0,
        "hasAttacked": false,
        "id": 1,
        "ability": {
          "id": 1,
          "name": "Heal",
          "parameter": "0"
        },
        "race": {
          "id": 4,
          "name": "Ferocious",
          "cardUrl": "/Content/Images/Card/Texture/ferocious.png"
        },
        "name": "Rhino",
        "attack": 5,
        "health": 7,
        "armor": 3,
        "cost": 6,
        "vendor_price": 0,
        "level": 0,
        "portrait": null
      },
      "10": {
        "isDead": false,
        "cid": 10,
        "slotId": 0,
        "hasAttacked": false,
        "id": 5,
        "ability": {
          "id": 1,
          "name": "Heal",
          "parameter": "0"
        },
        "race": {
          "id": 4,
          "name": "Ferocious",
          "cardUrl": "/Content/Images/Card/Texture/ferocious.png"
        },
        "name": "Huy",
        "attack": 3,
        "health": 6,
        "armor": 2,
        "cost": 5,
        "vendor_price": 0,
        "level": 0,
        "portrait": null
      },
      "11": {
        "isDead": false,
        "cid": 11,
        "slotId": 0,
        "hasAttacked": false,
        "id": 2,
        "ability": {
          "id": 2,
          "name": "Sacrifice",
          "parameter": "0"
        },
        "race": {
          "id": 3,
          "name": "Lightbringer",
          "cardUrl": "/Content/Images/Card/Texture/lightbringer.png"
        },
        "name": "Zebra",
        "attack": 1,
        "health": 1,
        "armor": 1,
        "cost": 2,
        "vendor_price": 0,
        "level": 0,
        "portrait": null
      }
    }
  },
  "opponent": {
    "info": {
      "health": "30",
      "attack": "1",
      "mana": "1",
      "name": "[BOT] Lisa",
      "type": "1"
    },
    "board": {},
    "hand": [
      {
        "cid": 12
      },
      {
        "cid": 13
      },
      {
        "cid": 14
      }
    ]
  },
  "create": true,
  "yourTurn": true
}                                                                     */
            /************************************************************************/

            p = new Bot();
            p.attack = int.Parse(item["player"]["info"]["attack"].ToString());
            p.name = item["player"]["info"]["name"].ToString();
            p.mana = int.Parse(item["player"]["info"]["mana"].ToString());
            p.health = int.Parse(item["player"]["info"]["health"].ToString());

            opp = new Bot();
            opp.attack = int.Parse(item["opponent"]["info"]["attack"].ToString());
            opp.name = item["opponent"]["info"]["name"].ToString();
            opp.mana = int.Parse(item["opponent"]["info"]["mana"].ToString());
            opp.health = int.Parse(item["opponent"]["info"]["health"].ToString());

            // General stuff
            yourTurn = bool.Parse(item["yourTurn"].ToString()); ;
            ident = int.Parse(item["ident"].ToString());
            gameId = int.Parse(item["gameId"].ToString());
            round = int.Parse(item["round"].ToString());


            // Populate Player hand
            foreach (var hCard_ in item["player"]["hand"])
            {
                foreach (var hCard__ in hCard_)
                {
                    int cid = int.Parse(hCard__["cid"].ToString());
                    int slotId = int.Parse(hCard__["slotId"].ToString());
                    bool hasAttacked = bool.Parse(hCard__["hasAttacked"].ToString());
                    bool isDead = bool.Parse(hCard__["isDead"].ToString());
                    int id = int.Parse(hCard__["id"].ToString());

                    Card c = (Card)GameEngine.cards.Where(x => x.id == id).FirstOrDefault().Clone();
                    c.isDead = isDead;
                    c.cid = cid;
                    c.slotId = slotId;
                    c.hasAttacked = hasAttacked;

                    this.p.handCards.TryAdd(c.cid, c);
                }
            }

            // Populate Enemy Hand
            foreach (var hCard_ in item["opponent"]["hand"])
            {
                foreach (var hCard__ in hCard_)
                {
                    foreach (var hCard___ in hCard_)
                    {
                        int cid = int.Parse(hCard___.First().ToString());
                        this.opp.handCards.TryAdd(cid, null);
                    }

                }
            }
        }


        public void ProcessGame()
        {
            if (yourTurn)
            {


                //////////////////////////////////////////////////////////////////////////
                // Attempt to Use cards (Use All mana)
                //////////////////////////////////////////////////////////////////////////
                var rnd = new Random((int)DateTime.Now.Ticks); // Random for random list
                foreach (KeyValuePair<int, Card> kvp in p.handCards.OrderBy(x => rnd.Next()))
                {
                    var c = kvp.Value;

                    // Can afford to use the card
                    if (p.mana - c.cost >= 0)
                    {
                        p.mana -= c.cost; //Subtract mana

                        // Loop through all of the slots until a empty slot (Null) is found
                        for (int slot = 0; slot < 7; slot++)
                        {

                            if (p.boardCards.Where(x => x.Value.slotId == slot).FirstOrDefault().Value == null)
                            {
                                this.UseCard(c, slot);
                                break;
                            }

                        }
                    }
                }

                Thread.Sleep(2000);
                //////////////////////////////////////////////////////////////////////////
                // Attack with card
                //////////////////////////////////////////////////////////////////////////
                this.Attack();

                //////////////////////////////////////////////////////////////////////////
                // All states done, give turn
                //////////////////////////////////////////////////////////////////////////
                this.yourTurn = false;
                this.NextTurn();

            }
            else
            {
                // Nothing 
            }
        }

        private void Attack()
        {

            foreach (Card c in p.boardCards.Values)
            {
                bool attackPlayerDeterm = (new Random((int)DateTime.Now.Ticks).Next(0, 200) > 175 || opp.boardCards.Count == 0);


                // Attack a card
                if (!attackPlayerDeterm)
                {
                    int r = new Random((int)DateTime.Now.Ticks).Next(opp.boardCards.Values.Count);

                    Card oppC;
                    try
                    {
                        oppC = opp.boardCards.Values.ElementAt(r);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("NO CARDS!? ");
                        continue;
                    }

                    Card trash;
                    c.Attack(oppC);
                    if (c.isDead)
                        p.boardCards.TryRemove(c.cid, out trash);
                    if (oppC.isDead)
                        opp.boardCards.TryRemove(oppC.cid, out trash);


                    this.SendTo(
                      new Response(
                          GameService.GameType.ATTACK,
                          new JObject(
                              new JProperty("source", c.cid),
                              new JProperty("target", oppC.cid),
                              new JProperty("type", 0 /*TODO*/))
                              )
                    );
                }
                else // Attack Player
                {
                    if (opp.health - c.attack >= 1)
                    {
                        Card trash;
                        c.Attack(opp);
                        if (c.isDead)
                            p.boardCards.TryRemove(c.cid, out trash);



                        this.SendTo(
                         new Response(
                             GameService.GameType.ATTACK,
                             new JObject(
                                 new JProperty("source", c.cid),
                                 new JProperty("target", -1),
                                 new JProperty("type", 1 /*TODO*/))
                                 )
                       );
                    }
                }


                Thread.Sleep(200);



            }

            /* ATTACK: {
                "Type": Messages.prototype.Type.Game.ATTACK,
                "Payload": {
                    "source": undefined,
                    "target": undefined,
                    "type" : -1
                }*/
        }

        public void SendTo(Response response)
        {
            ws.Send(response.ToJSON());
        }

        private void NextTurn()
        {
            this.SendTo(
               new Response(
                   GameService.GameType.NEXT_TURN,
                           new JObject())
                   );
        }

        /// <summary>
        /// Use Card function (Sends to server)
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="slotId"></param>
        private void UseCard(Card c, int slotId)
        {
            Card trash;
            c.slotId = slotId;
            this.p.handCards.TryRemove(c.cid, out trash);
            this.p.boardCards.TryAdd(c.cid, c);

            this.SendTo(
                new Response(
                    GameService.GameType.USECARD,
                            new JObject(
                                new JProperty("cid", c.cid),
                                new JProperty("slotId", slotId)
                                ))
                    );
        }

    }
}
