define("card", ["pixi", 'asset', 'tween'], function (pixi, asset, buzz, tween) {


    Card = function (jsonData) {
        var texture = asset.GetTexture(asset.Textures.CARD_NONE);
        pixi.Sprite.call(this, texture);

        this.anchor = { x: 0.5, y: 0.5 };
        this.position.x = this.position.originX = 0;
        this.position.y = this.position.originY = 0;
        this.order = Card.counter++; // Which number it is ordered in (NOT ID) //TODO? 

        this.width = 120;
        this.height = 150;

        this.CardData = jsonData;

        // TODO? 
        if (jsonData == undefined) {
            this.CardData =
                {
                    name: "Unknown",
                    health: "N",
                    mana: "N",
                    attack: "N",
                    cost: "N",
                    ability:
                    {
                        name: "NO",
                    }
                }
        }

        this.SetupCard(this);
        this.inSlot = null;
        this.hoverSlot = null;

    };
    // Constructor
    Card.prototype = Object.create(pixi.Sprite.prototype);
    Card.prototype.constructor = Card;



    Card.counter = 0; //TODO ??? 
    Card.prototype.SetupCard = function () {
        var that = this;

        // Create the card bound back (Which will ultimately be a border)
        var cBorder = new pixi.Graphics(that);
        cBorder.beginFill(0x000000);
        cBorder.lineStyle(2, 0x000000);
        cBorder.drawRect(0, 0, that.width, that.height);
        cBorder.endFill();
        cBorder.x = -(that.width / 2);
        cBorder.y = -(that.height / 2);

        // CardFactory background
        var cBackground = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_BG));
        cBackground.anchor = { x: 0.5, y: 0.5 };
        cBackground.x = 0;
        cBackground.y = 0;
        cBackground.width = that.width - 5;
        cBackground.height = that.height - 5;

        // AbilityPane
        var cAbilityPanel = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_ABILITY_PANEL));
        cAbilityPanel.anchor = { x: 0.5, y: 0.5 };
        cAbilityPanel.x = 0;
        cAbilityPanel.y = that.height / 4;
        cAbilityPanel.width = that.width - 10;
        cAbilityPanel.height = that.height / 2;


        // CardFactory portrait image
        var cPortrait = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_PORTRAIT));
        cPortrait.anchor = { x: 0.5, y: 0.5 };
        cPortrait.width = (that.width / 2) + 6;
        cPortrait.height = (that.height / 2) + 5;
        cPortrait.x = -1;
        cPortrait.y = -(that.height) / 3 - 3;

        // CardFactory portrait border
        var cPortraitBorder = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_PORTRAIT_BORDER));
        cPortraitBorder.anchor = { x: 0.5, y: 0.5 };
        cPortraitBorder.width = that.width;
        cPortraitBorder.height = that.height;
        cPortraitBorder.x = 0;
        cPortraitBorder.y = -that.height / 3;

        // CardFactory portrait wrapper
        var cPortraitWrapper = new pixi.Graphics();
        cPortraitWrapper.beginFill(0xCECECECE);
        cPortraitWrapper.drawEllipse(0, 0, (that.width / 4) + 4, (that.height / 4) + 9);
        cPortraitWrapper.endFill();
        cPortraitWrapper.x = -1
        cPortraitWrapper.y = -(that.height / 3) + 2

        // Add masking (Image and framebounds)
        cPortrait.mask = cPortraitWrapper


        // CardFactory Health Image
        var cHealth = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_HEALTH));
        cHealth.anchor = { x: 0, y: 0 };
        cHealth.width = that.width / 4;
        cHealth.height = that.height / 4;
        cHealth.x = (that.width / 2) - (cHealth.width / 2);
        cHealth.y = (that.height / 2) - (cHealth.height / 2) - 7;

        // CardFactory Mana Image
        var cMana = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_MANA));
        cMana.anchor = { x: 0.5, y: 0.5 };
        cMana.width = that.width / 3 - 5;
        cMana.height = that.height / 3 - 15;
        cMana.x = -(that.width / 2);
        cMana.y = -(that.height / 2)

        // CardFactory Attack Image
        var cAttack = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_ATTACK));
        cAttack.anchor = { x: 0, y: 0 };
        cAttack.width = that.width / 3 - 10;
        cAttack.height = that.height / 3 - 20;
        cAttack.x = -(that.width / 2) - (cAttack.width / 2);
        cAttack.y = (that.height / 2) - (cAttack.height / 2) - 3;

        // CardFactory Health Label
        var cHealthText = new pixi.Text(that.CardData.health,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cHealthText.anchor = { x: 0, y: 0 };
        cHealthText.position.x = (that.width / 2) - (cHealthText.width / 2);
        cHealthText.position.y = (that.height / 2) - (cHealthText.height / 2);

        // CardFactory Mana Label
        var cManaText = new pixi.Text(that.CardData.cost, //TODO (COST? != mana)
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cManaText.anchor = { x: 0, y: 0 };
        cManaText.position.x = -(that.width / 2) - (cManaText.width / 2);
        cManaText.position.y = -(that.height / 2) - (cManaText.height / 2);;


        // CardFactory Attack Label
        var cAttackText = new pixi.Text(that.CardData.attack,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cAttackText.anchor = { x: 0, y: 0 };
        cAttackText.position.x = -(that.width / 2) - (cAttackText.width / 2);
        cAttackText.position.y = (that.height / 2) - (cAttackText.height / 2) - 2;

        // CardFactory Ability Label
        var cAbilityPanelText = new pixi.Text(this.CardData.ability.name,
             {
                 font: "12px Arial",
                 fill: "black",
                 wordWrap: true,
                 wordWrapWidth: that.width - 24
             });
        cAbilityPanelText.anchor = { x: 0.5, y: 0.5 };
        cAbilityPanelText.position.x = 0;
        cAbilityPanelText.position.y = that.height / 4;

        // CardFactory Name Background
        var cNamePanel = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_NAME_PANEL));
        cNamePanel.anchor = { x: 0.5, y: 1 };
        cNamePanel.x = 0;
        cNamePanel.y = 0;
        cNamePanel.width = that.width;
        cNamePanel.height = (that.height / 5);


        // CardFactory Name Label
        var cNamePanelText = new pixi.Text(that.CardData.name,
            {
                font: "18px Arial bold",
                fill: "black",
                wordWrap: true,
                align: 'center',

                wordWrapWidth: that.width
            });
        cNamePanelText.anchor = { x: 0.5, y: 1 };
        cNamePanelText.x = 0;
        cNamePanelText.y = 0;

        // Add wrapper to the card
        that.addChild(cBorder);
        // Add background to the card
        that.addChild(cBackground);
        // Add abilityPane to the card
        that.addChild(cAbilityPanel);


        // Add the image to the portrait container
        that.addChild(cPortrait);
        // Add the portrait border to the portrait container
        that.addChild(cPortraitBorder);
        // Add Portraits wrapper to the Portrait container.
        that.addChild(cPortraitWrapper);

        // Add the Health Image to the card
        that.addChild(cHealth);
        // Add the Mana Image to the card
        that.addChild(cMana);
        // Add the Attack Image to the card
        that.addChild(cAttack);

        // Add the Health text to the CardFactory 
        that.addChild(cHealthText);
        // Add the Mana text to the CardFactory 
        that.addChild(cManaText);
        // Add the Attack text to the CardFactory 
        that.addChild(cAttackText);
        // Add the Ability text to the CardFactory
        that.addChild(cAbilityPanelText);

        // Add the Name pane to the CardFactory
        that.addChild(cNamePanel);
        // Add the Name text to the CardFactory
        that.addChild(cNamePanelText);

        // TODO SCALE Down cards by 20 (Standard is to big)
        that.scale.x = 0.80;
        that.scale.y = 0.80;
    }


    Card.prototype.putInSlot = function (cardslot) {
        // Check if the cardslot is occupied, if mount the card  into it
        if (cardslot.card.used == false) {
            cardslot.card.hover = false;
            cardslot.card.used = true
            cardslot.card.card = this;

            this.rotation = 0;
            this.inSlot = true;

            var position = { x: this.x, y: this.y };
            var target = { x: cardslot.x, y: cardslot.y };
            var tween = new TWEEN.Tween(position).to(target, 500);
            tween.easing(TWEEN.Easing.Elastic.InOut)

            // TODO           (Tweening onComplete, did not play out as we wanted it to.)
            setTimeout(function () {
                asset.GetSound(asset.Sound.CARD_MOUNT).play();
            }), 1000;

            var that = this;
            tween.onUpdate(function () {
                that.x = position.x;
                that.y = position.y;
            });
            tween.onStart(function () {
                that.interactive = false;
            });
            tween.onComplete(function () {
                that.interactive = true;
                cardslot.visible = false;
            });

            tween.start();

        }

    }

    Card.prototype.Pickup = function () {

        var position = { rotation: this.rotation };
        var target = { rotation: 0 };
        var tween = new TWEEN.Tween(position).to(target, 500);
        tween.easing(TWEEN.Easing.Elastic.Out)

        var that = this;
        tween.onUpdate(function () {
            that.rotation = position.rotation;
        });

        tween.start();
    }

    Card.prototype.OrderLast = function (displaygroup) {
        // Reorder this card in the DisplayContainer (Make it on top)
        displaygroup.removeChild(this)
        displaygroup.addChild(this)
    }

    Card.prototype.OrderFirst = function (displaygroup) {
        displaygroup.removeChild(this)
        displaygroup.addChildAt(this, 0);
    }

    Card.prototype.OrderOriginalPosition = function (displaygroup) {
        displaygroup.removeChild(this)
        displaygroup.addChildAt(this, this.order);
    }


    Card.prototype.AnimateBack = function () {

        var position = { x: this.x, y: this.y, rotation: this.rotation };
        var target = { x: this.originX, y: this.originY, rotation: this.originRot * (Math.PI / 180) };
        var tween = new TWEEN.Tween(position).to(target, 500);
        tween.easing(TWEEN.Easing.Elastic.Out)

        var that = this;
        tween.onUpdate(function () {
            that.rotation = position.rotation;
            that.x = position.x;
            that.y = position.y;
        });

        tween.start();
    }


    Card.prototype.Process = function () {


    }




    // A available callback function which is used for firing events to a parent class when an interaction is done in the Card class
    // This is therfore called at the end of each of the Mouse movement events.
    Card.prototype.InteractionCallback = function (func) { this.InteractionCallback = func }


    Card.prototype.mouseover = function (data) {
        this.mouseOver = true;

        if (!this.inSlot) {
            this.scale.x = 1.0;
            this.scale.y = 1.0
        } else {
            this.scale.x = 1.2;
            this.scale.y = 1.2;
        }

        // Call the GAME callback
        this.InteractionCallback(this, "hover");
    };

    Card.prototype.mouseout = function (data) {
        // Downscale the card when leaving it

        if (!this.inSlot) {
            this.scale.x = 0.80;
            this.scale.y = 0.80;
        }
        else {
            this.scale.x = 1.0;
            this.scale.y = 1.0;
        }

        // Run callback to game
        this.InteractionCallback(this, "leave");
    }


    // Mouse - Click
    Card.prototype.mousedown = Card.prototype.touchstart = function (mouseData) {

        asset.GetSound(asset.Sound.CARD_PICKUP).play()
        // Scale up the card while mousedown (20%)
        this.scale.x = 1.2;
        this.scale.y = 1.2;

        var mouse = mouseData.getLocalPosition(this.parent);

        this.position.click =
            {
                x: mouse.x,
                y: mouse.y,
                offset:
                    {
                        x: (mouse.x - this.position.x),
                        y: (mouse.y - this.position.y)
                    }
            };

        this.mouseDown = true;
        this.dragging = true;

        // Call the GAME callback
        this.InteractionCallback(this, (this.inSlot != null) ? "click-slot" : "click");
    };

    // Mouse - Release
    Card.prototype.mouseup = Card.prototype.mouseupoutside = Card.prototype.touchend = Card.prototype.touchendoutside = function (mouseData) {

        // Scale down the card when mouseup (-20%)
        this.scale.x = 1.0;
        this.scale.y = 1.0;

        this.mouseDown = false;
        this.dragging = false;

        // This property must be forced because PIXI does not update this before after this even is done.
        //this.__mouseIsDown = false;

        // Call the GAME callback
        this.InteractionCallback(this, (this.inSlot != null) ? "release-slot" : "release");
    };

    // Mouse - Move
    Card.prototype.mousemove = Card.prototype.touchmove = function (data) {

        if (this.dragging) {
            var mouse = data.getLocalPosition(this.parent);
            if (this.inSlot == null) {


                // console.log(" x offset is " + this.position.click.offset.x);
                this.position.x = mouse.x - this.position.click.offset.x;
                this.position.y = mouse.y - this.position.click.offset.y;


                // Call the GAME callback
                this.InteractionCallback(this, "drag");
                return;
            }

            // Call the GAME callback (While in slot)
            this.InteractionCallback(this, "drag-slot");
            return;

        }

    };

    return Card;

});