define("endturnbutton", ["jquery", "asset", "pixi", "conf", "messages"], function ($, Asset, Pixi, Conf, Message) {

    EndTurnButton = function (engine) {
        this.engine = engine;
        console.log("> EndTurnButton Loaded")

        // Cache images for undelayed swap
        this.disabledTexture = Asset.GetTexture(Asset.Textures.END_TURN_DISABLED);
        this.enabledTexture = Asset.GetTexture(Asset.Textures.END_TURN);

        // Enabled is default texture
        Pixi.Sprite.call(this, this.enabledTexture);

    }

    EndTurnButton.prototype = Object.create(Pixi.Sprite.prototype);

    // Constructor
    EndTurnButton.prototype.constructor = EndTurnButton;

    EndTurnButton.prototype.Init = function (active) {
        // Make endround button
        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 200;
        this.height = 75;
        this.x = Conf.width - 310;
        this.y = (Conf.height / 2) + 10;
        this.active = active;
        this.active = true;
        this.swapTexture = undefined;
    }


    EndTurnButton.prototype.SwapTexture = function () {
        var tmpTexture = this.swapTexture;
        this.swapTexture = this.texture;
        this.setTexture(tmpTexture);
    }

    EndTurnButton.prototype.SetPlayerTurn = function (active) {
        if (active) {
            this.setTexture(this.enabledTexture);
            this.swapTexture = this.disabledTexture;
            this.interactive = true;
        }
        else {
            this.setTexture(this.disabledTexture);
            this.swapTexture = this.enabledTexture;
            this.interactive = false;
        }
    }

    // Spins the button and activates\deactivated it depending on input
    EndTurnButton.prototype.Spin = function () {

        var originalHeight = this.height;

        Asset.GetSound(Asset.Sound.END_TURN).play();

        var that = this;
        var positive = this.scale.y;
        var position = { scaleY: this.scale.y }
        this.interactive = (this.interactive) ? false : true;
        this.engine.CreateJS.Tween.get(position, { onChange: onChange })
            .to({ scaleY: this.scale.y * (-1) }, 100)
            .to({ scaleY: positive }, 100)
            .to({ scaleY: this.scale.y * (-1) }, 100)
            .to({ scaleY: positive }, 100)
            .to({ scaleY: this.scale.y * (-1) }, 100)
            .to({ scaleY: positive }, 100)
            .to({ scaleY: this.scale.y * (-1) }, 100)
            .to({ scaleY: positive }, 100)
            .call(function () {
                that.SwapTexture();
            })

        function onChange(e) {
            that.scale.y = position.scaleY;
        }
    }

    // When endturn sign is pushed and interactive
    EndTurnButton.prototype.mousedown = EndTurnButton.prototype.touchstart = function () {

        // TODO Send endturn request to server
        // Run this if endturn response is OK

        this.Spin();
        this.RequestNextTurn();
    }

    EndTurnButton.prototype.RequestNextTurn = function () {
        var json = Message.GAME.NEXT_TURN;
        this.engine.gameSocket.send(json);
        console.log("Sendt endturn request");
    }

    return EndTurnButton;

});