define("cardmanager", ["cardslot", "conf", "toolbox"], function (CardSlot, Conf, Toolbox) {



    CardManager = function (engine) {
        this.engine = engine;
        this.cardSlots = new Object()

        this.holdingCard = null; //Should be a ID of the card
        this.hand = new Object();
        this.board = new Object();

        //this.cardOverview = cardOverView; // Static list with all cards

    }
    // Constructor
    CardManager.prototype.constructor = CardManager;


    CardManager.prototype.ToBoard = function (cid, slotid) {

    }

    CardManager.prototype.PickupCard = function (cid) {

    }

    CardManager.prototype.AddCardHand = function (c) {
        this.hand[c.cid] = c;
    }

    CardManager.prototype.AddCardBoard = function (c) {
        this.board[c.cid] = c;
    }

    CardManager.prototype.AddCardSlots = function (isPlayer) {

        var position = { x: (Conf.width / 13), y: 700 }

        if (!isPlayer) position.y = 420;

        for (var i = 0; i < 7; i++) {
            this.cardSlots[i] = new CardSlot(520 + position.x * i, position.y, i);
            this.engine.addChild("CardSlot", this.cardSlots[i]);
        }
        
    }

    CardManager.prototype.GiveCards = function (jsonCards, conf) {

        var count = 0;
        // Iterate over cards
        for (var cJson in jsonCards) {
            count++;
            var c = new Card(this.engine, jsonCards[cJson]);
            c.x = c.originX = conf.x + (120 * (count));
            c.y = c.originY = conf.y;
            c.originRot = 0;
            this.AddCardHand(c);


            this.engine.addChild(conf.playoropp, c);

            if (conf.playoropp == "Player") {
                c.interactive = true;
            }

        }
    }

    CardManager.prototype.checkHover = function () {
        var hoverSlot;

        for (var index in this.cardSlots) {
            var cardslot = this.cardSlots[index];

            // Check if card is hovering a cardSlot
            if ((Toolbox.Rectangle.intersectsYAxis(this.engine.player.currentCard, cardslot, { x: -10, y: -15 }, { x: 3, y: 3 }) == true) && cardslot.used == false) {
                cardslot.isHoverd = true;
                hoverSlot = index;
                cardslot.doScaling();
            }
            else {
                cardslot.isHoverd = false;
                cardslot.doScaling();
            }
        }
        this.engine.player.currentCard.hoverSlot = this.cardSlots[hoverSlot];
        //console.log(index);
    }

    

    return CardManager;
});