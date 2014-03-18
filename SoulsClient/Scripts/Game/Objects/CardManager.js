define("cardmanager", ["cardslot", "conf", "toolbox"], function (CardSlot, Conf, Toolbox) {



    CardManager = function (engine, isPlayer) {
        this.engine = engine;
        this.cardSlots = new Object()

        this.hand = new Object();
        this.board = new Object();
        this.isPlayer = isPlayer;

        //this.cardOverview = cardOverView; // Static list with all cards

    }
    // Constructor
    CardManager.prototype.constructor = CardManager;


    CardManager.prototype.ToBoard = function (cid, slotid) {

    }

    CardManager.prototype.PickupCard = function (cid) {

    }

    CardManager.prototype.AddCardHand = function (c, isPlayer) {
        this.hand[c.cid] = c;
        if (!this.isPlayer) {
            this.hand[c.cid].FlipCard();
        }
    }

    CardManager.prototype.AddCardBoard = function (c) {
        this.board[c.cid] = c;
        this.board[c.cid].FlipCard();
    }

    CardManager.prototype.AddCardSlots = function () {

        var position = { x: (Conf.width / 13), y: 700 }
        var groupName = "PlayerCardSlot";

        if (!this.isPlayer) {
            position.y = 420;
            groupName = "OpponentCardSlot";
        }

        for (var i = 0; i < 7; i++) {
            this.cardSlots[i] = new CardSlot(520 + position.x * i, position.y, i);
            this.engine.addChild(groupName, this.cardSlots[i]);
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
                c.owner = this.engine.player;
            }
            else if(conf.playoropp == "Opponent") {
                c.owner = this.engine.opponent;
            }
            else{
                console.log("Error, a non existing ''playoropp''");
            }

        }
    }
   
    return CardManager;
});