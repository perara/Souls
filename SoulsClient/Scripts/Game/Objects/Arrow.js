define("arrow", ["jquery", "pixi", "asset"], function ($, pixi, Asset) {

    Arrow = function (engine) {
        this.engine = engine;
        pixi.Graphics.call(this);

        this.filter = new pixi.ColorMatrixFilter();
        this.filters = [this.filter];

        this.originPos = {};
        this.mousePos = {};
        this.active = false;
        this.visible = false;

        var texture = Asset.GetTexture(Asset.Textures.ARROW_CORSHAIR);
        this.arrowHead = new pixi.Sprite(texture)
        this.arrowHead.anchor = { x: 0.2, y: 0.5 };
        this.arrowHead.width = 100;
        this.arrowHead.height = 100;
        this.arrowHead.visible = false;

        var that = this;
        var orginalScale = this.arrowHead.scale;
        var updatus = {
            scaleX: this.arrowHead.scale.x,
            scaleY: this.arrowHead.scale.y,
        };

        this.arrowHeadTween = this.engine.CreateJS.Tween.get(updatus, {
            override: true,
            loop: true,
            onChange: function () {
                that.arrowHead.scale.x = updatus.scaleX;
                that.arrowHead.scale.y = updatus.scaleY;
            }
        })
        .to({
            scaleX: (orginalScale.x * 1.2),
            scaleY: (orginalScale.y * 1.2),
            rotation: 50
        }, 300, this.engine.CreateJS.Ease.quadIn)
        .to({
            scaleX: (orginalScale.x * 1),
            scaleY: (orginalScale.y * 1),
        }, 300, this.engine.CreateJS.Ease.quadIn)
        this.arrowHeadTween.setPaused(true);

        this.Count = 0;
    }
    // Constructor
    Arrow.prototype = Object.create(pixi.Graphics.prototype);
    Arrow.prototype.constructor = Arrow;


    /// <summary>
    /// This function checks if a card is In a slot and then if its picked up. If so it means the player are doing a attack
    /// </summary>
    /// <param name="card">The card</param>
    /// <returns type="">Weither a attack is beeing done.</returns>
    Arrow.prototype.ArrowShowCheck = function (card) {
        // The card must actually exist
        if (!card) return false;

        // If the card is picked up, but is in a slot (Arrow dragging)
        if (!!card.inSlot && !!card.pickedUp) {

            this.Show();
            this.Draw(
                this.engine.conf.mouse, //TO
                card.position //ORIGIN
                );

            return true;
        }
        else {
            this.Reset();
            // Deactivate
            return false;
        }
    }


    Arrow.prototype.PlayerAttackCheck = function (card) {

        var opponent = this.engine.opponent;

        // Determine if the Mouse is inside the card bounds
        var isInside = this.engine.toolbox.Rectangle.containsRaw(
        opponent.x - (opponent.width / 2), // Top
        opponent.y - (opponent.height / 2), // Left
        opponent.width,
        opponent.height,
        this.engine.conf.mouse.x,
        this.engine.conf.mouse.y);

        if (isInside) {
            card.target = opponent;
            opponent.ScaleUp();
            return true;
        }
        else {
            card.target = undefined;
            opponent.ScaleDown();
            return false;
        }
    }


    Arrow.prototype.CardAttackCheck = function (card) {

        // Iterate over opponents cards
        var opponentBoard = this.engine.opponent.cardManager.board;

        for (var index in opponentBoard) {
            var opponentCard = opponentBoard[index];


            // Determine if the Mouse is inside the card bounds
            var isInside = this.engine.toolbox.Rectangle.containsRaw(
            opponentCard.x - (opponentCard.width / 2), // Top
            opponentCard.y - (opponentCard.height / 2), // Left
            opponentCard.width,
            opponentCard.height,
            this.engine.conf.mouse.x,
            this.engine.conf.mouse.y);

            if (isInside) {
                // Assign target on source card to the found opponent card
                card.target = opponentCard;
                opponentCard.ScaleUp();
                return true;
            }
            else {
                // Nothing was found
                opponentCard.ScaleDown();
                card.target = undefined;
                return false;
            }
        }

    }


    Arrow.prototype.Draw = function (mouse, origin) {
        if (mouse.x <= 0 && mouse.y <= 0)
            return;

        this.originPos = origin;
        this.mousePos = mouse;
        this.active = true;


        // Rotate the ArrowHead accordingly to current mouse position.
        var deltaY = mouse.y - origin.y;
        var deltaX = mouse.x - origin.x;
        var angleInDegrees = Math.atan2(deltaY, deltaX) * 180 / Math.PI
        this.arrowHead.rotation = Math.atan2(deltaY, deltaX);
        this.arrowHead.x = mouse.x;
        this.arrowHead.y = mouse.y;

        // Draw the arrow shaft
        this.clear();
        this.lineStyle(15, 0xff0000, 1);
        this.beginFill(0xffFF00, 0.5);
        this.moveTo(origin.x, origin.y);
        this.lineTo(mouse.x, mouse.y);
        this.endFill();
    }

    Arrow.prototype.Show = function () {
        if (!this.visible) {
            this.visible = true;
            this.arrowHead.visible = true;
            this.arrowHeadTween.setPaused(false);
        }
    }

    Arrow.prototype.Hide = function () {
        if (this.visible) {
            this.visible = false;
            this.arrowHead.visible = false;
            this.arrowHeadTween.setPaused(true);
        }
    }

    Arrow.prototype.Reset = function () {
        if (this.active) {
            this.engine.opponent.ScaleDown();

            this.arrowHead.visible = false;
            this.active = false;
            var position = { x: this.mousePos.x, y: this.mousePos.y };
            var target = { x: this.originPos.x, y: this.originPos.y };

            var that = this;
            this.engine.CreateJS.Tween.get(position, {
                override: true, onChange: function () {
                    that.clear();
                    that.lineStyle(20, 0xff0000, 1);
                    that.beginFill(0xffFF00, 0.5);
                    that.moveTo(that.originPos.x, that.originPos.y);
                    that.lineTo(position.x, position.y);
                }
            })
              .to(target, 300, this.engine.CreateJS.Ease.quadIn)

               // Call on complete 
              .call(function () {
                  that.Hide();
              });

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