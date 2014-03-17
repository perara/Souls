define("gameService", ["networkBase"], function (NetworkBase) {

    var that;
    GameService = function (engine, socket) {
        NetworkBase.call(this);

        that = this;
        console.log("> Network Manager")

        this.engine = engine;
        this.socket = this.engine.gameSocket;



        /// Game 
        this.RegisterResponseAction(["11", "12", "13"], Response_NotLoggedIn);
        this.RegisterResponseAction(["10"], Response_LoggedIn);
        this.RegisterResponseAction(["100"], Response_QueueOK);
        this.RegisterResponseAction(["206"], Response_GameCreate);
        this.RegisterResponseAction(["209"], Response_GameOpponentMove);
        this.RegisterResponseAction(["210"], Response_GameOpponentRelease);

    }
    // Constructor
    GameService.prototype = Object.create(NetworkBase.prototype);
    GameService.prototype.constructor = GameService;


    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    ///////////////////////GAME-RESPONSES/////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    function Response_NotLoggedIn(json) {
        console.log((arguments.callee.name) + ": NOT LOGGED IN resp : ");

    }

    function Response_LoggedIn() {
        console.log(":D");
        that.engine.chatService.Connect();
        that.engine.chatService.Login();
    }

    function Response_QueueOK() // 100
    {

    }

    function Response_GameCreate(data) // 206
    {
        console.log("heh");
        that.engine.gameId = data.Payload.gameId;
        that.engine.player.SetText(data.Payload.player.info);
        that.engine.opponent.SetText(data.Payload.opponent.info);
        that.engine.player.GiveCards(data.Payload.player.hand, {
            x: 200,
            y: 1000,
            playoropp: "Player"
        });
        that.engine.opponent.GiveCards(data.Payload.opponent.hand, {
            x: 200,
            y: 200,
            playoropp: "Opponent"
        });


        // Create Chat room (If you are player 1)
        if (data.Payload.ident == 1) {
            that.engine.chatService.RequestNewGameRoom();
        }

    }

    function Response_GameOpponentMove(json) {

        var card = that.engine.opponent.cardManager.hand[json.Payload.cid];


        var position = {
            x: card.x,
            y: card.y
        };

        var tween = new TWEEN.Tween(position).to({
            x: json.Payload.x,
            y: json.Payload.y
        }, 500)
            .easing(TWEEN.Easing.Linear.None)
            .onUpdate(onUpdate)


        function onUpdate() {
            card.x = position.x;
            card.y = position.y;
        }

        tween.start();

    }

    function Response_GameOpponentRelease(json) {
        var card = that.engine.opponent.cardManager.hand[json.Payload.cid];

        console.log(card);

        card.AnimateBack(card);


    }
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    ////////////////////////////REQUESTS//////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    GameService.prototype.Login = function () {
        this.socket.send(this.message.GENERAL.LOGIN);
        this.socket.send(this.message.GAME.QUEUE); // TODO, this should not be called here.
    }



    return GameService;




});