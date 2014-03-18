define("background", ["jquery", "asset", "pixi"], function ($, Asset, Pixi) {

    Background = function (engine) {
        console.log("> Background Loaded")
        this.engine = engine;

    }
    // Constructor
    Background.prototype.constructor = Background;

    Background.prototype.Init = function()
    {
        // Setup the Background image
        var bg = new Pixi.Sprite(Asset.GetTexture(Asset.Textures.GAME_BG));
        bg.width = this.engine.conf.width;
        bg.height = this.engine.conf.height;
        bg.interactive = true;
        this.engine.addChild("Background", bg);

        // Make endround button
        var endRound = new Pixi.Sprite(Asset.GetTexture(Asset.Textures.END_ROUND));
        endRound.width = 100;
        endRound.height = 30;
        endRound.x = 800;
        endRound.x = this.engine.conf.height / 2;
        endRound.interactive = true;
        this.engine.addChild("EndRound", endRound);
        console.log(endRound);
    }


    return Background;

});