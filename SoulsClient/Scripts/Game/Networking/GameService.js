﻿define("gameService", ["messages"], function (Message) {

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
        this.RegisterResponseAction(["210", "212"], Response_GameOpponentRelease);
        this.RegisterResponseAction(["207", "211"], Response_UseCard);
        this.RegisterResponseAction(["202"], Repsonse_NotYourTurn);
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

    function Response_UseCard(json) // "207" GAME_USECARD_PLAYER_OK // "211" GAME_USECARD_OPPONENT_OK // TOODO FAILED OSV
    {

        console.log(json);

        var card;
        var cardSlot;

        if (json.Type == 211) // OPPONENT
        {
            card = that.engine.opponent.cardManager.hand[json.Payload.cid];
            cardSlot = that.engine.opponent.cardManager.cardSlots[json.Payload.slotId];
        }
        else if (json.Type == 207) // PLAYER
        {
            card = that.engine.player.cardManager.hand[json.Payload.cid];
            cardSlot = that.engine.player.cardManager.cardSlots[json.Payload.slotId];
        }

        card.PutInSlot(cardSlot);
    }

    function Response_GameCreate(data) // 206 CREATE // 220 RECOVER
    {

        console.log(data);

        that.engine.gameId = data.Payload.gameId;
        that.engine.player.SetText(data.Payload.player.info);
        that.engine.opponent.SetText(data.Payload.opponent.info);

        // Give cards to hand
        that.engine.player.cardManager.JSONToHandCards(data.Payload.player.hand, {
            x: 200,
            y: 1000,
            playoropp: "Player"
        });

        that.engine.opponent.cardManager.JSONToHandCards(data.Payload.opponent.hand, {
            x: 200,
            y: 200,
            playoropp: "Opponent"
        });

        that.engine.player.cardManager.JSONToBoardCards(data.Payload.player.board);
        that.engine.opponent.cardManager.JSONToBoardCards(data.Payload.opponent.board);


        // Create Chat room (If you are player 1)
        if (data.Payload.ident == 1 && data.Type != 220) {
            that.engine.chatService.RequestNewGameRoom();
        }

    }

    function Response_GameOpponentMove(json) {

        var card = that.engine.opponent.cardManager.hand[json.Payload.cid];


        var target =
          {
              x: json.Payload.x,
              y: json.Payload.y
          }

        //TODO bug which makes it so that its choppy. CHANGED callback!
        that.engine.CreateJS.Tween.get(card, { override: true })
            .to(target, 1000, that.engine.CreateJS.Ease.linear)
            .call(onComplete);

        function onComplete() {
            // Set Mount variable to true
        }

    }

    function Response_GameOpponentRelease(json) // 210 GAME_OPPONENT_RELEASE // 212 GAME_PLAYER_RELEASE
    {
        var card;
        if (json.Type == 210) {
            card = that.engine.opponent.cardManager.hand[json.Payload.cid];
        }
        else if (json.Type == 212) {
            card = that.engine.player.cardManager.hand[json.Payload.cid];
        }

        card.AnimateBack(card);
    }

    function Repsonse_NotYourTurn(json) // 202 NOT YOUR TURN
    {
        console.log(json);
        var card = that.engine.player.cardManager.hand[json.Payload.card];
        
        that.engine.ScreenMessage(["Not your turn!"], false);
    
        
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

    GameService.prototype.Request_UseCard = function (cid, slotId) {
        var json = this.message.GAME.USECARD;
        json.Payload.cid = cid;
        json.Payload.slotId = slotId;
        this.socket.send(json);
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