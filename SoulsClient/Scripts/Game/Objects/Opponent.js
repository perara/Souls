define("opponent", ["jquery", "asset", "playerbase"], function ($, asset, playerBase) {


    Opponent = function (engine) {
        var texture = asset.GetTexture(asset.Textures.PLAYER_NONE);
        playerBase.call(this, texture, engine)



    }
    // Constructor
    Opponent.prototype = Object.create(PlayerBase.prototype);
    Opponent.prototype.constructor = Opponent;

    Opponent.prototype.Init = function () {
        this.engine.addChild("Opponent", this);
        this.cardManager = new CardManager(this.engine);
        this.cardManager.AddCardSlots(false);
        this.SetPosition(
            {
                x: (this.engine.conf.width / 2),
                y: this.height / 2
            });

    }





    return Opponent;

});