define("game",
    ["jquery",
     "stopwatch",
    "state",
    "player",
    "opponent",
    "conf",
    "gameService",
    "background",
    "cardslot",
    "chatService",
    "socket",
    "pixi",
    "toolbox",
    'easeljs',
    'tweenjs',
    'queue',
    'proton'], function ($, stopwatch, State, Player, Opponent, Conf, GameService, Background, CardSlots, ChatService, Socket, Pixi, ToolBox, CreateJS, _, Queue, proton) {

        var that;
        Engine = function () {
            that = this;
            State.call(this, "0xCAECAE");
            console.log("> Game Class");
            // Variables
            this.gameId = undefined;
            this.conf = Conf;
            // Create Groups
            this.addGroup("Background");
            this.addGroup("EndTurn");
            this.addGroup("CardSlot-Opponent");
            this.addGroup("CardSlot-Player");
            this.addGroup("Card-Opponent");
            this.addGroup("Opponent");
            this.addGroup("Player");
            this.addGroup("Card-Player");
            this.addGroup("Arrow");
            this.addGroup("Card-Focus");
            this.addGroup("Attacker");
            this.addGroup("Queue");
            this.addGroup("Text");

            // Set Default group Visibility to false (Is set to true on Response_GameCreate in GameService.js
            this.getGroup("Player").visible = false;
            this.getGroup("Opponent").visible = false;
            this.getGroup("EndTurn").visible = false;



            // Network for best host
            this.gameSocket = new Socket("ws://hybel.keel.no:8140/game");
            this.chatSocket = new Socket("ws://hybel.keel.no:8140/chat");

            // Network for råtn host
            //this.gameSocket = new Socket("ws://tux.persoft.no:8140/game");
            //this.chatSocket = new Socket("ws://tux.persoft.no:8140/chat");

            // Tools etc
            this.toolbox = ToolBox;
            this.CreateJS = CreateJS;
            CreateJS.Ticker.setFPS(Conf.FPS);

            // Objects
            this.player = new Player(this);
            this.opponent = new Opponent(this);
            this.background = new Background(this);
           // this.queue = new Queue(this);

            // Connect to the chat service
            this.chatService = new ChatService(this);
            //this.chatService.OpenChatWindow(this);

            // Connect to the game service
            this.gameService = new GameService(this)



            this.OnStart();
        }

        // Create a object of the State prototype, then set the constructor to Game function.
        Engine.prototype = Object.create(State.prototype);
        Engine.prototype.constructor = Engine;


        Engine.prototype.OnStart = function () {
            this.background.Init();
            this.opponent.Init();
            this.player.Init();


            this.gameService.Connect();
            this.gameService.Login();
            this.chatService.Connect();

        }

        Engine.prototype.OnEnd = function () {


        }

        Engine.prototype.OnPause = function () {


        }


        Engine.prototype.Process = function () {
            this.gameService.Process();
            this.chatService.Process();

            this.player.Process();

            return this.stage;
        };



        // Afunction which displays a fadeinout text on the screen
        // https://api.jquery.com/animate/

        /// <signature>
        /// <summary>Function summary 1.</summary>
        /// <param name="messageArray" type="array">A array of words/sentances</param>
        /// <param name="forever" type="bool">Decides if the function should run permanently (until closed by callback)</param>
        /// <param name="callback" type="array">callback returns intervalId and round count</param>
        /// <returns type="void" />
        /// </signature>
        Engine.prototype.ScreenMessage = function (messageArray, forever, callback) {

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
            flashText.position.x = that.conf.width / 2;
            flashText.position.y = that.conf.height / 2;
            flashText.alpha = 0;
            flashText.forever = forever;
            flashText.message = messageArray;
            flashText.countTo = messageArray.length;
            var group = that.getGroup("Text")
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

        return Engine;
    });