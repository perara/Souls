http://buzz.jaysalvat.com/documentation/buzz/


    define("asset", ["pixi", "soundjs"], function (Pixi, SoundJS) {

        function Asset() {
            console.log("> Asset class")
            // Load all textures into PIXI cache
            Pixi.AssetLoader(Asset.Textures);
            // Load all sound into memory (Asset.soundList)
            Asset.LoadSound();

             
        }

      

        Asset.Path =
        {
            Textures: "Content/Images/",
            Sound: "Content/Sound/",
        }


        Asset.LoadSound = function () {
            createjs.Sound.alternateExtensions = ["mp3"];

            $.each(Asset.prototype.Sound, function (key, value) {
                createjs.Sound.registerSound(value, value);
            });
            return true;
        }

        Asset.prototype.GetSound = function (soundEnum) {
            var instance = createjs.Sound.play(soundEnum);  // play using id.  Could also use full source path or event.src.
            return instance;
        }

        Asset.prototype.Textures =
        {
            // Game apperance
            GAME_BG: (Asset.Path.Textures + "arena.jpg"),

            // Card Textures
            CARD_NONE: Asset.Path.Textures + "Card/CARD_NONE.png",
            CARD_BG: Asset.Path.Textures + "Card/card_bg.jpg",
            CARD_PORTRAIT: Asset.Path.Textures + "Card/dr_robot.png",
            CARD_PORTRAIT_BORDER: Asset.Path.Textures + "Card/card_image_frame.png",
            CARD_HEALTH: Asset.Path.Textures + "Card/card_health.png",
            CARD_MANA: Asset.Path.Textures + "Card/card_mana.png",
            CARD_ATTACK: Asset.Path.Textures + "Card/card_attack.png",
            CARD_ABILITY_PANEL: Asset.Path.Textures + "Card/card_ability_pane.png",
            CARD_NAME_PANEL: Asset.Path.Textures + "Card/card_name_pane.png",
            CARD_SLOT: Asset.Path.Textures + "Card/cardslot.png",

            PLAYER_NONE: Asset.Path.Textures + "Card/player_none.png",
            PLAYER_FRAME: Asset.Path.Textures + "Card/player_frame.png",

            OPPONENT_NONE: Asset.Path.Textures + "Card/player_none.png", //TODO
            OPPONENT_FRAME: Asset.Path.Textures + "Card/player_frame.png",//TODO
        }

        Asset.prototype.Sound =
            {
                CARD_PICKUP: Asset.Path.Sound + "fjopp.mp3",
                CARD_MOUNT: Asset.Path.Sound + "card_mount.mp3",


            }
        Asset.prototype.GetTexture = function (texture) {
            return Pixi.Texture.fromImage(texture);
        }


        return new Asset();

    });