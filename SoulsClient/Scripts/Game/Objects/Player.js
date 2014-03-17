define("player", ["jquery", "pixi", "asset", "playerbase", "arrow", "cardmanager"], function ($, pixi, asset, playerBase, Arrow, CardManager) {


    Player = function (engine) {
        var texture = asset.GetTexture(asset.Textures.PLAYER_NONE);
        playerBase.call(this, texture, engine)
        this.currentCard = undefined;
    }
    // Constructor
    Player.prototype = Object.create(playerBase.prototype);
    Player.prototype.constructor = Player;


    Player.prototype.Process = function () {
        this.arrow.Process();

        // Process Hand Cards
        for (var cardIndex in this.cardManager.hand) {
            this.cardManager.hand[cardIndex].Process();
        }

        // Process Board Cards
        for (var cardIndex in this.cardManager.board) {
            this.cardManager.board[cardIndex].Process();
        }
        
        if (!!this.currentCard) {
            this.cardManager.checkHover();
        }
        
    }

    Player.prototype.Init = function () {
        this.arrow = new Arrow();
        this.engine.addChild("Player", this);
        this.cardManager = new CardManager(this.engine);
        this.cardManager.AddCardSlots();
        this.engine.getGroup("CardSlot").visible = false;
        this.engine.getGroup("CardSlot").alpha = 1;
        this.SetPosition(
            {
                x: (this.engine.conf.width / 2),
                y: (this.engine.conf.height - this.height / 2)
            });

    }

    return Player;

});