define("card", ['pixi', 'asset', 'stopwatch', 'messages', 'conf'], function (pixi, asset, StopWatch, Message, Conf) {

    var that = this;

    Card = function (engine, jsonData) {
        var texture = asset.GetTexture(asset.Textures.CARD_NONE);
        pixi.Sprite.call(this, texture);

        // Engine reference
        this.engine = engine;

        // Create a global tunnel to "this" object
        that = this;
      
        // Card Dimensions
        this.width = 120;
        this.height = 150;
        this.anchor = { x: 0.5, y: 0.5 };

        // Card Position
        this.position.x = this.position.originX = 0;
        this.position.y = this.position.originY = 0;
        this.order = Card.counter++; // Which number it is ordered in (NOT ID) //TODO? 
        console.log(this.order);
        /// <summary>
        /// Cusom Variables
        /// </summary>
        /// Data which is received from server
        this.cid = (!!jsonData.cid) ? jsonData.cid : "NA";
        this.name = (!!jsonData.name) ? jsonData.name : "NA";
        this.health = (!!jsonData.health || jsonData.health == 0) ? jsonData.health : "NA";
        this.attack = (!!jsonData.attack || jsonData.attack == 0) ? jsonData.attack : "NA";
        this.cost = (!!jsonData.cost || jsonData.cost == 0) ? jsonData.cost : "NA"; //TODO, 0 might  turn this false?
        this.ability = {
            name: (!!jsonData.ability) ? jsonData.ability.name : "NO"
        }

        // Create a stopwatch for network pulse (Movement specifically)
        this.networkStopWatch = new Stopwatch();
        this.networkStopWatch.start();

        // Card definitions
        this.backCard = undefined;
        this.frontCard = undefined;

        // Interaction definitions
        this.hoverSlot = undefined;
        this.inSlot = undefined;
        this.owner = undefined;
        this.pickedUp = undefined;
        this.cardFlipped = false;

        // Setup the card layout / graphics
        this.SetupFrontCard(this);
        this.SetupBackCard(this);

    };
    // Global order counter
    Card.counter = 0;

    // Constructor
    Card.prototype = Object.create(pixi.Sprite.prototype);
    Card.prototype.constructor = Card;

    Card.prototype.SetupBackCard = function()
    {
        // Make card backside
        this.backCard = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_BACK));
        this.backCard.anchor = { x: 0.5, y: 0.5 };
        this.backCard.height = 190;
        this.backCard.width = 110;
    }

    Card.prototype.SetupFrontCard = function () {
        // Make card frontside
        this.frontCard = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_NONE));
        this.frontCard.anchor = { x: 0.5, y: 0.5 };
        this.addChild(this.frontCard);

        // Create the card bound back (Which will ultimately be a border)
        var cBorder = new pixi.Graphics(this);
        cBorder.beginFill(0x000000);
        cBorder.lineStyle(2, 0x000000);
        cBorder.drawRect(0, 0, this.width, this.height);
        cBorder.endFill();
        cBorder.x = -(this.width / 2);
        cBorder.y = -(this.height / 2);

        // CardFactory background
        var cBackground = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_BG));
        cBackground.anchor = { x: 0.5, y: 0.5 };
        cBackground.x = 0;
        cBackground.y = 0;
        cBackground.width = this.width - 5;
        cBackground.height = this.height - 5;

        // AbilityPane
        var cAbilityPanel = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_ABILITY_PANEL));
        cAbilityPanel.anchor = { x: 0.5, y: 0.5 };
        cAbilityPanel.x = 0;
        cAbilityPanel.y = this.height / 4;
        cAbilityPanel.width = this.width - 10;
        cAbilityPanel.height = this.height / 2;


        // CardFactory portrait image
        var cPortrait = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_PORTRAIT));
        cPortrait.anchor = { x: 0.5, y: 0.5 };
        cPortrait.width = (this.width / 2) + 6;
        cPortrait.height = (this.height / 2) + 5;
        cPortrait.x = -1;
        cPortrait.y = -(this.height) / 3 - 3;

        // CardFactory portrait border
        var cPortraitBorder = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_PORTRAIT_BORDER));
        cPortraitBorder.anchor = { x: 0.5, y: 0.5 };
        cPortraitBorder.width = this.width;
        cPortraitBorder.height = this.height;
        cPortraitBorder.x = 0;
        cPortraitBorder.y = -this.height / 3;

        // CardFactory portrait wrapper
        var cPortraitWrapper = new pixi.Graphics();
        cPortraitWrapper.beginFill(0xCECECECE);
        cPortraitWrapper.drawEllipse(0, 0, (this.width / 4) + 4, (this.height / 4) + 9);
        cPortraitWrapper.endFill();
        cPortraitWrapper.x = -1
        cPortraitWrapper.y = -(this.height / 3) + 2

        // Add masking (Image and framebounds)
        cPortrait.mask = cPortraitWrapper


        // CardFactory Health Image
        var cHealth = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_HEALTH));
        cHealth.anchor = { x: 0, y: 0 };
        cHealth.width = this.width / 4;
        cHealth.height = this.height / 4;
        cHealth.x = (this.width / 2) - (cHealth.width / 2);
        cHealth.y = (this.height / 2) - (cHealth.height / 2) - 7;

        // CardFactory Mana Image
        var cMana = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_MANA));
        cMana.anchor = { x: 0.5, y: 0.5 };
        cMana.width = this.width / 3 - 5;
        cMana.height = this.height / 3 - 15;
        cMana.x = -(this.width / 2);
        cMana.y = -(this.height / 2)

        // CardFactory Attack Image
        var cAttack = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_ATTACK));
        cAttack.anchor = { x: 0, y: 0 };
        cAttack.width = this.width / 3 - 10;
        cAttack.height = this.height / 3 - 20;
        cAttack.x = -(this.width / 2) - (cAttack.width / 2);
        cAttack.y = (this.height / 2) - (cAttack.height / 2) - 3;

        // CardFactory Health Label
        var cHealthText = new pixi.Text(this.health,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cHealthText.anchor = { x: 0, y: 0 };
        cHealthText.position.x = (this.width / 2) - (cHealthText.width / 2);
        cHealthText.position.y = (this.height / 2) - (cHealthText.height / 2);

        // CardFactory Mana Label
        var cManaText = new pixi.Text(this.cost, //TODO (COST? != mana)
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cManaText.anchor = { x: 0, y: 0 };
        cManaText.position.x = -(this.width / 2) - (cManaText.width / 2);
        cManaText.position.y = -(this.height / 2) - (cManaText.height / 2);;


        // CardFactory Attack Label
        var cAttackText = new pixi.Text(this.attack,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4
            });
        cAttackText.anchor = { x: 0, y: 0 };
        cAttackText.position.x = -(this.width / 2) - (cAttackText.width / 2);
        cAttackText.position.y = (this.height / 2) - (cAttackText.height / 2) - 2;

        // CardFactory Ability Label
        var cAbilityPanelText = new pixi.Text(this.ability.name,
             {
                 font: "12px Arial",
                 fill: "black",
                 wordWrap: true,
                 wordWrapWidth: this.width - 24
             });
        cAbilityPanelText.anchor = { x: 0.5, y: 0.5 };
        cAbilityPanelText.position.x = 0;
        cAbilityPanelText.position.y = this.height / 4;

        // CardFactory Name Background
        var cNamePanel = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_NAME_PANEL));
        cNamePanel.anchor = { x: 0.5, y: 1 };
        cNamePanel.x = 0;
        cNamePanel.y = 0;
        cNamePanel.width = this.width;
        cNamePanel.height = (this.height / 5);


        // CardFactory Name Label
        var cNamePanelText = new pixi.Text(this.name,
            {
                font: "18px Arial bold",
                fill: "black",
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        cNamePanelText.anchor = { x: 0.5, y: 1 };
        cNamePanelText.x = 0;
        cNamePanelText.y = 0;

        // Add wrapper to the card
        this.frontCard.addChild(cBorder);
        // Add background to the card
        this.frontCard.addChild(cBackground);
        // Add abilityPane to the card
        this.frontCard.addChild(cAbilityPanel);


        // Add the image to the portrait container
        this.frontCard.addChild(cPortrait);
        // Add the portrait border to the portrait container
        this.frontCard.addChild(cPortraitBorder);
        // Add Portraits wrapper to the Portrait container.
        this.frontCard.addChild(cPortraitWrapper);

        // Add the Health Image to the card
        this.frontCard.addChild(cHealth);
        // Add the Mana Image to the card
        this.frontCard.addChild(cMana);
        // Add the Attack Image to the card
        this.frontCard.addChild(cAttack);

        // Add the Health text to the CardFactory 
        this.frontCard.addChild(cHealthText);
        // Add the Mana text to the CardFactory 
        this.frontCard.addChild(cManaText);
        // Add the Attack text to the CardFactory 
        this.frontCard.addChild(cAttackText);
        // Add the Ability text to the CardFactory
        this.frontCard.addChild(cAbilityPanelText);

        // Add the Name pane to the CardFactory
        this.frontCard.addChild(cNamePanel);
        // Add the Name text to the CardFactory
        this.frontCard.addChild(cNamePanelText);
    }


    /// <summary>
    /// Checks if the card is hovered over a cardslot (BROKEN) //TODO
    /// </summary>
    Card.prototype.checkHover = function () {

        // Get cardSlot group
        var cardSlots = this.engine.player.cardManager.cardSlots;

        var hoverSlot;

        for (var index in cardSlots) {
            var cardslot = cardSlots[index];


            // Check if card is hovering a cardSlot
            if ((Toolbox.Rectangle.intersectsYAxis(this, cardslot, { x: -10, y: -15 }, { x: 3, y: 3 }) == true)) {
                hoverSlot = index;

                cardslot.doScaling(true);
            }
            else {
                cardslot.doScaling(false);
            }
        }

        if (!!hoverSlot) {
            this.hoverSlot = cardSlots[hoverSlot];
        }
        else if (!!this.hoverSlot) {
            this.hoverSlot = undefined;
        }
    }

    /// <summary>
    /// Flips the card to the opposite side
    /// </summary>
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

    /// <summary>
    /// Picks up the card
    /// </summary>
    Card.prototype.Pickup = function () {
        this.ScaleUp();

        this.pickedUp = true;
        this.engine.player.holdingCard = this;

        var position = { rotation: this.rotation };

        asset.GetSound(asset.Sound.CARD_PICKUP).play();
    }

    /// <summary>
    /// Puts down the card
    /// </summary>
    Card.prototype.PutDown = function () {
        this.ScaleDown();
        
        // Do scaling for the hoverslot if exists
        if (!!this.hoverSlot)
        {
            this.hoverSlot.card = undefined;
            this.hoverSlot.doScaling();
        }

        this.engine.player.lastHoldingCard = this.engine.player.holdingCard;
        this.engine.player.holdingCard = undefined;
    }

    /// <summary>
    /// Animates the card back to original position
    /// </summary>
    /// <param name="c">The card</param>
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

    /// <summary>
    /// Function which processes Hover effects on the card (Player hovers the card)
    /// </summary>
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

    /// <summary>
    /// Puts a card in a slot
    /// </summary>
    /// <param name="cardSlot">a card slot</param>
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
        } // On complete end
    } // PutInSlot end



    /// <summary>
    /// Processes the Card
    /// </summary>
    Card.prototype.Process = function () {

        // Check if the card is dragged, and not in a slot.
        if (this.dragging && this.networkStopWatch.getElapsed().milliseconds > 200 && !this.inSlot) {
            this.networkStopWatch.reset();
            this.RequestMove();
        }

        // Check if card hovers a cardslot
        if (this.pickedUp)
        {
            this.checkHover();
        }

        // Check if player hovers a card
        this.OnHoverEffects();
    }

    /// <summary>
    /// Request a move action to the server.
    /// </summary>
    Card.prototype.RequestMove = function () {
        var json = Message.GAME.MOVE_CARD;
        json.Payload.x = this.x;
        json.Payload.y = Conf.height - this.y + this.height / 1.5;
        json.Payload.cid = this.cid;
        json.Payload.gameId = this.engine.gameId;

        this.engine.gameSocket.send(json);
    }

    /// <summary>
    /// Requests a card release.
    /// </summary>
    Card.prototype.RequestRelease = function () {
        var json = Message.GAME.RELEASE_CARD;
        json.Payload.cid = this.cid;
        json.Payload.gameId = this.engine.gameId;
        this.engine.gameSocket.send(json);
    }


    /// <summary>
    /// Mouse callbacks
    /// </summary>
    ////////////////////
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

    return Card;

});