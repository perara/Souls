define("playerbase", ["jquery", "pixi", "asset"], function ($, pixi, asset) {

    PlayerBase = function (texture, json) {
        pixi.Sprite.call(this, texture);

        this.NetworkData = undefined;
        this.cards = new Array();

        ///TODODODO
        this.NetworkData =
            {
                name: json.name,
                attack: json.attack,
                health: json.health,
                mana: json.mana

            }


        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 385;
        this.height = 255;

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


        var pNamePanelText = new pixi.Text(this.NetworkData.name,
            {
                font: "26px Helvetica",
                fill: "black",
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        pNamePanelText.anchor = { x: 0.5, y: 1 };
        pNamePanelText.x = 0;
        pNamePanelText.y = this.height / 2;


        var pAttackText = new pixi.Text(this.NetworkData.attack,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        pAttackText.anchor = { x: 0.5, y: 1 };
        pAttackText.x = -this.width / 4;;
        pAttackText.y = this.height / 2 - pAttackImage.height;

        var pManaText = new pixi.Text(this.NetworkData.mana,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        pManaText.anchor = { x: 0.5, y: 1 };
        pManaText.x = -this.width / 4;
        pManaText.y = 0;


        var pHealthText = new pixi.Text(this.NetworkData.health,
            {
                font: "18px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        pHealthText.anchor = { x: 0.5, y: 1 };
        pHealthText.x = this.width / 4;
        pHealthText.y = this.height / 2 - pHealthImage.height;


        // TODO ATTACK, MANA, HEALTH
        this.addChild(masking);
        this.addChild(pPortrait);
        this.addChild(pFrame);
        this.addChild(pNamePlate);
        this.addChild(pAttackImage);
        this.addChild(pManaImage);
        this.addChild(pHealthImage);

        this.addChild(pNamePanelText);
        this.addChild(pAttackText);
        this.addChild(pManaText);
        this.addChild(pHealthText);


    }
    // Constructor
    PlayerBase.prototype = Object.create(pixi.Sprite.prototype);
    PlayerBase.prototype.constructor = PlayerBase;

});