﻿define("player", ["jquery", "pixi", "asset", "playerbase", "arrow", "cardmanager"], function ($, pixi, asset, playerBase, Arrow, CardManager) {


    Player = function (engine) {
        var texture = asset.GetTexture(asset.Textures.PLAYER_FRAME);
        var portrait = asset.GetTexture(asset.Textures.CARD_PORTRAIT);
        playerBase.call(this, portrait, engine, true);

        this.holdingCard; // Currently holding card (Object)
        this.lastHoldingCard; // The last card the player held

        this.glowingCrystals = 0;
    }
    // Constructor
    Player.prototype = Object.create(playerBase.prototype);
    Player.prototype.constructor = Player;


    Player.prototype.Process = function () {

        // Process arrow
        this.arrow.Process();

        // Check if manacrystals whould be updated, recolored or alphaed
        this.CheckManaCost();

        // Process cardmanager
        this.cardManager.Process();

        // Check if the arrow should be activated
        var isArrowAction = this.arrow.ArrowShowCheck(this.holdingCard);

        // if movement - check card attack
        if (isArrowAction)
            var isAttackingC = this.arrow.CardAttackCheck(this.holdingCard);

        // if no card attack found - do opponent attack
        if (isArrowAction && !isAttackingC)
            var isAttackingP = this.arrow.PlayerAttackCheck(this.holdingCard);


    }

    // Checks the players mana against the card cost
    Player.prototype.CheckManaCost = function () {

        for (var i = 0; i < 10; i++) {

            item = this.manaCrystals[i];
            
            // If hovercard is defined and hava a cost more than current manacrystal
            if (!!this._chover && i < this._chover.cost) {

                // If players mana is more than current crystal
                if (i < this.mana) {

                    // Checks if the player can afford this card in total. If yes, cost crystals are green, if no available are yellow.
                    if (this.mana >= this._chover.cost)
                        item.tint = 0x4cd328; // Green crystal
                    else item.tint = 0xFFFE00; // Yellow crystal
                }
                // If players mana is less than current crystal
                else {
                    item.visible = true;
                    item.tint = 0xe52a2a; // Red crystal
                    item.alpha = 0.5;
                }
            }
            
            // If hovercard is not defined or cost is less than current crystal
            // Resets the crystal to original color and visibility
            else {
                item.alpha = 1;
                item.tint = 0x6e93b3;
                // Checks if player have enough mana to show this crystal
                if (i >= this.mana) {
                    item.visible = false;
                }
            }
        }
    }


    // Initializer function
    Player.prototype.Init = function () {
        this.arrow = new Arrow(this.engine);
        this.engine.addChild("Player", this);

        this.cardManager = new CardManager(this.engine, true);
        this.cardManager.AddCardSlots();
        this.engine.getGroup("CardSlot-Player").visible = false;
        this.engine.getGroup("CardSlot-Player").alpha = 0.7;

        this.SetPosition(
            {
                x: (this.engine.conf.width / 2),
                y: (this.engine.conf.height - this.height / 2)
            });
    }
    return Player;

});