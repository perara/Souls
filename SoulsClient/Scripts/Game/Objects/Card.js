define("card", ['pixi', 'asset', 'stopwatch', 'messages', 'conf'], function (pixi, asset, StopWatch, Message, Conf) {

    var that = this;
    Card = function (engine, jsonData) {
        that = this;
        this.engine = engine;
        var texture = asset.GetTexture(asset.Textures.CARD_NONE);
        pixi.Sprite.call(this, texture);

        // Make card backside
        this.backCard = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_BACK));
        this.backCard.anchor = { x: 0.5, y: 0.5 };
        this.backCard.height = 190;
        this.backCard.width = 110;

        // Make card frontside
        this.frontCard = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_NONE));
        this.frontCard.anchor = { x: 0.5, y: 0.5 };
        this.cardFlipped = false;
        this.addChild(this.frontCard);

        this.anchor = { x: 0.5, y: 0.5 };
        this.position.x = this.position.originX = 0;
        this.position.y = this.position.originY = 0;
        this.order = Card.counter++; // Which number it is ordered in (NOT ID) //TODO? 

        this.width = 120;
        this.height = 150;
        this.networkStopWatch = new Stopwatch();
        this.networkStopWatch.start();

        this.cid = (!!jsonData.cid) ? jsonData.cid : "NA";
        this.name = (!!jsonData.name) ? jsonData.name : "NA";
        this.health = (!!jsonData.health || jsonData.health == 0) ? jsonData.health : "NA";
        this.attack = (!!jsonData.attack || jsonData.attack == 0) ? jsonData.attack : "NA";
        this.cost = (!!jsonData.cost || jsonData.cost == 0) ? jsonData.cost : "NA"; //TODO, 0 might  turn this false?
        this.ability = {
            name: (!!jsonData.ability) ? jsonData.ability.name : "NO"
        }


        this.SetupFrontCard(this);

        this.hoverSlot = undefined;
        this.inSlot = undefined;
        this.pickedUp = false;
        this.owner = undefined;

    };
    // Constructor
    Card.prototype = Object.create(pixi.Sprite.prototype);
    Card.prototype.constructor = Card;

    Card.prototype.SetupFrontCard = function () {
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
        var cHealthText = new pixi.Text(that.health,
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
        var cManaText = new pixi.Text(that.cost, //TODO (COST? != mana)
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
        var cAttackText = new pixi.Text(that.attack,
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
        var cAbilityPanelText = new pixi.Text(this.ability.name,
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
        var cNamePanelText = new pixi.Text(that.name,
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
        that.frontCard.addChild(cBorder);
        // Add background to the card
        that.frontCard.addChild(cBackground);
        // Add abilityPane to the card
        that.frontCard.addChild(cAbilityPanel);


        // Add the image to the portrait container
        that.frontCard.addChild(cPortrait);
        // Add the portrait border to the portrait container
        that.frontCard.addChild(cPortraitBorder);
        // Add Portraits wrapper to the Portrait container.
        that.frontCard.addChild(cPortraitWrapper);

        // Add the Health Image to the card
        that.frontCard.addChild(cHealth);
        // Add the Mana Image to the card
        that.frontCard.addChild(cMana);
        // Add the Attack Image to the card
        that.frontCard.addChild(cAttack);

        // Add the Health text to the CardFactory 
        that.frontCard.addChild(cHealthText);
        // Add the Mana text to the CardFactory 
        that.frontCard.addChild(cManaText);
        // Add the Attack text to the CardFactory 
        that.frontCard.addChild(cAttackText);
        // Add the Ability text to the CardFactory
        that.frontCard.addChild(cAbilityPanelText);

        // Add the Name pane to the CardFactory
        that.frontCard.addChild(cNamePanel);
        // Add the Name text to the CardFactory
        that.frontCard.addChild(cNamePanelText);
    }


    Card.prototype.checkHover = function () {

        // Get cardSlot group
        var cardSlots = this.engine.player.cardManager.cardSlots;

        var hoverSlot;

        for (var index in cardSlots) {
            var cardslot = cardSlots[index];

            // Check if card is hovering a cardSlot
            if ((Toolbox.Rectangle.intersectsYAxis(this, cardslot, { x: -10, y: -15 }, { x: 3, y: 3 }) == true) && !cardslot.card) {
                hoverSlot = index;

                cardslot.doScaling();
            }
            else {
                cardslot.doScaling();
            }
        }

        if (!!hoverSlot) {
            this.hoverSlot = cardSlots[hoverSlot];
            this.hoverSlot.isHovered = true;
        }
        else if (!!this.hoverSlot) {
            this.hoverSlot.isHovered = false;
            this.hoverSlot = undefined;

        }
    }



    Card.prototype.FlipCard = function () {
        if (this.cardFlipped) {
            this.removeChild(this.backCard)
            this.addChild(this.frontCard)
            this.cardFlipped = false;
        }
        else {
            this.removeChild(this.frontCard)
            this.addChild(this.backCard)
            this.cardFlipped = true;
        }
    }

    Card.prototype.ScaleUp = function () {
        this.scale.x = 1.2;
        this.scale.y = 1.2;
    }

    Card.prototype.ScaleDown = function () {
        this.scale.x = 1;
        this.scale.y = 1;
    }


    Card.prototype.Pickup = function () {
        this.ScaleUp();

        this.pickedUp = true;
        this.engine.player.holdingCard = this;

        var position = { rotation: this.rotation };

        asset.GetSound(asset.Sound.CARD_PICKUP).play();
    }

    Card.prototype.PutDown = function () {
        this.ScaleDown();

        this.engine.player.lastHoldingCard = this.engine.player.holdingCard;
        this.engine.player.holdingCard = undefined;

        ///Todo 
        // this.pickedUp = false;
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


    Card.prototype.AnimateBack = function (c) {
        var target =
            {
                x: this.position.originX,
                y: this.position.originY
            }

        this.engine.CreateJS.Tween.get(c, { override: true })
            .to(target, 1000, this.engine.CreateJS.Ease.elasticOut)
            .call(onComplete);

        function onComplete() {
            // Set Mount variable to true
            this.pickedUp = false;
        }
    }

    Card.prototype.OnHoverEffects = function () {
        // Only fire when card is not picked up
        if (!this.pickedUp) { // TODO , can allso apply onHover here to minimalize this function to run. But it might fail?

            // Create a mouse object to test intersection with
            var mouse =
                {
                    x: this.engine.stage.getMousePosition().x + (this.width / 2) - 40,
                    y: this.engine.stage.getMousePosition().y,
                    width: 80,
                    height: this.height
                }


            // Check if the mouse intersects the card 
            var mouseIntersects = Toolbox.Rectangle.intersectsYAxis(this, mouse, { x: 0, y: 0 }, { x: 0, y: 0 });

            // If the mouse intersects
            if (mouseIntersects) {
                // Do antispam
                if (!!this.antiSpam) {


                    // Scale up
                    this.scale = { x: 2.2, y: 2.2 };

                    // Run tween
                    this.engine.CreateJS.Tween.get(this, { override: true })
                        .to({ y: this.position.originY - 100 }, 200, this.engine.CreateJS.Ease.elasticOut)

                    // Reorganize the card order to top
                    this.engine.addChild("Cards", this);

                    // Change state of antispam
                    this.antiSpam = false;
                }

            }
            else {
                // Do antispam
                if (!this.antiSpam) {

                    // Scale down
                    this.ScaleDown();

                    // Run tween
                    this.engine.CreateJS.Tween.get(this, { override: true })
                        .to({ y: this.position.originY }, 500, this.engine.CreateJS.Ease.elasticOut)


                    // Change antispam state
                    this.antiSpam = true;
                }
            } // -- Intersects
        } // -- Picked Up
    } // -- Function


    Card.prototype.Process = function () {

        if (this.dragging && this.networkStopWatch.getElapsed().milliseconds > 200 && !this.inSlot) {
            this.networkStopWatch.reset();
            this.RequestMove();
        }

        if (this.pickedUp) {
            this.checkHover();
        }


        this.OnHoverEffects();


    }

    Card.prototype.RequestMove = function () {
        var json = Message.GAME.MOVE_CARD;
        json.Payload.x = this.x;
        json.Payload.y = Conf.height - this.y + this.height / 1.5;
        json.Payload.cid = this.cid;
        json.Payload.gameId = this.engine.gameId;

        this.engine.gameSocket.send(json);
    }

    Card.prototype.RequestRelease = function () {
        var json = Message.GAME.RELEASE_CARD;
        json.Payload.cid = this.cid;
        json.Payload.gameId = this.engine.gameId;
        this.engine.gameSocket.send(json);
    }



    Card.prototype.mouseover = function (data) {
        this.isOver = true;
    };

    Card.prototype.mouseout = function (data) {
        this.isOver = false;
    }

    Card.prototype.mousedown = Card.prototype.touchstart = function (mouseData) {

        // Pickup the card
        this.Pickup();


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

        this.x = mouse.x;
        this.y = mouse.y;

        this.mouseDown = true;
        this.dragging = true;


        // Only show cardslots on unslotted cards
        if (!this.inSlot) {
            this.engine.getGroup("PlayerCardSlot").visible = true;
        }
    };

    // Mouse - Release
    Card.prototype.mouseup = Card.prototype.mouseupoutside = Card.prototype.touchend = Card.prototype.touchendoutside = function (mouseData) {

        // Put the card down
        this.PutDown();

        this.mouseDown = false;
        this.dragging = false;
        this.engine.getGroup("PlayerCardSlot").visible = false;


        // If the card is not in a slot, we want to tween it back to original position.
        //console.log("");
        if (!this.inSlot && !this.hoverSlot) {
            this.RequestRelease();
        }
        else if (!!this.hoverSlot && !this.inSlot)   // Check if card is over a slot and try to use it on that slot
        {
            this.engine.gameService.Request_UseCard(this.cid, this.hoverSlot.slotId);
        }

    };

    // Dragging Callback
    Card.prototype.mousemove = Card.prototype.touchmove = function (data) {

        if (this.dragging) {
            var mouse = data.getLocalPosition(this.parent);
            if (this.inSlot == null) {

                this.x = mouse.x;
                this.y = mouse.y;

            }
        }
    };

    Card.prototype.PutInSlot = function (cardSlot) {

        cardSlot.card = this;
        this.inSlot = true;
        this.interactive = false;

        var target =
          {
              x: cardSlot.x,
              y: cardSlot.y
          }

        this.engine.CreateJS.Tween.get(this, { override: true })
            .to(target, 500, this.engine.CreateJS.Ease.ElasticInOut)
            .call(onComplete);

        function onComplete() {
            var originalWidth = this.width;

            asset.GetSound(asset.Sound.CARD_MOUNT).play(); // CHANGE ENEMY SOUND

            // Check if the card is owned by the player
            if (this.owner == this.engine.player) {
                this.interactive = true;
                asset.GetSound(asset.Sound.CARD_MOUNT).play();
            }
            else {

                // Flip effect
                var tweenShrink = this.engine.CreateJS.Tween.get(this, { override: true })
                .to({ width: 0 }, 100, this.engine.CreateJS.Ease.ElasticInOut)
                .call(function () { // CHain tweenback

                    this.FlipCard();

                    this.engine.CreateJS.Tween.get(this, { override: true })
                    .to({ width: originalWidth }, 100, this.engine.CreateJS.Ease.ElasticInOut)
                });







            }
        }
    }

    return Card;

});