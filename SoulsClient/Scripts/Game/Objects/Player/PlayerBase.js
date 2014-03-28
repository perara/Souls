﻿define("playerbase", ["jquery", "pixi", "asset", "card"], function ($, pixi, asset, Card) {

    PlayerBase = function (texture, engine) {
        pixi.Sprite.call(this, texture);

        this.engine = engine;

        var json = { name: "NA", attack: "NA", health: "NA", mana: "NA" };

        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 385;
        this.height = 255;
        this.isPlayer = undefined;

        this.health = json.health;
        this.attack = json.attack;
        this.mana = json.mana;
        this.name = json.name;

        var masking = new pixi.Graphics();
        masking.beginFill(0xFFFFFF);
        masking.lineStyle(0, 0xffffff);
        masking.fillAlpha = 0.1;
        masking.drawEllipse(0, 0, this.width / 4 + 15, this.height / 2);
        masking.endFill();
        masking.x = -1;
        masking.y = this.height / 4 + 11;

        // Player Frame
        var pFrame = new pixi.Sprite(asset.GetTexture(asset.Textures.PLAYER_FRAME));
        pFrame.anchor = { x: 0.5, y: 0.5 };
        pFrame.width = this.width;
        pFrame.height = this.height;
        pFrame.x = 0;
        pFrame.y = 0;
        pFrame.tint = 0x044444

        // Player Image
        var pPortrait = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_PORTRAIT));
        pPortrait.anchor = { x: 0.5, y: 0.5 };
        pPortrait.width = 220;
        pPortrait.height = 145;
        pPortrait.x = 0;
        pPortrait.y = 20;


        pPortrait.mask = masking;

        // Player NamePlate
        var pNamePlate = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_NAME_PANEL));
        pNamePlate.anchor = { x: 0.5, y: 0 };
        pNamePlate.width = 220;
        pNamePlate.height = this.height / 6;
        pNamePlate.x = 0;
        pNamePlate.y = this.height / 2 - pNamePlate.height;


        ///////////////////////////////////////////
        //////////////////////////////////////////////
        /////////////TOOODOOOO/////////////////////////
        // Attack Orb
        var pAttackImage = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_ATTACK));
        pAttackImage.anchor = { x: 0.5, y: 0.5 };
        pAttackImage.width = 60;
        pAttackImage.height = 60;
        pAttackImage.x = -this.width / 4;;
        pAttackImage.y = this.height / 2 - pAttackImage.height;

        var pManaImage = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_MANA));
        pManaImage.anchor = { x: 0.5, y: 0.5 };
        pManaImage.width = 60;
        pManaImage.height = 60;
        pManaImage.x = -this.width / 4;
        pManaImage.y = 0;

        var pHealthImage = new pixi.Sprite(asset.GetTexture(asset.Textures.CARD_HEALTH));
        pHealthImage.anchor = { x: 0.5, y: 0.5 };
        pHealthImage.width = 60;
        pHealthImage.height = 60;
        pHealthImage.x = this.width / 4;
        pHealthImage.y = this.height / 2 - pHealthImage.height;


        this.pNamePanelText = new pixi.Text(this.name,
            {
                font: "26px Helvetica",
                fill: "black",
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pNamePanelText.anchor = { x: 0.5, y: 1 };
        this.pNamePanelText.x = 0;
        this.pNamePanelText.y = this.height / 2;


        this.pAttackText = new pixi.Text(this.attack,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pAttackText.anchor = { x: 0.5, y: 1 };
        this.pAttackText.x = -this.width / 4;;
        this.pAttackText.y = this.height / 2 - pAttackImage.height;

        this.pManaText = new pixi.Text(this.mana,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pManaText.anchor = { x: 0.5, y: 1 };
        this.pManaText.x = -this.width / 4;
        this.pManaText.y = 0;


        this.pHealthText = new pixi.Text(this.health,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pHealthText.anchor = { x: 0.5, y: 1 };
        this.pHealthText.x = this.width / 4;
        this.pHealthText.y = this.height / 2 - pHealthImage.height;


        // TODO ATTACK, MANA, HEALTH
        this.addChild(masking);
        this.addChild(pPortrait);
        this.addChild(pFrame);
        this.addChild(pNamePlate);
        this.addChild(pAttackImage);
        this.addChild(pManaImage);
        this.addChild(pHealthImage);

        this.addChild(this.pNamePanelText);
        this.addChild(this.pAttackText);
        this.addChild(this.pManaText);
        this.addChild(this.pHealthText);

    }
    // Constructor
    PlayerBase.prototype = Object.create(pixi.Sprite.prototype);
    PlayerBase.prototype.constructor = PlayerBase;

    PlayerBase.prototype.Process = function () {
        console.log("> [PlayerBase]: Implement Process()");
    }

    PlayerBase.prototype.Init = function () {
        console.log("> [PlayerBase]: Implement Init()");
        this.playerNr = undefined;
    }

    /*var json = { name: "NA", attack: "NA", health: "NA", mana: "NA" };*/
   /* PlayerBase.prototype.SetText = function (jsonData) {
        this.graphicText = jsonData;
        this.pAttackText.setText(this.graphicText.attack);
        this.pHealthText.setText(this.graphicText.health);
        this.pManaText.setText(this.graphicText.mana);
        this.pNamePanelText.setText(this.graphicText.name);
    }*/

    /// <summary>
    /// Sets a text field on the card depending on what the input is. see param
    /// </summary>
    /// <param name="text">Example on format : {health: 10, cost: 5}</param>
    PlayerBase.prototype.SetText = function (text) {
        console.log(text);
        this.graphicText = text;
        if (text.health) {
            this.health = text.health;
            this.pHealthText.setText(text.health);
        }
        if (text.mana || text.mana == 0) {
            this.mana = text.mana;
            this.pManaText.setText(text.mana);
        }
        if (text.ability) {
            //this.setText(text.ability);
        }
        if (text.name) {
            this.name = text.name;
            this.pNamePanelText.setText(text.name);
        }
        if (text.attack) {
            this.attack = text.attack;
            this.pAttackText.setText(text.attack);
        }
        if (text.race) {
           // this.race = text.race.id;
        }
    }

    PlayerBase.prototype.SetPosition = function (xy) {
        this.position = xy;
    }

    PlayerBase.prototype.ScaleUp = function () {
        this.scale.x = 1.2;
        this.scale.y = 1.2;
    }

    PlayerBase.prototype.ScaleDown = function () {
        this.scale.x = 1;
        this.scale.y = 1;
    }

    // playoropp = "Player" or "Opponent" 
    // Conf: {x,y,playoropp}
   

    return PlayerBase
});