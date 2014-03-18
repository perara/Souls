define("endturnbutton", ["jquery", "asset", "pixi", "conf"], function ($, Asset, Pixi, Conf) {

    EndTurnButton = function () {
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

    EndTurnButton.prototype.Init = function () {
        // Make endround button
        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 200;
        this.height = 75;
        this.x = Conf.width - 310;
        this.y = (Conf.height / 2) + 10;
        this.interactive = true;
        this.active = true;
    }
    
    // Spins the button and activates\deactivated it depending on input
    EndTurnButton.prototype.Spin = function (activate) {
       
        var count = 0;
        var spins = 7;
        var that = this;

        that.active = activate;

        var position = {
            
                scaleY: this.scale.y,
            };

        var target = {
            
            scaleY: this.scale.y * (-1),
        };
        
        Asset.GetSound(Asset.Sound.END_TURN).play();

        var tween = new TWEEN.Tween(position).to(target, 100).onUpdate(onUpdate).onComplete(onComplete).onStart(onStart);
        var tweenBack = new TWEEN.Tween(position).to({ scaleY: this.scale.y }, 100).onUpdate(onUpdate).onComplete(onComplete);

        tween.easing(TWEEN.Easing.Linear.None);
        tweenBack.easing(TWEEN.Easing.Linear.None);

        tween.chain(tweenBack);
        tweenBack.chain(tween);
 
        function onUpdate () {
            that.scale.y = position.scaleY;
        }

        function onStart() {
            that.interactive = false;
            
        }

        function onComplete () {
            if (count++ == spins) {
                Finished();
            }
        }

        // Run when all tweens are completed
        function Finished() {
            tween.stop();

            if (that.active) {
                that.setTexture(that.enabledTexture);
                that.interactive = true;
            }
            else {
                that.setTexture(that.disabledTexture);
            }
        }
        tween.start();

        // Activation or deactivation of the button?
        
    }

    // When endturn sign is pushed and interactive
    EndTurnButton.prototype.mousedown = EndTurnButton.prototype.touchstart = function () {

        // TODO Send endturn request to server
        // Run this if endturn response is OK

        this.Spin(false);

        // Test
        var that = this;
        setTimeout(function () { that.Spin(true) }, 3000);

    }

    return new EndTurnButton();

});