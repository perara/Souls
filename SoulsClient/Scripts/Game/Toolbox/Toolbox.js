define("toolbox", ["toolbox_rectangle"], function (Rectangle) {

    Toolbox = function () {

        console.log("> Toolbox Loaded!");
    }

    Toolbox.Rectangle = new Rectangle();


    Toolbox.prototype.TweenToPos = function (obj, end, duration) {

        var position =
            {
                x: obj.x,
                y: obj.y
            };

        var target =
            {
                x: end.x,
                y: end.y
            };


        var sween = new TWEEN.Tween(position).to(target, duration);
        sween.easing(TWEEN.Easing.Elastic.Out)

        sween.onUpdate(function () {
            obj.x = position.x;
            obj.y = position.y;
        });

        sween.start();

    }


    return Toolbox;
});