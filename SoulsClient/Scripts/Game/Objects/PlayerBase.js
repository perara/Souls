define("playerbase", ["jquery", "pixi", "asset", "card"], function ($, pixi, asset, Card) {

    PlayerBase = function (texture, engine) {
        pixi.Sprite.call(this, texture);

        this.engine = engine;

        this.cardManager = new CardManager();

        var json = { name: "NA", attack: "NA", health: "NA", mana: "NA" };
        this.graphicText =
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


        this.pNamePanelText = new pixi.Text(this.graphicText.name,
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


        this.pAttackText = new pixi.Text(this.graphicText.attack,
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

        this.pManaText = new pixi.Text(this.graphicText.mana,
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


        this.pHealthText = new pixi.Text(this.graphicText.health,
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
    }

    /*var json = { name: "NA", attack: "NA", health: "NA", mana: "NA" };*/
    PlayerBase.prototype.SetText = function (jsonData) {
        this.graphicText = jsonData;
        this.pAttackText.setText(this.graphicText.attack);
        this.pHealthText.setText(this.graphicText.health);
        this.pManaText.setText(this.graphicText.mana);
        this.pNamePanelText.setText(this.graphicText.name);
    }

    PlayerBase.prototype.SetPosition = function (xy) {
        this.position = xy;
    }

    // playoropp = "Player" or "Opponent" 
    // Conf: {x,y,playoropp}
    PlayerBase.prototype.GiveCards = function(jsonCards, conf)
    {
        console.log(jsonCards)

        var count = 0;
        // Iterate over car
        for (var cJson in jsonCards)
        {
            count++;
            var c = new Card(this.engine, jsonCards[cJson]);
            c.x = c.originX = conf.x + (120 * (count));
            c.y = c.originY = conf.y;
            c.originRot = 0;
            this.cardManager.AddCardHand(c);


            this.engine.addChild(conf.playoropp, c);

            if (conf.playoropp == "Player")
            {
                c.interactive = true;
            }
        }



        /*

        var count = 0;
        for (var i in data.hand) {
            count++;
            var cid;
            if (config.player) {
                cid = i;
            }
            else {
                cid = data.hand[i];
            }

            var cardData = (config.player) ? data.hand[i] : undefined;
            console.log(cardData);
            var c = new Card(cardData);
            c.x = c.originX = config.x + (120 * (count));
            c.y = c.originY = config.y;
            c.originRot = 0;

            if (!!config.player) {
                c.interactive = true;
                c.InteractionCallback(this.CardInteractionCallback);
                this.Player.cards[cardData.cid] = c;
            }
            else {
                c.CardData.cid = cid;
                this.Opponent.cards[cid] = c;
            }

            this.addChild("Card", c);

        }*/
    }

    return PlayerBase
});