var This = CardFactory.prototype;
function CardFactory(assetLoader) {
    This.assetLoader = assetLoader;
    console.log("CardFactorty: OK");
}

// Initial Configuration data for the card.
Card =
    {
        Size: { width: 120, height: 150 },
    }

// Initial COnfiguration for card slots

This.CardSlots = CardSlots =
    {
        Occupation: new Array(10),
    };


This.CardSlots.GenerateCardSlots = function () {
    var arr = new Array();
    // Start X position
    var x = 300;
    for (var i = 0; i < 7; i++) {
        // Create the card bound back (Which will ultimately be a border)
        var slot = new PIXI.Graphics();
        slot.id = i;
        slot.lineStyle(2, 0xC0EC0EE);
        slot.beginFill(0x000000);
        slot.fillAlpha = 0.3;
        slot.drawRect(0, 0, 120, 150);
        slot.endFill();
        slot.x = x += 150;
        slot.y = 700;
        slot.width = 120; //TODO
        slot.height = 150; //TODO SHOULD BE VAR
        slot.runOnceUp = false; // THIS IS for scale when hovering
        slot.runOnceDown = false; // This is for scale when hovering
        slot.hitArea = new PIXI.Rectangle(0, 0, 120, 150); //TODO

        arr.push(slot);

    }
    // Return the cardSlot array
    return arr;

}

