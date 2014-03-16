define("cardmanager", [], function ($, pixi, asset, playerBase, Arrow) {

    CardManager = function (engine) {
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





    return CardManager;

});