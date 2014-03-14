define("game", [
    "pixi",
    "state",
    "asset",
    "conf",
    "socket",
    "card",
    "cardslot",
    "toolbox",
    "player",
    "tween",
    "opponent"], function (Pixi, State, Asset, Conf, Socket, card, CardSlot, Toolbox, tween) {

        function Game() {
            // Extend State
            State.call(this, "0xCAECAE");
            console.log("> Game Class");


            this.Player = undefined;
            this.Opponent = undefined;

            this.OnStart();
            return this;
        }

        // Create a object of the State prototype, then set the constructor to Game function.
        Game.prototype = Object.create(State.prototype);
        Game.prototype.constructor = Game;

        // This contains all relevant data to the plater, IE which card he is holding.


        // Contains all general game data, IE gameId
        Game.prototype.GameData = {};


        Game.prototype.OnStart = function () {
            // Create Sprite Groups
            this.addGroup("Environment", new Pixi.DisplayObjectContainer());
            this.addGroup("CardSlots", new Pixi.DisplayObjectContainer());
            this.addGroup("Player", new Pixi.DisplayObjectContainer());
            this.addGroup("Card", new Pixi.DisplayObjectContainer());

            // Set group specific settings
            this.getGroup("CardSlots").visible = false;

            // Setup the Background image
            var bg = new Pixi.Sprite(Asset.GetTexture(Asset.Textures.GAME_BG));
            bg.width = Conf.Data.width;
            bg.height = Conf.Data.height;
            bg.interactive = true;
            this.addChild("Environment", bg);
            bg.mousemove = function (mouseData) {
                // this line will get the mouse coords relative to the sprites parent..
                var parentCoordsPosition = mouseData.getLocalPosition(this.parent);


                if (!!Game.prototype.Player) Game.prototype.Player.mouse = parentCoordsPosition;

            }

            // Register all of the callbacks specified with a response code
            this.RegisterNetworkCallbacks();


            Game.prototype.gameClock = 0;
            this.StartGameClock();

        }

        Game.prototype.StartGameClock = function () {
            setInterval(function () {
                Game.prototype.gameClock++;
            }, 100);
        }

        Game.prototype.RegisterNetworkCallbacks = function () {
            Socket.RegisterCallback(["209"], Game.prototype.OpponentMove)

        }

        Game.prototype.Process = function () {

            $.each(Game.prototype.getGroup("CardSlots").children, function (key, cardslot) {
                cardslot.Process();
            });

            $.each(Game.prototype.getGroup("Card").children, function (key, card) {
                card.Process();
            });

            // Process Player
            if (!!this.Player) {
                this.Player.p.Process()
            }

            // Process tweening
            TWEEN.update();

            return this.stage;
        };

        Game.prototype.CreatePlayer = function (payload) {

            // Init player
            var player = new Player(payload.player.info);

            player.position = {
                x: Conf.Data.width / 2,
                y: Conf.Data.height - player.height / 2
            };
            this.addChild("Player", player);

            // Init Arrow
            var playerArrow = player.Arrow.SetupArrow();
            this.addChild("Player", playerArrow);
            Game.prototype.Player = player;

            // Now create Player Cards
            this.CreateCards(payload.player,
                {
                    x: 200,
                    y: 1000,
                    player: true
                });

        }

        Game.prototype.CreateOpponent = function (payload) {
            // Init player
            var opponent = new Opponent(payload.opponent.info);

            opponent.position = {
                x: Conf.Data.width / 2,
                y: opponent.height / 4
            };
            this.addChild("Player", opponent);
            Game.prototype.Opponent = opponent;

            // Now create Opponent Cards
            this.CreateCards(payload.opponent,
                 {
                     x: 200,
                     y: 200,
                     player: false
                 });
        }

        Game.prototype.CreateSlots = function () {
            // Create Card Slots and Cards
            var csStartX = 530;
            var cardSlots = new Array();
            for (var i = 0; i < 7/*TODO,MAXCARD??*/; i++) {

                // cS as CardSlot. Creates a new object and set the position relative to the counter.
                var cS = new CardSlot();
                cS.width = 120;
                cS.x = csStartX + (i * (cS.width + 25));
                cS.y = 650;
                this.addChild("CardSlots", cS);
                cardSlots.push(cS);
            }

            Game.prototype.GameData.CardSlots = cardSlots; //TODO? should be this or somthing?
        }



        Game.prototype.CreateCards = function (data, config) {
            // Config:
            // x (int)
            // y (int)
            // player (bool)
            ////////////////

            var count = 0;
            for (var i in data.hand) {
                count++;
                var cid;
                if (config.player) {
                    cid = i;
                }
                else {
                    cid = data.hand[i];
                }

                var cardData = (config.player) ? data.hand[i] : undefined;
                console.log(cardData);
                var c = new Card(cardData);
                c.x = c.originX = config.x + (120 * (count));
                c.y = c.originY = config.y;
                c.originRot = 0;

                if (!!config.player) {
                    c.interactive = true;
                    c.InteractionCallback(this.CardInteractionCallback);
                    this.Player.cards[cardData.cid] = c;
                }
                else {
                    c.CardData.cid = cid;
                    this.Opponent.cards[cid] = c;
                }

                this.addChild("Card", c);

            }


            /* var startX = 200;
             var startY = 1000;
             for (var i = 0; i < payload.hand.length; i++) {
                 var c = new Card(payload.hand[i]);
                 c.x = c.originX = startX += 120;
                 c.y = c.originY = startY;
                 c.originRot = 0;
                 c.InteractionCallback(this.CardInteractionCallback);
                 this.addChild("Card", c);
                 this.Player.cards[payload.hand[i].cid] = c;
             }*/




            /* var PI = 3.14159265358979323846;
             var slice = -PI / payload.First.length + 11;    // Constant NUM cards  + 2
    
             var offsetX = 450;
             var offsetY = -290;
             var rotDeg = 10 
             for (var i = 2;  i < payload.First.length + 2 ; i++) {
                 
                 var c = new Card(payload.First[i-2]);
    
                 // Make space between half of the hand (cards)
    
    
                 rotDeg -= 10; // Rotation minus 10 degrees
                 var angle = slice * i;
                 var newX = ((Conf.Data.width / 2) + 300 * Math.cos(angle));
                 var newY = ((Conf.Data.height - c.height / 2) + 150 * Math.sin(angle)); // BFORE +150
    
    
                 c.x = c.originX = newX - offsetX;
                 c.y = c.originY = newY + offsetY;
                 c.InteractionCallback(this.CardInteractionCallback);
                 c.originRot = rotDeg
    
    
                 c.rotation = rotDeg * (PI / 180);
                 this.addChild("Card", c);
    
        
            }*/
        }

        // Afunction which displays a fadeinout text on the screen
        // https://api.jquery.com/animate/

        /// <signature>
        /// <summary>Function summary 1.</summary>
        /// <param name="messageArray" type="array">A array of words/sentances</param>
        /// <param name="forever" type="bool">Decides if the function should run permanently (until closed by callback)</param>
        /// <param name="callback" type="array">callback returns intervalId and round count</param>
        /// <returns type="void" />
        /// </signature>
        Game.prototype.ScreenMessage = function (messageArray, forever, callback) {

            var msg = messageArray.shift();
            if (forever) messageArray.push(msg);

            var flashText = new Pixi.Text(msg,
            {
                font: "70px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });

            flashText.anchor = { x: 0.5, y: 0.5 };
            flashText.position.x = Conf.Data.width / 2;
            flashText.position.y = Conf.Data.height / 2;
            flashText.alpha = 0;
            flashText.forever = forever;
            flashText.message = messageArray;
            flashText.countTo = messageArray.length;
            var group = this.getGroup("Environment")
            group.addChild(flashText);

            var loopFlag = true;
            var roundCount = 0;
            var round = 0;
            var intervalId = setInterval(function () {
                if (flashText.alpha < 1 && loopFlag) {
                    flashText.alpha += 0.1;
                    if (flashText.alpha > 1) {
                        loopFlag = false;

                    }
                }

                else if (flashText.alpha > 0 && !loopFlag) {
                    flashText.alpha -= 0.1;
                    if (flashText.alpha < 0) {
                        loopFlag = true;

                        if (messageArray.length != 0) {
                            var msg = messageArray.shift();
                            flashText.setText(msg);
                            if (!!forever) {
                                messageArray.push(flashText.text)
                                if (roundCount++ % messageArray.length == 0) round++; // Increment round
                                if (!!callback) callback(intervalId, round)
                            }

                        }

                        if (messageArray.length == 0) {

                            group.removeChild(flashText);
                            clearInterval(intervalId);
                        }
                    }
                }
            }, 100);
        }


        Game.prototype.CardInteractionCallback = function (card, type) {

            // Hovering a card
            if (type == "hover") {
                card.OrderLast(Game.prototype.getGroup("Card"));
            }


            // Leaving a card
            if (type == "leave") {
                card.OrderOriginalPosition(Game.prototype.getGroup("Card"));
            }

            // Dragging a card
            if (type == "drag") {

                // NETWORK, 
                //console.log(Game.prototype.OpponentMoveTick);
                //console.log(Game.prototype + Game.prototype.gameClock);
                if (Game.prototype.OpponentMoveTick < Game.prototype.gameClock) {
                    Game.prototype.OpponentMoveTick = Game.prototype.gameClock + 1;
                    console.log("snett");

                    var json = Socket.Message.GAME.MOVE_CARD;
                    json.Payload.gameId = Game.prototype.GameData.gameId;
                    json.Payload.cid = card.CardData.cid;
                    json.Payload.x = Game.prototype.Player.mouse.x;
                    json.Payload.y = Game.prototype.Player.mouse.y;

                    Socket.send(json);
                }


                var cardHoverSlot = null;
                $.each(Game.prototype.getGroup("CardSlots").children, function (key, cardslot) {

                    // Check if card is hovering a cardSlot
                    if ((Toolbox.Rectangle.intersectsYAxis(card, cardslot, { x: -10, y: -15 }, { x: 3, y: 3 }) == true) && cardslot.card.used == false) {
                        cardHoverSlot = cardslot;

                    }
                    else {
                        card.hoverSlot = null;
                        cardslot.card.hover = false;
                    }
                });

                // If we found a hoverslot
                if (cardHoverSlot != null) {
                    card.hoverSlot = cardHoverSlot;
                    card.hoverSlot.card.hover = true;
                }

            }

            // Clicking on a card
            if (type == "click") {
                // Run Pickup Tween
                card.Pickup();

                card.OrderLast(Game.prototype.getGroup("Card"));

                // Only Pickup if its not already in a slot
                if (!card.inSlot) {
                    Game.prototype.Player.pickedUpCard = card;
                    Game.prototype.getGroup("CardSlots").visible = true;
                }
            }

            if (type == "release") {
                card.OrderOriginalPosition(Game.prototype.getGroup("Card"));

                // if card still has a hoverslot when release, we want to place it into that slot.
                if (card.hoverSlot != null) {

                    ///NETWORK: send "USECARD" command 
                    var json = Socket.Message.GAME.USECARD;
                    json.Payload.gameId = Game.prototype.GameData.gameId
                    console.log(card);
                    json.Payload.cid = card.CardData.cid;
                    json.Payload.slotId = card.hoverSlot.slotId;

                    Socket.send(json);

                    Socket.RegisterCallback(["202", "208", "207"], function (data) {


                        console.log("----------------");
                        console.log(data.Type);
                        console.log(data.Payload);
                        console.log("----------------");


                        var whichCard = data.Payload.card;
                        var toSlot = data.Payload.slot;

                        var card = Game.prototype.Player.cards[whichCard];
                        var slot = Game.prototype.GameData.CardSlots[toSlot];

                        // GAME_USECARD_OK
                        if (data.Type == 207) {

                            card.putInSlot(slot);
                            card.inSlot = slot;
                            card.hoverSlot = null //NEEDED? test PLS //TODO
                            slot.card.used = true; //TODO tree structure (Remove .card)


                        }
                        else if (data.Type == 202) //GAME_NOT_YOUR_TURN
                        {
                            card.AnimateBack();
                            Game.prototype.ScreenMessage(["Not your turn!"]);
                        }
                        else if (data.Type == 208) //GAME_OOM
                        {
                            card.AnimateBack();
                            Game.prototype.ScreenMessage(["Not enough mana!"]);
                        }


                    });


                } else if (card.inSlot == null) {
                    card.AnimateBack();
                }

                // Release the card
                Game.prototype.Player.pickedUpCard = null;


                // Hide CardSlot displayGroup
                Game.prototype.getGroup("CardSlots").visible = false;
            }


            //////////////////////////////////////////
            /////////////////////////////////////////
            ////While card is mounted! /////////////
            ///////////////////////////////////////
            // Dragging while card is in slot
            if (type == "drag-slot") {
                Game.prototype.Player.Arrow.Draw(Game.prototype.Player.mouse, { x: card.x, y: card.y });
            }

            if (type == "release-slot") {
                Game.prototype.Player.Arrow.Hide();
            }

            if (type == "click-slot") {
                Game.prototype.Player.Arrow.Show();
            }
        }


        Game.prototype.OpponentMoveTick = 0;
        Game.prototype.OpponentMove = function (data) {

            var opponentCard = Game.prototype.Opponent.cards[data.Payload.cid];



            var position = opponentCard.position;
            var target = { x: data.Payload.x, y: data.Payload.y };
            var tween = new TWEEN.Tween(position).to(target, 1000);

            tween.easing(TWEEN.Easing.Exponential.Out); // THIS DETERMINES THE TWEEN

            var that = this;
            tween.onUpdate(function () {
                opponentCard.position = position;
            });

            tween.start();

        }



        Socket.onMessage(function (json) {
            var data = JSON.parse(json.data);
            // Run callbacks
            if (typeof Socket.Callbacks[data.Type] != 'undefined') {
                for (var index in Socket.Callbacks[data.Type]) {
                    Socket.Callbacks[data.Type][index](data);
                }

            }

            // 206 = GAME_CREATE 
            if (data.Type == 206) {
                // Set gameId
                Game.prototype.GameData.gameId = data.Payload.gameId;
                Game.prototype.CreateSlots();
                Game.prototype.CreatePlayer(data.Payload);
                Game.prototype.CreateOpponent(data.Payload);
            }
        });


        return new Game();
    });