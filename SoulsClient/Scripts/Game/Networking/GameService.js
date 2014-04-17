define("gameService", ["messages"], function (Message) {

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
        this.RegisterResponseAction(["210", "212"], Response_GameRelease);
        this.RegisterResponseAction(["207", "211"], Response_UseCard);
        this.RegisterResponseAction(["202"], Repsonse_NotYourTurn);
        this.RegisterResponseAction(["213"], Response_SlotOccupied);
        this.RegisterResponseAction(["218"], Response_Attack);
        this.RegisterResponseAction(["226"], Response_NextTurn);
        this.RegisterResponseAction(["222", "223"], Response_NewCard);
        this.RegisterResponseAction(["208"], Response_UseCard_OOM);
        this.RegisterResponseAction(["230", "231"], Response_VictoryDefeat);


    }
    // Constructor
    GameService.prototype.constructor = GameService;


    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    ///////////////////////GAME-RESPONSES/////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    function Response_UseCard_OOM(json) {
        that.engine.ScreenMessage(["Not enough mana!"], false);
    }

    function Response_NewCard(json) {
        var type = json.Type;
        var cards = json.Payload.card;

        if (type == 222) {


            that.engine.player.cardManager.JSONToHandCards(cards,
                {
                    x: 165,
                    y: 980
                });
        }
        else if (type == 223) {


            that.engine.opponent.cardManager.JSONToHandCards(cards,
             {
                 x: 165,
                 y: 200
             });

        }

    }

    function Response_VictoryDefeat(json) // 230 = Victory , 231 = Defeat
    {
        var type = json.Type;

        that.engine.OnEnd();
        if(type == 230)
        {

            that.engine.queue.FadeInGameEnd("Victory!");
        }
        else
        {
            that.engine.queue.FadeInGameEnd("Defeat!");
        }



    }


    function Response_Attack(json) {

        var attackType = json.Payload.type;

        // Fetch CardInfo (dmgDOne etc)
        var jsonPInfo = json.Payload.player;
        var jsonOppInfo = json.Payload.opponent;

        // Determine the attacker
        var jsonAttacker = json.Payload.player.attacker;


        if (attackType == 0) { // Card on Card
            // Get cards
            var playerCard = that.engine.player.cardManager.board[jsonPInfo.cid];
            var opponentCard = that.engine.opponent.cardManager.board[jsonOppInfo.cid];


            // The player is attacking
            if (jsonAttacker) {
                playerCard.Attack(jsonPInfo, jsonOppInfo, opponentCard);
            }
            else // The opponent is attacking
            {
                opponentCard.Attack(jsonOppInfo, jsonPInfo, playerCard);
            }
        }
        else if (attackType == 1) { // Card on hero

            // The player is attacking
            if (jsonAttacker) {
                var attacker = that.engine.player.cardManager.board[jsonPInfo.cid];
                var defender = that.engine.opponent;

                attacker.AttackOpponent(jsonPInfo, jsonOppInfo, defender);
            }
            else // The opponent is attacking
            {
                var attacker = that.engine.opponent.cardManager.board[jsonOppInfo.cid];
                var defender = that.engine.player;

                attacker.AttackOpponent(jsonOppInfo,jsonPInfo, defender);
            }

        }

    } // -- Function end

    function Response_NotLoggedIn(json) {
        console.log((arguments.callee.name) + ": NOT LOGGED IN resp : ");

    }

    function Response_LoggedIn() {
        that.engine.chatService.Login();
        that.engine.queue.SetText("Connection established!")
    }

    function Response_QueueOK() // 100
    {
        that.engine.queue.SetText("Queued... Waiting for Match")
    }

    function Response_SlotOccupied(json) // 213 GAME_SLOTOCCUPIED
    {

        that.engine.ScreenMessage([json.Payload.message], false);
    }

    function Response_UseCard(json) // "207" GAME_USECARD_PLAYER_OK // "211" GAME_USECARD_OPPONENT_OK // TOODO FAILED OSV
    {

        var cardData = json.Payload.card;
        var pInfo = json.Payload.pInfo;
        var cid = cardData.cid;
        var slotId = cardData.slotId;
        var card;
        var cardSlot;

        if (json.Type == 211) // OPPONENT
        {

            var opponent = that.engine.opponent

            // Set updated opponent text
            opponent.SetText(pInfo);

            card = opponent.cardManager.hand[cid]; // Fetch from hand
            delete opponent.cardManager.hand[cid]; // Delete from hand
            opponent.cardManager.board[cid] = card; // Add to board
            opponent.cardManager.SortHand();

            // Update the card
            card.SetText(
             {
                 health: cardData.health,
                 attack: cardData.attack,
                 name: cardData.name,
                 cost: cardData.cost,
                 ability: cardData.ability.name,
                 race: cardData.race,
                 id: cardData.id
             });


            // Set the Card Texture
            var texture = card.CardType.GetCardTexture(cardData.race.id);
            card.frontCard.texture = texture;

            // Set the Card Portrait.
            var portrait = card.CardType.GetPortraitTexture(cardData.id)
            card.portrait.texture = portrait;
            card.portrait.width = 236;
            card.portrait.height = 167;

            card.ScaleDown();


            // card.SetupTextData(json.Payload.card, true);
            cardSlot = opponent.cardManager.cardSlots[slotId];
        }
        else if (json.Type == 207) // PLAYER
        {
            var player = that.engine.player;

            // Set updated opponent text
            player.SetText(pInfo);

            card = player.cardManager.hand[cid]; // Fetch from hand
            delete player.cardManager.hand[cid]; // Delete from hand
            player.cardManager.SortHand();
            player.cardManager.board[cid] = card; // Add to board
            cardSlot = player.cardManager.cardSlots[slotId];
        }

        card.PutInSlot(cardSlot);
    }

    function Response_GameCreate(data) // 206 CREATE // 220 RECOVER
    {
        // Create the Player and Opponent
        that.engine.player = new Player(that.engine);
        that.engine.opponent = new Opponent(that.engine);
        that.engine.ProcessList.push(that.engine.player);
        //that.engine.ProcessList.push(that.engine.opponent);

        that.engine.gameId = data.Payload.gameId;
        that.engine.player.playerNr = data.Payload.ident;
        that.engine.player.SetText(data.Payload.player.info);
        that.engine.opponent.SetText(data.Payload.opponent.info);

        // Give cards to hand
        that.engine.player.cardManager.JSONToHandCards(data.Payload.player.hand, {
            x: 165,
            y: 980
        });

        that.engine.opponent.cardManager.JSONToHandCards(data.Payload.opponent.hand, {
            x: 165,
            y: 200
        });

        that.engine.player.cardManager.JSONToBoardCards(data.Payload.player.board);
        that.engine.opponent.cardManager.JSONToBoardCards(data.Payload.opponent.board);


        // Create Chat room (If you are player 1)
        if (data.Payload.ident == 1 && data.Payload.create) {
            that.engine.chatService.RequestNewGameRoom();
        }


        that.engine.background.endTurnButton.SetPlayerTurn(data.Payload.yourTurn);

        // Set Group Visibility to visible
        that.engine.getGroup("Player").visible = true;
        that.engine.getGroup("Opponent").visible = true;
        that.engine.getGroup("EndTurn").visible = true;

        // Fade out the Queue overlay
        that.engine.queue.FadeOut();

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

    function Response_GameRelease(json) // 210 GAME_OPPONENT_RELEASE // 212 GAME_PLAYER_RELEASE
    {
        var card;
        if (json.Type == 210) {
            card = that.engine.opponent.cardManager.hand[json.Payload.cid];
        }
        else if (json.Type == 212) {
            card = that.engine.player.cardManager.hand[json.Payload.cid];
        }

        card.Animation.MoveBack(card);
    }

    function Repsonse_NotYourTurn(json) // 202 NOT YOUR TURN
    {
        that.engine.ScreenMessage(["Not your turn!"], false);


    }
    function Response_NextTurn(json) // 226 NEXT TURN
    {
        if (json.Payload.yourTurn) {
            that.engine.background.endTurnButton.Spin();
        }

        // Update player and opponent portrait
        that.engine.opponent.SetText(json.Payload.opponentInfo);
        that.engine.player.SetText(json.Payload.playerInfo);

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

    GameService.prototype.Request_UseCard = function (card) {
        // If the card is hovering a slot AND that its NOT in a slot
        if (!!card.hoverSlot && !card.inSlot) {
            var json = this.message.GAME.USECARD;
            json.Payload.cid = card.cid;
            json.Payload.slotId = card.hoverSlot.slotId;
            this.socket.send(json);
        }
    }

    /// <summary>
    /// Sends a request to the server with CID of the sourceCard and the targetCard of an attack
    /// </summary>
    /// <param name="sourceCid">The source card (defender) (cid)</param>
    /// <param name="targetCid">The target card (attacker) (cid)</param>    
    GameService.prototype.Request_Attack = function (source, target, type) {

        var json = this.message.GAME.ATTACK;
        json.Payload.type = type;

        if (type == 0) // Card on Card
        {
            json.Payload.source = source.cid
            json.Payload.target = target.cid;

        }
        else if (type == 1) // Card on Hero
        {
            json.Payload.source = source.cid
            json.Payload.target = -1; //TODO?
        }
        else if (type == 2) // Hero on Card
        {

        }
        else {
            console.log("wrong type!");
            return; // ERROR!
        }

        this.socket.send(json);
    };

    /// <summary>
    /// Request a move action to the server.
    /// </summary>
    GameService.prototype.RequestMove = function (card) {
        var json = Message.GAME.MOVE_CARD;
        json.Payload.x = card.x;
        json.Payload.y = this.engine.conf.height - card.y + (card.height / 4);
        json.Payload.cid = card.cid;
        json.Payload.gameId = this.engine.gameId;

        this.engine.gameSocket.send(json);
    }

    /// <summary>
    /// Requests a card release.
    /// </summary>
    GameService.prototype.RequestRelease = function (card) {
        card._awaitRequest = true; // Variables which indicates Waiting response from server
        // If the card is not in a slot AND it is not hovering a slot
        if (!card.inSlot && !card.hoverSlot) {
            var json = Message.GAME.RELEASE_CARD;
            json.Payload.cid = card.cid;
            json.Payload.gameId = card.engine.gameId;
            this.engine.gameSocket.send(json);
        }
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