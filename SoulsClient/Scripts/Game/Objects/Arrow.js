define("arrow", ["jquery", "pixi"], function ($, pixi) {

    Arrow = function()
    {
        pixi.Graphics.call(this);
        this.filter = new pixi.ColorMatrixFilter();
        this.Arrow = undefined;
  
    }
    // Constructor
    Arrow.prototype = Object.create(pixi.Sprite.prototype);
    Arrow.prototype.constructor = Arrow;


    // Setups the arrow and returns it.
    Arrow.prototype.SetupArrow = function () {
        var Arrow = new pixi.Graphics();
        Arrow.origin = { x: 0, y: 0 };
        Arrow.lastpos = { x: 0, y: 0 };
        Arrow.lineStyle(20, 0x33FF00);
        Arrow.filters = [this.filter];

        this.Arrow = Arrow;
        return Arrow;
    }

    Arrow.prototype.Draw = function (mouse, origin) {
        this.Arrow.origin = origin;
        this.Arrow.lastpos = { x: mouse.x, y: mouse.y };

        this.Arrow.clear();
        this.Arrow.lineStyle(20, 0xff0000, 1);
        this.Arrow.beginFill(0xffFF00, 0.5);
        this.Arrow.moveTo(origin.x, origin.y);
        this.Arrow.lineTo(mouse.x, mouse.y);

    }

    Arrow.prototype.Show = function () {
        this.Arrow.visible = true;
        this.Arrow.clear();

        if (!!this.Tweens.hideArrowTween) // Ensure that this tween actually exists
            this.Tweens.hideArrowTween.stop();
    }

    Arrow.prototype.Hide = function () {
        //        this.Arrow.visible = false;

        var position = { x: this.Arrow.lastpos.x, y: this.Arrow.lastpos.y };
        var target = { x: this.Arrow.origin.x, y: this.Arrow.origin.y };
        var tween = new TWEEN.Tween(position).to(target, 1000);

        this.Tweens.hideArrowTween = tween;
        tween.easing(TWEEN.Easing.Exponential.Out); // THIS DETERMINES THE TWEEN

        var that = this;
        tween.onUpdate(function () {
            that.Arrow.clear();
            that.Arrow.lineStyle(20, 0xff0000, 1);
            that.Arrow.beginFill(0xffFF00, 0.5);
            that.Arrow.moveTo(that.Arrow.origin.x, that.Arrow.origin.y);
            that.Arrow.lineTo(position.x, position.y);
        });
        tween.onComplete(function () {
            that.Arrow.visible = false;
        });

        tween.start();

    }


    Arrow.prototype.Process = function () {
        if (!!this.Arrow) {

            var count = this.Count += 0.1


            var colorMatrix = [1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                0, 0, 0, 1];

            colorMatrix[1] = Math.sin(count) * 3;
            colorMatrix[2] = Math.cos(count);
            colorMatrix[3] = Math.cos(count) * 1.5;
            colorMatrix[4] = Math.sin(count / 3) * 2;
            colorMatrix[5] = Math.sin(count / 2);
            colorMatrix[6] = Math.sin(count / 4);
            this.filter.matrix = colorMatrix;


        }
    }

    return Arrow;

});