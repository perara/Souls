define("arrow", ["jquery", "pixi"], function ($, pixi) {

    Arrow = function (engine) {
        this.engine = engine;
        pixi.Graphics.call(this);

        this.filter = new pixi.ColorMatrixFilter();
        this.filters = [this.filter];

        this.originPos = {};
        this.mousePos = {};
        this.active = false;

        this.Count = 0;
    }
    // Constructor
    Arrow.prototype = Object.create(pixi.Graphics.prototype);
    Arrow.prototype.constructor = Arrow;

    Arrow.prototype.AttackCheck = function (card) {
        // The card must actually exist
        if (!!card) {

            // If the card is picked up, but is in a slot (Arrow dragging)
            if (card.inSlot && card.pickedUp) {

                this.Show();
                this.Draw(
                    this.engine.conf.mouse, //TO
                    card.position //ORIGIN
                    );

                // Check if the arrow is actually active (Is moving)
                if (this.active) {

                    // Iterate over opponents cards
                    var opponentBoard = this.engine.opponent.cardManager.board;
                    for (var index in opponentBoard) {
                        var oppCard = opponentBoard[index];


                        // Check if the mouse is inside the card
                        var isInside = this.engine.toolbox.Rectangle.containsRaw(
                        oppCard.x - (oppCard.width / 2), //Top
                        oppCard.y - (oppCard.height / 2), //LEFT
                        oppCard.width,
                        oppCard.height,
                        this.engine.conf.mouse.x,
                        this.engine.conf.mouse.y);

                        // Was it inside?
                        if (isInside) {
                            // Set card to attack IF mouse is released
                            card.attackCard = oppCard;
                            oppCard.ScaleUp();
                        }
                        else {
                            oppCard.ScaleDown();
                            card.attackCard = undefined;
                        }
                    }
                }
            }
        }
    }


    Arrow.prototype.Draw = function (mouse, origin) {
        if (mouse.x <= 0 && mouse.y <= 0)
            return;

        this.originPos = origin;
        this.mousePos = mouse;
        this.active = true;

        this.clear();
        this.lineStyle(20, 0xff0000, 1);
        this.beginFill(0xffFF00, 0.5);
        this.moveTo(origin.x, origin.y);
        this.lineTo(mouse.x, mouse.y);
    }

    Arrow.prototype.Show = function () {
        this.visible = true;
    }

    Arrow.prototype.Hide = function () {
        this.visible = false;
    }

    Arrow.prototype.Reset = function () {
        this.active = false;
        var position = { x: this.mousePos.x, y: this.mousePos.y };
        var target = { x: this.originPos.x, y: this.originPos.y };


        this.engine.CreateJS.Tween.get(position, { override: true, onChange: onUpdate })
          .to(target, 300, this.engine.CreateJS.Ease.quadIn)
          .call(onComplete);

        var that = this;
        function onUpdate() {
            that.clear();
            that.lineStyle(20, 0xff0000, 1);
            that.beginFill(0xffFF00, 0.5);
            that.moveTo(that.originPos.x, that.originPos.y);
            that.lineTo(position.x, position.y);

        }

        function onComplete() {
            that.Hide();
        }
    }

    Arrow.prototype.Process = function () {
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

    return Arrow;

});