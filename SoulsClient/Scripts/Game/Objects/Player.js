define("player", ["jquery", "pixi", "asset", "playerbase", "arrow"], function ($, pixi, asset, playerBase, Arrow) {


    Player = function (jsonObject) {
        var texture = asset.GetTexture(asset.Textures.PLAYER_NONE);
        PlayerBase.call(this, texture, jsonObject)

        this.pickedUpCard = undefined;

        this.mouse =
        {
            x: 0,
            y: 0
        }

        this.Arrow = new Arrow();
        


    }
    // Constructor
    Player.prototype = Object.create(PlayerBase.prototype);
    Player.prototype.constructor = Player;


    Player.prototype.Process = function () {
        this.ProcessArrow();
    }





    return Player;

});