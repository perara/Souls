define("game",
    ["jquery",
     "stopwatch",
    "state",
    "player",
    "opponent",
    "inputmanager",
    "conf",
    "networkmanager",
    "background",
    "tween",
    "cardslot",
    "chat",
    "socket"], function ($, stopwatch, State, Player, Opponent, InputManager, Conf, NetworkManager, Background, Tween, CardSlots, Chat, Socket) {


        Engine = function () {
            State.call(this, "0xCAECAE");
            console.log("> Game Class");
            // Variables
            this.gameId = undefined;
            this.conf = Conf;

            // Network
            this.gameSocket = new Socket("ws://tux.persoft.no:8140/game");
            this.chatSocket = new Socket("ws://tux.persoft.no:8140/chat");

            // Objects
            this.player = new Player(this);
            this.opponent = new Opponent(this);
            this.inputManager = new InputManager(this);
            this.networkManager = new NetworkManager(this)
            this.background = new Background(this);
            this.chat = new Chat(this);
            this.chat.OpenChatWindow(this);

           // this.cardSlots = new CardSlot

            this.OnStart();

        }

        // Create a object of the State prototype, then set the constructor to Game function.
        Engine.prototype = Object.create(State.prototype);
        Engine.prototype.constructor = Engine;


        Engine.prototype.OnStart = function () {
            this.background.Init();
            this.player.Init();
            this.opponent.Init();

            this.networkManager.Connect();
            this.networkManager.Login();

            this.chat.Connect();

            // Login should happen AFTER gamelogin is complete!
            var that = this;
            setTimeout(function () { that.chat.Login() }, 3000);
          
        }

        Engine.prototype.OnEnd = function () {


        }

        Engine.prototype.OnPause = function () {


        }


        Engine.prototype.Process = function () {
            this.networkManager.Process();

            this.inputManager.Process();
            this.player.Process();
            TWEEN.update();
            return this.stage;
        };






        return Engine;
    });