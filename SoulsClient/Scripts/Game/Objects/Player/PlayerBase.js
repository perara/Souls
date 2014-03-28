define("playerbase", ["jquery", "pixi", "asset", "card"], function ($, pixi, asset, Card) {

    PlayerBase = function (portrait, engine, isPlayer) {

        var frame = (isPlayer) ? asset.GetTexture(asset.Textures.PLAYER_FRAME) : asset.GetTexture(asset.Textures.OPPONENT_FRAME);
        pixi.Sprite.call(this, asset.GetTexture(asset.Textures.PLAYER_NONE));
        this.engine = engine;

        var json = { name: "NA", attack: "NA", health: "NA", mana: "NA" };

        // Defining variables
        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 300;
        this.height = 250;
        this.isPlayer = isPlayer;
        this.health = json.health;
        this.attack = json.attack;
        this.mana = json.mana;
        this.name = json.name;
        this.manaCrystals = new Array();

        // Making mask for the portrait
        var masking = new pixi.Graphics();
        masking.beginFill(0xFFFFFF);
        masking.lineStyle(0, 0xffffff);
        masking.fillAlpha = 0.5;
        masking.drawEllipse(0, 0, this.width / 2.2, this.height / 2.1);
        masking.endFill();
        masking.x -= 4;

        // Player Frame
        var pFrame = new pixi.Sprite(frame);
        pFrame.anchor = { x: 0.5, y: 0.5 };
        pFrame.width = this.width;
        pFrame.height = this.height;

        // Player Portrait
        var pPortrait = new pixi.Sprite(portrait);
        pPortrait.anchor = { x: 0.5, y: 0.5 };
        pPortrait.width = this.width;
        pPortrait.height = this.height;
        pPortrait.x = 0;
        pPortrait.y = 0;

        pPortrait.mask = masking;

        // Name text
        this.pNamePanelText = new pixi.Text(this.name,
            {
                font: "26px Helvetica",
                fill: "white",
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pNamePanelText.anchor = { x: 0.5, y: 1 };
        this.pNamePanelText.x = 0;
        this.pNamePanelText.y = this.height / 2.1;

        // Attack damage text
        this.pAttackText = new pixi.Text(this.attack,
            {
                font: "35px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pAttackText.anchor = { x: 0.5, y: 1 };
        this.pAttackText.x -= this.width / 2.66;
        this.pAttackText.y += this.height / 3.2;
      
        // Health text
        this.pHealthText = new pixi.Text(this.health,
            {
                font: "35px Arial",
                fill: "white",
                stroke: '#000000',
                strokeThickness: 4,
                align: 'center',

                wordWrapWidth: this.width
            });
        this.pHealthText.anchor = { x: 0.5, y: 1 };
        this.pHealthText.x += this.width / 2.65;
        this.pHealthText.y += this.height / 3.2;

        // Adding sprites to base
        this.addChild(pPortrait);
        this.addChild(masking);
        this.addChild(pFrame);
        this.addChild(this.pNamePanelText);
        this.addChild(this.pAttackText);
        this.addChild(this.pHealthText);

        // Generating the manacrystals in elipse around owner
        var num = 10;
        var deg = (Math.PI * 2) / 180;
        var degJump = (Math.PI / 180) * 20
        var radiusX = (this.width - 10) / 2;
        var radiusY = (this.height - 25) / 2;
        var YOffset;
        var startOffset

        if (!!this.isPlayer) {
            startOffset = 9;
            YOffset = 0;
        }
        else {
            startOffset = 0;
            YOffset = 25;
        }

        for (var i = 0; i < num; i++) {
            var item = this.manaCrystals[i];
            item = new pixi.Sprite(asset.GetTexture(asset.Textures.PLAYER_MANA));
            item.anchor = { x: 0.5, y: 0.5 };
            item.width = 30;
            item.height = 30;
            item.tint = 0x6e93b3;
            item.x = this.x + radiusX * Math.cos(deg + (degJump * (i + startOffset))) - 2;
            item.y = this.y + radiusY * Math.sin(deg + (degJump * (i + startOffset))) - 15 + YOffset;
            item.visible = false;
            this.manaCrystals[i] = item;
            this.addChild(item);
        }

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

            $.each(this.manaCrystals, function (index, value) {
                console.log(":DD")
                if (index < text.mana) {
                    value.visible = true;
                }
                else {
                    value.visible = false;
                }

            });

            //this.pManaText.setText(text.mana);
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