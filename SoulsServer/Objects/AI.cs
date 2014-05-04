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

namespace SoulsServer.Objects
{
    public class AI
    {
        private WebSocket ws { get; set; }

        public string name { get; set; }

        public List<Card> p_handCards { get; set; }

        public Dictionary<int, Card> p_boardCards { get; set; }

        public List<int> e_handCards { get; set; }

        public Dictionary<int, Card> e_boardCards { get; set; }

        public int gameId { get; set; }
        public int round { get; set; }
        public int ident { get; set; }
        public bool yourTurn { get; set; }





        public AI()
        {
            this.p_handCards = new List<Card>();
            this.p_boardCards = new Dictionary<int, Card>();
            this.e_boardCards = new Dictionary<int, Card>();
            this.e_handCards = new List<int>();

        }


        public void Connect()
        {
            ws = new WebSocket("ws://localhost:8140/game");

            ws.OnMessage += (sender, e) =>
                this.Progress(e);
            ws.Connect();


            this.SendTo(
               new Response(
                   GameService.SERVICE.LOGIN,
                           new JObject(new JProperty("hash", "BOT")))
                   );

            this.SendTo(
                new Response(
                    GameService.GameType.QUEUE,
                    new JObject(
                        new JProperty("Type", 200),
                        new JProperty("Payload",
                            new JObject(
                            new JProperty("hash", "BOT")))))
                    );

        }


        public void Progress(MessageEventArgs e)
        {
            // Process the JSON
            JObject pl = JObject.Parse(e.Data);
            var payload = pl["Payload"];
            var type = int.Parse(pl.GetValue("Type").ToString());

            Console.WriteLine("RESPONSE TYPE: " + type);
            Console.WriteLine(payload.ToString());

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

            this.e_handCards.Remove(cid);
            this.e_boardCards.Add(slotId, c);


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
            this.e_handCards.Add(cid);


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

                // Do Stuff for player
                Card pCard = p_boardCards.Where(x => x.Value.cid == pCardCid).FirstOrDefault().Value;
                if (pCardIsDead)
                {
                    p_boardCards.Remove(pCard.slotId);
                }
                else
                {
                    pCard.health = pCardHealth;
                }

                // Do stuff for opponent
                Card oppCard = e_boardCards.Where(x => x.Value.cid == oppCardCid).FirstOrDefault().Value;
                if (oppCardIsDead)
                {
                    e_boardCards.Remove(oppCard.slotId);
                }
                else
                {
                    oppCard.health = oppCardHealth;
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


            this.p_handCards.Add(c);


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
            this.mana = int.Parse(payload["playerInfo"]["mana"].ToString());

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
            gameId = int.Parse(item["gameId"].ToString());
            round = int.Parse(item["round"].ToString());
            ident = int.Parse(item["ident"].ToString());
            yourTurn = bool.Parse(item["yourTurn"].ToString());
            mana = int.Parse(item["player"]["info"]["mana"].ToString());
            health = int.Parse(item["player"]["info"]["health"].ToString());


            // Populate Player hand
            foreach (var hCard_ in item["player"]["hand"])
            {
                foreach (var hCard__ in hCard_)
                {
                    bool isDead = bool.Parse(hCard__["isDead"].ToString());
                    int cid = int.Parse(hCard__["cid"].ToString());
                    int slotId = int.Parse(hCard__["slotId"].ToString());
                    bool hasAttacked = bool.Parse(hCard__["hasAttacked"].ToString());
                    int id = int.Parse(hCard__["id"].ToString());

                    Card c = (Card)GameEngine.cards.Where(x => x.id == id).FirstOrDefault().Clone();
                    c.isDead = isDead;
                    c.cid = cid;
                    c.slotId = slotId;
                    c.hasAttacked = hasAttacked;

                    this.p_handCards.Add(c);
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
                        this.e_handCards.Add(cid);
                    }

                }
            }



            // Create game processing loop
            Thread pollThread = new Thread(delegate()
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    this.ProcessGame();

                }



            });
            pollThread.Start();

        }


        public void ProcessGame()
        {
            if (yourTurn)
            {

                //////////////////////////////////////////////////////////////////////////
                // Attempt to Use cards (Use All mana)
                //////////////////////////////////////////////////////////////////////////
                var rnd = new Random((int)DateTime.Now.Ticks); // Random for random list
                foreach (Card c in p_handCards.OrderBy(x => rnd.Next()))
                {
                    // Can afford to use the card
                    if (this.mana - c.cost >= 0)
                    {
                        this.mana -= c.cost; //Subtract mana

                        // Loop through all of the slots until a empty slot (Null) is found
                        for (int slot = 0; slot < 7; slot++)
                        {

                            if (!p_boardCards.ContainsKey(slot))
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
            if (e_boardCards.Values.Count <= 0) return;

            foreach (Card c in p_boardCards.Values)
            {
            
                Random rnd = new Random();
                int r = rnd.Next(e_boardCards.Values.Count);

                Card oppC = e_boardCards.Values.ElementAt(r);

                this.SendTo(
                    new Response(
                        GameService.GameType.ATTACK,
                        new JObject(
                            new JProperty("source",c.cid),
                            new JProperty("target",oppC.cid),
                            new JProperty("type", 0 /*TODO*/))
                            )
     
                   );



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
            c.slotId = slotId;
            this.p_handCards.Remove(c);
            this.p_boardCards.Add(slotId, c);

            this.SendTo(
                new Response(
                    GameService.GameType.USECARD,
                            new JObject(
                                new JProperty("cid", c.cid),
                                new JProperty("slotId", slotId)
                                ))
                    );
        }


        public int health { get; set; }

        public int mana { get; set; }
    }
}
