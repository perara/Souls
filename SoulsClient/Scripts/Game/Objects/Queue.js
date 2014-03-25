define("queue", ["jquery", "asset", "pixi"], function ($, Asset, Pixi) {

    Queue = function (engine) {
        Pixi.Sprite.call(this, Asset.GetTexture(Asset.Textures.GAME_QUEUE));
        console.log("> Queue Loaded")
        this.engine = engine;

        this.width = this.engine.conf.width;
        this.height = this.engine.conf.height;
        this.engine.addChild("Queue", this);


        // CardFactory Name Label
        this.messageText = new Pixi.Text("Connecting...",
            {
                font: "120px Arial bold",
                fill: "white",
                stroke: '#FFFFFF',
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.messageText.anchor = { x: 0.5, y: 0.5 };
        this.messageText.x = this.width / 2;
        this.messageText.y = this.height / 2;

        this.addChild(this.messageText);


    }
    // Constructor
    Queue.prototype = Object.create(Pixi.Sprite.prototype);
    Queue.prototype.constructor = Queue;



    Queue.prototype.SetText = function(text)
    {
        console.log(text);
        var that = this;

        this.engine.CreateJS.Tween.get(this.messageText)
       .to({ alpha: 0 }, 1000, this.engine.CreateJS.Linear)
       .call(function () { that.messageText.setText(text) })
       .to({ alpha: 1 }, 1000, this.engine.CreateJS.Linear)
    }


    Queue.prototype.FadeOut = function () {
        var that = this;

        this.engine.CreateJS.Tween.get(this.messageText)
        .to({ alpha: 0 }, 1000, this.engine.CreateJS.Linear)
        .call(function () { that.messageText.setText("Prepare!") })
        .to({ alpha: 1 }, 1000, this.engine.CreateJS.Linear)
        .call(fadeOut)

        function fadeOut() {
            that.engine.CreateJS.Tween.get(that)
           .wait(2000)
           .to({ alpha: 0 }, 1/*5000*/, that.engine.CreateJS.Linear)
           .call(onComplete)

            function onComplete() {
                // Remove from stage as we wont be needing this anymore.
                this.engine.stage.removeChild(that.engine.getGroup("Queue"));
            }
        }
    }



    return Queue;

});