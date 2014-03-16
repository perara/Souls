define("networkmanager", ["jquery","messages"], function ($, Message) {

    var that;
    NetworkManager = function (engine, socket) {
        that = this;
        console.log("> Network Manager")
        this.engine = engine;
        this.socket = this.engine.gameSocket;

        this.responseAction = new Object();
        this.networkBuffer = NetworkManager.prototype.networkBuffer = new Array();

        this.RegisterResponseAction(["11", "12", "13"], Response_NotLoggedIn);
        this.RegisterResponseAction(["10"], Response_LoggedIn);
        this.RegisterResponseAction(["100"], Response_QueueOK);
        this.RegisterResponseAction(["206"], Response_GameCreate);
        this.RegisterResponseAction(["209"], Response_GameOpponentMove);
        this.RegisterResponseAction(["210"], Response_GameOpponentRelease);
    }

    // Constructor
    NetworkManager.prototype.constructor = NetworkManager;

    NetworkManager.prototype.RegisterResponseAction = function (responseArray, func) {
        for (var response in responseArray) {
            this.responseAction[responseArray[response]] = func;
        }
    }

    NetworkManager.prototype.GetResponseAction = function (responseId) {
        if (!this.responseAction[responseId]) {
            console.log("Could not find RESPONSE!" + responseId)

        }
        return this.responseAction[responseId];
    }


    function Response_NotLoggedIn(json) {
        console.log((arguments.callee.name) + ": NOT LOGGED IN resp : ");

    }

    function Response_LoggedIn() {

    }

    function Response_QueueOK() // 100
    {

    }

    function Response_GameCreate(data) // 206
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


    NetworkManager.prototype.Process = function () {

        if (this.networkBuffer.length > 0) {
            // Get a packet
            var packet = this.networkBuffer.shift();

            if (!!this.GetResponseAction(packet.Type))
                this.GetResponseAction(packet.Type)(packet);


        }

    }









    NetworkManager.prototype.Connect = function () {
        this.socket.connect();

        // Recieves all Data from server
        this.socket.onMessage(TrafficHandler);
    }

    NetworkManager.prototype.Login = function () {
        this.socket.send(Message.GENERAL.LOGIN);
        this.socket.send(Message.GAME.QUEUE); // TODO, this should not be called here.
    }

    NetworkManager.prototype.Send = function (json) {

        this.socket.send(json);
    }


    function TrafficHandler(json) {
        console.log(json.data);
        NetworkManager.prototype.networkBuffer.push(JSON.parse(json.data));
    }





    return NetworkManager;




});