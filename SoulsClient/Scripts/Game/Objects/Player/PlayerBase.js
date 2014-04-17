define("playerbase", ["jquery", "pixi", "asset", "iAnimation", "animation"], function ($, pixi, Asset, AnimationInterface, Animation) {


    PlayerBase = function (engine, isPlayer) {
       
        var frame = (isPlayer) ? Asset.GetTexture(Asset.Textures.PLAYER_FRAME) : Asset.GetTexture(Asset.Textures.OPPONENT_FRAME); 
        pixi.Sprite.call(this, Asset.GetTexture(Asset.Textures.PLAYER_NONE));
        this.engine = engine;

        // Player Animation
        this.Animation = new AnimationInterface();
        this.Animation.Death = Animation.Player.Death;
        this.Animation.Defend = Animation.Player.Defend;
        this.Animation.Attack = Animation.Player.Attack;

        var json = { name: "NA", attack: "NA", health: "NA", mana: "NA", type: 0 };

        // Defining variables
        this.anchor = { x: 0.5, y: 0.5 };
        this.width = 300;
        this.height = 250;
        this.isPlayer = isPlayer;
        this.health = json.health;
        this.attack = json.attack;
        this.mana = json.mana;
        this.name = json.name;
        this.type = json.type;
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

        this._originalScale =
            {
                x: this.scale.x,
                y: this.scale.y
            }


        // Player Portrait
        this.pPortrait = this.portrait = new pixi.Sprite(this.GetPortrait(this.type));
        this.pPortrait.anchor = { x: 0.5, y: 0.5 };
        this.pPortrait.width = this.width;
        this.pPortrait.height = this.height;
        this.pPortrait.x = 0;
        this.pPortrait.y = 0;

        this.pPortrait.mask = masking;

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
        this.addChild(this.pPortrait);
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
            item = new pixi.Sprite(Asset.GetTexture(Asset.Textures.PLAYER_MANA));
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

    PlayerBase.prototype.GetPortrait = function (portraitId) {
        var portraits =
            {
                0: Asset.Textures.PLAYER_PORTRAIT_UNKNOWN,
                1: Asset.Textures.PLAYER_PORTRAIT_ONE,
                2: Asset.Textures.PLAYER_PORTRAIT_TWO,
                3: Asset.Textures.PLAYER_PORTRAIT_THREE,
                4: Asset.Textures.PLAYER_PORTRAIT_FOUR,
                5: Asset.Textures.PLAYER_PORTRAIT_FIVE,
                6: Asset.Textures.PLAYER_PORTRAIT_SIX
            };

        if (portraits[portraitId]) {
            return Asset.GetTexture(portraits[portraitId]);
        }
        else {
            console.log("Missing PLAYER Texture ID: " + portraitId);
            return Asset.GetTexture(portraits[0]);
        }
    }

    PlayerBase.prototype.Process = function () {
        console.log("> [PlayerBase]: Implement Process()");
    }

    PlayerBase.prototype.Init = function () {
        console.log("> [PlayerBase]: Implement Init()");
        this.playerNr = undefined;
    }


    /// <summary>
    /// Sets a text field on the card depending on what the input is. see param
    /// </summary>
    /// <param name="text">Example on format : {health: 10, cost: 5}</param>
    PlayerBase.prototype.SetText = function (text) {
        this.graphicText = text;
        if (text.health) {
            this.health = text.health;
            this.pHealthText.setText(text.health);
        }
        if (text.mana || text.mana == 0) {
            this.mana = text.mana;

            $.each(this.manaCrystals, function (index, value) {

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
        if(text.type)
        {
            this.pPortrait.texture = this.GetPortrait(text.type);
        }
    }

    PlayerBase.prototype.SetPosition = function (xy) {
        this.position = xy;
    }

    PlayerBase.prototype.ScaleUp = function () {
        this.scale.x = 1.1;
        this.scale.y = 1.2;
    }

    PlayerBase.prototype.ScaleDown = function () {
        this.scale.x = this._originalScale.x;
        this.scale.y = this._originalScale.y;
    }

    PlayerBase.prototype.Attack =
        {
            CreateProton : function() {
                var texture = new Pixi.Texture.fromImage("Content/Images/particle.png");
                proton = new Proton();
                emitter = new Proton.BehaviourEmitter();
                emitter.rate = new Proton.Rate(new Proton.Span(5, 10), new Proton.Span(.02, .015));
                emitter.addInitialize(new Proton.Mass(10));
                emitter.addInitialize(new Proton.Life(1, 2.5));
                emitter.addInitialize(new Proton.ImageTarget(texture, 32));
                emitter.addInitialize(new Proton.Radius(40));
                emitter.addInitialize(new Proton.V(new Proton.Span(5, 4), 0, 'polar'));
                emitter.addBehaviour(new Proton.Alpha(1, 0));
                emitter.addBehaviour(new Proton.Color('#CECECE'));
                emitter.addBehaviour(new Proton.Scale(3, 8));
                emitter.addBehaviour(new Proton.CrossZone(new Proton.RectZone(0, 0, 1003, 1080), 'dead'));
                emitter.p.x = 1900 / 2;
                emitter.p.y = 800;
                emitter.emit();
                proton.addEmitter(emitter);

            },

            TransformSprite : function(particleSprite, particle) {
                particleSprite.position.x = particle.p.x;
                particleSprite.position.y = particle.p.y;
                particleSprite.scale.x = particle.scale;
                particleSprite.scale.y = particle.scale;
                particleSprite.anchor.x = 0.5;
                particleSprite.anchor.y = 0.5;
                particle.sprite.tint = '0x' + Pixi.rgb2hex([particle.transform.rgb.r, particle.transform.rgb.g, particle.transform.rgb.b])
            },

            CreateRender : function(){
                var renderer = new Proton.Renderer('other', proton);
                renderer.blendFunc("SRC_ALPHA", "ONE");

                renderer.onProtonUpdate = function () {

                };

                var that = this;
                renderer.onParticleCreated = function (particle) {
                    var particleSprite = new Pixi.Sprite(particle.target);

                    particle.sprite = particleSprite;

                    that.stage.addChild(particle.sprite);
                };

                renderer.onParticleUpdate = function (particle) {
                    transformSprite(particle.sprite, particle);
          
                    particle.sprite.tint = Pixi.rgb2hex([particle.transform.rgb.r, particle.transform.rgb.g, particle.transform.rgb.b])
                };

                renderer.onParticleDead = function (particle) {
                    that.stage.removeChild(particle.sprite);
                };
                
            },

            Start: function()
            {
                renderer.start();
            },

            Stop : function()
            {
                renderer.stop();
            }


        };


    // playoropp = "Player" or "Opponent" 
    // Conf: {x,y,playoropp}


    return PlayerBase
});