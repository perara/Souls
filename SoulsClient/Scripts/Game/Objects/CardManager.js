define("cardmanager", ["cardslot", "conf", "toolbox"], function (CardSlot, Conf, Toolbox) {



    CardManager = function (engine, isPlayer) {
        this.engine = engine;
        this.cardSlots = new Object()

        this.hand = new Object();
        this.board = new Object();
        this.isPlayer = isPlayer;


    }

    CardManager.prototype.constructor = CardManager;

    /// <summary>
    /// Adds a card to the hand
    /// </summary>
    /// <param name="c">The card</param>
    CardManager.prototype.AddCardHand = function (c) {
        this.hand[c.cid] = c;

        // If this is Opponent we want to start the card flipped
        if (!this.isPlayer) {
            this.hand[c.cid].FlipCard();
        }
    }

    /// <summary>
    /// Adds a card to the board
    /// </summary>
    /// <param name="c">The card</param>
    CardManager.prototype.AddCardBoard = function (c) {
        this.board[c.cid] = c;
    }

    /// <summary>
    /// Function which creates and adds cardsslots to the board
    /// </summary>
    CardManager.prototype.AddCardSlots = function () {

        // Sets the position and set default cardSlot groupname
        var position = { x: (Conf.width / 13), y: 700 }
        var groupName = "CardSlot-Player";

        // If its not a player, it must be a opponent. So set the opponent cardslot group and replace the position
        if (!this.isPlayer) {
            position.y = 420;
            groupName = "CardSlot-Opponent";
        }

        //  Iterate over each of the cardsslots (7)
        for (var i = 0; i < 7; i++) {
            // Create a new Slot and place it in the array (made in constructor)
            this.cardSlots[i] = new CardSlot(520 + position.x * i, position.y, i);

            // Add the cardslot into the group
            this.engine.addChild(groupName, this.cardSlots[i]);
        }

    }

    /// <summary>
    /// This function parses JSON and creates a Card which is placed into the board
    /// </summary>
    /// <param name="jsonCards">JSON object retrieved from GameSerivce's reply from server</param>
    /// <param name="conf">a Confiuration array with position</param>
    CardManager.prototype.JSONToBoardCards = function (jsonCards, conf) {

        for (var cJson in jsonCards) {

            var position =
                {
                    x: this.cardSlots[jsonCards[cJson].slotId].x,
                    y: this.cardSlots[jsonCards[cJson].slotId].y,
                    rotation: 0
                };


            var card = this.CreateCard(jsonCards[cJson], position);

            card.inSlot = true;

            // CARD
            this.AddCardBoard(card);
        }


    }

    /// <summary>
    /// This function parses JSON and creates a Card which is placed into the hand
    /// </summary>
    /// <param name="jsonCards">JSON object retrieved from GameSerivce's reply from server</param>
    /// <param name="conf">a Confiuration array with position</param>
    CardManager.prototype.JSONToHandCards = function (jsonCards, conf) {

        var count = 0;
        // Iterate over cards
        for (var cJson in jsonCards) {
            count++;

            var position = {
                x: conf.x + (50 * (count)),
                y: conf.y,
                rotation: 0
            };

            // Create a card
            var card = this.CreateCard(jsonCards[cJson], position);


            this.AddCardHand(card);
        }
    }

    /// <summary>
    /// Creates a card
    /// </summary>
    /// <param name="jsonCard">Contains the JSON data retrieved from the server</param>
    /// <param name="position">Should contain: X position, Y position and ROTATION</param>
    /// <param name="owner">either "Player" or "Opponent"</param>
    /// <returns type=""></returns>
    CardManager.prototype.CreateCard = function (jsonCard, position) {

        var c = new Card(this.engine, jsonCard);
        c.x = c.position.originX = position.x;
        c.y = c.position.originY = position.y;
        c.originRot = position.rotation;

        if (this.isPlayer) {
            c.interactive = true;
            c.owner = this.engine.player;
            this.engine.addChild("Card-Player", c);
        }
        else if (!this.isPlayer) {
            c.interactive = false;
            c.owner = this.engine.opponent;
            this.engine.addChild("Card-Opponent", c);
        }
        else {
            console.log("Error, a non existing ''playoropp''");
        }

        return c;
    }

    return CardManager;
});