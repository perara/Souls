define("player", ["jquery", "pixi", "asset", "playerbase", "arrow", "cardmanager"], function ($, pixi, asset, playerBase, Arrow, CardManager) {


    Player = function (engine) {
        var texture = asset.GetTexture(asset.Textures.PLAYER_NONE);
        playerBase.call(this, texture, engine)

        this.isPlayer = true;
        this.holdingCard; // Currently holding card (Object)
        this.lastHoldingCard; // The last card the player held

    }
    // Constructor
    Player.prototype = Object.create(playerBase.prototype);
    Player.prototype.constructor = Player;


    Player.prototype.Process = function () {
        this.arrow.Process();

        this.cardManager.Process();



        this.arrow.AttackCheck(this.holdingCard);

    }



    Player.prototype.Init = function () {
        this.arrow = new Arrow(this.engine);
        this.engine.addChild("Player", this);

        this.engine.addChild("Player", this.arrow);
        this.engine.getGroup("Player").addChild(this.arrow.arrowHead);

        this.cardManager = new CardManager(this.engine, true);
        this.cardManager.AddCardSlots();
        this.engine.getGroup("CardSlot-Player").visible = false;
        this.engine.getGroup("CardSlot-Player").alpha = 0.7;

        this.SetPosition(
            {
                x: (this.engine.conf.width / 2),
                y: (this.engine.conf.height - this.height / 2)
            });
    }
    return Player;

});