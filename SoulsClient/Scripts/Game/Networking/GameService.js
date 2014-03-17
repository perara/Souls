define("gameService", ["messages", "networkBase"], function (Message) {

    var that;
    GameService = function (engine, socket) {

        that = this;
        console.log("> Network Manager")

        this.engine = engine;
        this.socket = this.engine.gameSocket;

        // Create what is needed for the network to work
        this.responseAction = new Object();
        this.networkBuffer = GameService.prototype.networkBuffer = new Array();
        this.message = Message;

        /// Game 
        this.RegisterResponseAction(["11", "12", "13"], Response_NotLoggedIn);
        this.RegisterResponseAction(["10"], Response_LoggedIn);
        this.RegisterResponseAction(["100"], Response_QueueOK);
        this.RegisterResponseAction(["206", "220"], Response_GameCreate);
        this.RegisterResponseAction(["209"], Response_GameOpponentMove);
        this.RegisterResponseAction(["210"], Response_GameOpponentRelease);
    }
    // Constructor
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
        that.engine.chatService.Login();
    }

    function Response_QueueOK() // 100
    {

    }

    function Response_GameCreate(data) // 206 CREATE // 220 RECOVER
    {
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
        if (data.Payload.ident == 1 && data.Type != 220) {
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


    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    //NETWORKBASEHERPDERPFUCKINHERITANCE.
    ////////////////////////////////////////////////////////////////
    GameService.prototype.RegisterResponseAction = function (responseArray, func) {
        for (var response in responseArray) {
            this.responseAction[responseArray[response]] = func;
        }
    }

    GameService.prototype.GetResponseAction = function (responseId) {
        if (!this.responseAction[responseId]) {
            console.log("Could not find RESPONSE!" + responseId)

        }
        return this.responseAction[responseId];
    }


    GameService.prototype.Process = function () {
        if (GameService.prototype.networkBuffer.length > 0) {
            // Get a packet
            var packet = GameService.prototype.networkBuffer.shift();

            if (!!this.GetResponseAction(packet.Type))
                this.GetResponseAction(packet.Type)(packet);
        }

    }

    GameService.prototype.Send = function (json) {
        this.socket.send(json);
    }

    GameService.prototype.Connect = function () {
        this.socket.connect();

        // Recieves all Data from server
        this.socket.onMessage(GameService.prototype.TrafficHandler);
    }


    GameService.prototype.TrafficHandler = function (json) {
        var prsJson = JSON.parse(json.data);
        console.log("[GAME]Received: " + prsJson.Type);
        GameService.prototype.networkBuffer.push(prsJson);
    }










    return GameService;




});