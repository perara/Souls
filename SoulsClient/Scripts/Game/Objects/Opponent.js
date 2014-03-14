define("opponent", ["jquery", "asset", "playerbase"], function ($, asset, playerBase) {


    Opponent = function (jsonObject) {
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
    Opponent.prototype = Object.create(PlayerBase.prototype);
    Opponent.prototype.constructor = Opponent;


 




    return Opponent;

});