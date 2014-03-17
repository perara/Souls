define("game",
    ["jquery",
     "stopwatch",
    "state",
    "player",
    "opponent",
    "inputmanager",
    "conf",
    "gameService",
    "background",
    "tween",
    "cardslot",
    "chatService",
    "socket"], function ($, stopwatch, State, Player, Opponent, InputManager, Conf, GameService, Background, Tween, CardSlots, ChatService, Socket) {


        Engine = function () {
            State.call(this, "0xCAECAE");
            console.log("> Game Class");
            // Variables
            this.gameId = undefined;
            this.conf = Conf;
            // Create Groups
            this.addGroup("Background");
            this.addGroup("CardSlot");
            this.addGroup("Opponent");
            this.addGroup("Player");

            // Network
            this.gameSocket = new Socket("ws://hybel.keel.no:8140/game");
            this.chatSocket = new Socket("ws://hybel.keel.no:8140/chat");

            // Objects
            this.player = new Player(this);
            this.opponent = new Opponent(this);
            this.inputManager = new InputManager(this);
            this.background = new Background(this);

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

            //conlosle.log(this.Groups);
            this.gameService.Connect();
            this.gameService.Login();
            this.chatService.Connect();

            console.log(this.stage);
        }

        Engine.prototype.OnEnd = function () {


        }

        Engine.prototype.OnPause = function () {


        }


        Engine.prototype.Process = function () {
            this.gameService.Process();
            this.chatService.Process();

            this.inputManager.Process();
            this.player.Process();
            TWEEN.update();

            return this.stage;
        };

        return Engine;
    });