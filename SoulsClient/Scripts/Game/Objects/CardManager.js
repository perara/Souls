define("cardmanager", ["cardslot", "conf"], function (CardSlot, Conf) {

    

    CardManager = function (engine) {
        this.holdingCard = null; //Should be a ID of the card
        this.hand = new Object();
        this.board = new Object();

        this.engine = engine;
        console.log(this.engine);
        this.cardSlots = new Array();
        this.AddCardSlots();
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

    CardManager.prototype.AddCardSlots = function () {
        console.log(this);
        for (var i = 0; i < 7; i++) {
            this.cardSlots[i] = new CardSlot(520 + (Conf.width / 13) * i, 700);
            this.engine.addChild("CardSlot", this.cardSlots[i]);
        }
    }

    return CardManager;

});