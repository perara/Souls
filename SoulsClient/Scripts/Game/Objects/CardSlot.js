define("cardslot", ["pixi", 'asset'], function (pixi, asset) {

    var num = 0;
    CardSlot = function (x, y) {
        //console.log("> CardSlot Class")
        var texture = asset.GetTexture(asset.Textures.CARD_NONE);
        pixi.Sprite.call(this, texture);

        this.anchor = { x: 0.5, y: 0.5 };
        this.position.x = this.position.originX = x;
        this.position.y = this.position.originY = y;
        this.width = 120;
        this.height = 150;
        this.interactive = true;
        this.slotId = CardSlot.counter++;

       
        // Create the card bound back (Which will ultimately be a border)
        var cBorder = new pixi.Graphics(this);
        cBorder.beginFill(0x000000);
        cBorder.lineStyle(2, 0xC0EC0EE);
        cBorder.fillAlpha = 0.2;
        cBorder.drawRect(0, 0, this.width, this.height);
      
        cBorder.endFill();
        cBorder.x = -(this.width / 2);
        cBorder.y = -(this.height / 2);

        this.addChild(cBorder);


        //////////////////////////////////
        /////Flags which is used//////////
        //////////////////////////////////
        this.card = {
            used: false, // Weither the slot is used or not
            hover: false,  //This flag is set by card when hoverSlot is active.
        }


    };

    // Constructor
    CardSlot.prototype = Object.create(pixi.Sprite.prototype);
    CardSlot.prototype.constructor = CardSlot;

    CardSlot.counter = 0; //TODO ??? 

    CardSlot.prototype.Process = function () {

        this.doScaling();

    }

    CardSlot.prototype.doScaling = function () {

        if (this.isHovering() == true) {
            this.scale.x = 1.20;
            this.scale.y = 1.20;
        }
        else {
            this.scale.x = 1.0;
            this.scale.y = 1.0;

        }

    }



    CardSlot.prototype.isHovering = function () {
        return this.card.hover;
    }



    return CardSlot;

});