This.generateCard = function (x, y, clickCallback) {

    // Card Container
    var cContainer = new PIXI.DisplayObjectContainer();
    cContainer.original = { width: Card.Size.width, height: Card.Size.height }; // This is the original size of the container
    cContainer.position.x = x;
    cContainer.position.y = y;
    cContainer.width = Card.Size.width;
    cContainer.height = Card.Size.height;

    console.log(cContainer.original.width * (2 / 100));

    // Custom Data
    cContainer.slot =
        {
            id: null,
            list: CardSlots.Occupation,

        };







    // Create the card bound back (Which will ultimately be a border)
    var cBorder = new PIXI.Graphics();
    cBorder.beginFill(0x000000);
    cBorder.lineStyle(2, 0xC0EC0EE);
    cBorder.drawRect(0, 0, cContainer.width, cContainer.height);
    cBorder.endFill();
    cBorder.x = 0;
    cBorder.y = 0

    // CardFactory background
    cBackground = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.CardBackground);
    cBackground.x = 3;
    cBackground.y = 3;
    cBackground.width = cContainer.width - 5;
    cBackground.height = cContainer.height - 5;

    // AbilityPane
    cAbilityPanel = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_AbilityPane);
    cAbilityPanel.x = 5;
    cAbilityPanel.y = cContainer.height / 2;
    cAbilityPanel.width = cContainer.width - 10;
    cAbilityPanel.height = cContainer.height / 2;


    // CardFactory portrait image
    cPortrait = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.CardRobot);
    cPortrait.width = cContainer.width / 2 + 10;
    cPortrait.height = cContainer.height / 2;
    cPortrait.x = cContainer.width / 4 - 5;
    cPortrait.y = (-cContainer.height / 10) - 2;

    // CardFactory portrait border
    cPortraitBorder = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.CardImageBorder);
    cPortraitBorder.x = 0;
    cPortraitBorder.y = -cContainer.height / 3;
    cPortraitBorder.width = cContainer.width;
    cPortraitBorder.height = cContainer.height;

    // CardFactory portrait wrapper
    cPortraitWrapper = new PIXI.Graphics();
    cPortraitWrapper.beginFill(0xCECECECE);
    cPortraitWrapper.drawEllipse(0, 0, (cContainer.width / 3.7), (cContainer.height / 3.3));
    cPortraitWrapper.endFill();
    cPortraitWrapper.x = (cContainer.width / 2) - 1
    cPortraitWrapper.y = (cContainer.height / 5) - 7

    // Add masking (Image and framebounds)
    cPortrait.mask = cPortraitWrapper


    // CardFactory Health Image
    cHealth = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_Health);
    cHealth.anchor = { x: 0.5, y: 0.5 };
    cHealth.width = cContainer.width / 4;
    cHealth.height = cContainer.height / 4;
    cHealth.x = cContainer.width;
    cHealth.y = cContainer.height - (cHealth.height / 4) + 7

    // CardFactory Mana Image
    cMana = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_Mana);
    cMana.anchor = { x: 0.5, y: 0.5 };
    cMana.width = cContainer.width / 3 - 5;
    cMana.height = cContainer.height / 3 - 15;
    cMana.x = 0;
    cMana.y = 0;

    // CardFactory Attack Image
    cAttack = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_Attack);
    cAttack.anchor = { x: 0.5, y: 0.5 };
    cAttack.width = cContainer.width / 3 - 10;
    cAttack.height = cContainer.height / 3 - 20;
    cAttack.x = 0;
    cAttack.y = cContainer.height - (cAttack.height / 4) + 2;

    // CardFactory Health Label
    cHealthText = new PIXI.Text("13",
        {
            font: "18px Arial",
            fill: "white",
            stroke: '#000000',
            strokeThickness: 4
        });
    cHealthText.anchor = { x: 0.5, y: 0.5 };
    cHealthText.position.x = cContainer.width;
    cHealthText.position.y = cContainer.height - (cHealth.height / 4) + 5;

    // CardFactory Mana Label
    cManaText = new PIXI.Text("20",
        {
            font: "18px Arial",
            fill: "white",
            stroke: '#000000',
            strokeThickness: 4
        });
    cManaText.anchor = { x: 0.5, y: 0.5 };
    cManaText.position.x = 0;
    cManaText.position.y = 0;


    // CardFactory Attack Label
    cAttackText = new PIXI.Text("50",
        {
            font: "18px Arial",
            fill: "white",
            stroke: '#000000',
            strokeThickness: 4
        });
    cAttackText.anchor = { x: 0.5, y: 0.5 };
    cAttackText.position.x = 0;
    cAttackText.position.y = cContainer.height - (cAttack.height / 4) + 3;

    // CardFactory Ability Label
    cAbilityPanelText = new PIXI.Text("Charges wildly into an random enemy card",
        {
            font: "12px Arial",
            fill: "black",
            wordWrap: true,
            wordWrapWidth: cContainer.width - 24
        });

    cAbilityPanelText.position.x = 15;
    cAbilityPanelText.position.y = cContainer.height / 2 + 10;

    // CardFactory Name Background
    cNamePanel = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_NamePane);
    cNamePanel.x = 0;
    cNamePanel.y = (cContainer.width / 8) * 3 - 2;
    cNamePanel.width = cContainer.width;
    cNamePanel.height = (cContainer.height / 5);


    // CardFactory Name Label
    cNamePanel.Text = new PIXI.Text("Dr. Robotnik",
        {
            font: "14px Arial Black",
            fill: "black",
            wordWrap: true,
            align: 'center',

            wordWrapWidth: cContainer.width,
        });
    cNamePanel.Text.x = (cNamePanel.width / 8) - 4;
    cNamePanel.Text.y = (cContainer.height / 8) * 3 - (cNamePanel.height / 2) + 9;




    // Add wrapper to the card
    cContainer.addChild(cBorder);
    // Add background to the card
    cContainer.addChild(cBackground);
    // Add abilityPane to the card
    cContainer.addChild(cAbilityPanel);


    // Add the image to the portrait container
    cContainer.addChild(cPortrait);
    // Add the portrait border to the portrait container
    cContainer.addChild(cPortraitBorder);
    // Add Portraits wrapper to the Portrait container.
    cContainer.addChild(cPortraitWrapper);

    // Add the Health Image to the card
    cContainer.addChild(cHealth);
    // Add the Mana Image to the card
    cContainer.addChild(cMana);
    // Add the Attack Image to the card
    cContainer.addChild(cAttack);

    // Add the Health text to the CardFactory 
    cContainer.addChild(cHealthText);
    // Add the Mana text to the CardFactory 
    cContainer.addChild(cManaText);
    // Add the Attack text to the CardFactory 
    cContainer.addChild(cAttackText);
    // Add the Ability text to the CardFactory
    cContainer.addChild(cAbilityPanelText);

    // Add the Name pane to the CardFactory
    cContainer.addChild(cNamePanel);
    // Add the Name text to the CardFactory
    cContainer.addChild(cNamePanel.Text);

    // Listeners and Callbacks
    {
        cContainer.setInteractive(true);
        cContainer.buttonMode = true;

        cContainer.mousedown = cContainer.touchstart = function (data) {

            // Scale up a notch

            // Scale Card
            PIXIE.ScaleGraphics(1.2, 1.2, this);

            this.data = data;
            this.alpha = 0.9;
            this.dragging = true;

            // Send callback to Game.Func.Action.Card.Click with information that card was clicked!
            clickCallback.Click(this);
        };

        // set the events for when the mouse is released or a touch is released
        cContainer.mouseup = cContainer.mouseupoutside = cContainer.touchend = cContainer.touchendoutside = function (data) {
            // Scale Card
            PIXIE.ScaleGraphics(1, 1, this);

            this.alpha = 1
            this.dragging = false;

            // set the interaction data to null
            this.data = null;

            // Disable the targeter
            Souls.Objects['targeter'].visible = false;

            // Send callback to Game.Func.Action.Card.Click with information that cardclick was released!
            clickCallback.Release(this);
        };

        // set the callbacks for when the mouse or a touch moves
        cContainer.mousemove = cContainer.touchmove = function (data) {
            if (this.dragging) {

                // need to get parent coords..
                var newPosition = this.data.getLocalPosition(this.parent);

                if (this.slot.id == null) {

                    this.x = (newPosition.x - (cContainer.width / 2));
                    this.y = (newPosition.y - (cContainer.height / 2));

                    // Callback which is activated when you drag the mouse (Callback is in Game)
                    var card = this;
                    $.each(Souls.Window.ViewGroup.CardSnap.children, function (key, val) {

                        // Check if the Mouse intersects with the snap
                        var hit = PIXIE.Intersects(
                             {
                                 width: 1,
                                 height: 1,
                                 x: card.x + (card.width / 2),// Account for anchor which is 0.5,0.5
                                 y: card.y + (card.height / 2), // Account for anchor which is 0.5,0.5
                             }, val);



                        if (hit && !val.runOnceUp) {
                            PIXIE.ScaleGraphics(1.2, 1.2, val);
                            val.runOnceUp = true;
                            val.runOnceDown = false;

                            // Set which element the card is hovering over
                            card.slot.hover = val;

                            return;
                        }


                        if (!hit && !val.runOnceDown) {
                            PIXIE.ScaleGraphics(1, 1, val);
                            val.runOnceUp = false;
                            val.runOnceDown = true;

                            card.slot.hover = null;

                            return;
                        }

                    });

                }
                else
                {
                    // Card is in a slot
                    Souls.Objects['targeter'].position = 
                        { 
                            x: newPosition.x,
                            y: newPosition.y,
                        };

                    // Set the targeter to visible.
                    Souls.Objects['targeter'].visible = true;

                }



            }
        }

        // Return the card
        return cContainer;
    }
}