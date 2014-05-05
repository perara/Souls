    define("asset", ["pixi", "soundjs"], function (Pixi, SoundJS) {

        function Asset() {
            console.log("> Asset class")
        }



        Asset.Path =
        {
            Textures: "Content/Images/",
            Sound: "Content/Sound/"
        }

        Asset.prototype.PreLoad = function (onProgressCallback, onCompleteCallBack) {

            var contentArray = new Array()
            for (var key in Asset.prototype.Textures) {
                contentArray.push(Asset.prototype.Textures[key]);
            }

            var loader = new Pixi.AssetLoader(contentArray)
            loader.on('onProgress', function (e) {
                var prct = 1 - (e.content.loadCount / e.content.assetURLs.length);
                onProgressCallback(prct);
            });
            loader.on('onComplete', function (e) {
                onCompleteCallBack();
            });

            loader.load();
        }

        Asset.prototype.SetVolume = function (val) {
            createjs.Sound.setVolume(val);
        }

        Asset.prototype.LoadSound = function (onProgressCallback, onCompleteCallBack) {
            createjs.Sound.alternateExtensions = ["mp3"];

            $.each(Asset.prototype.Sound, function (key, value) {
                createjs.Sound.registerSound(value, value);
            });


            var totalSounds = Object.keys(Asset.prototype.Sound).length;
            var currentLoaded = 0
            createjs.Sound.addEventListener("fileload", handleFileLoad);
            function handleFileLoad(event) {
                console.log("Preloaded:", event.id);
                var percent = ++currentLoaded / totalSounds;

                if (percent == 1) {
                    onCompleteCallBack();
                }
                else {
                    onProgressCallback(percent * 100);
                }

            }
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
            GAME_QUEUE: Asset.Path.Textures + "queue.jpg",
            GAME_VOLUME_ON: (Asset.Path.Textures + "audio_on.png"),
            GAME_VOLUME_OFF: Asset.Path.Textures + "audio_off.png",

            // End turn
            END_TURN: Asset.Path.Textures + "endturn.png",
            END_TURN_DISABLED: Asset.Path.Textures + "endturn_disabled.png",

            // Card
            CARD_NONE: Asset.Path.Textures + "Card/CARD_NONE.png",
            CARD_SLOT: Asset.Path.Textures + "Card/cardslot.png",
            CARD_BACK: Asset.Path.Textures + "Card/cardback.png",
            CARD_DARKNESS: Asset.Path.Textures + "Card/Texture/darkness.png",
            CARD_VAMPIRIC: Asset.Path.Textures + "Card/Texture/vampiric.png",
            CARD_LIGHTBRINGER: Asset.Path.Textures + "Card/Texture/lightbringer.png",
            CARD_FEROCIOUS: Asset.Path.Textures + "Card/Texture/ferocious.png",
            CARD_ERROR: Asset.Path.Textures + "Card/Texture/card_error.png",

            // Target arrow
            ARROW_CORSHAIR: Asset.Path.Textures + "target.png",

            // Player Frame
            PLAYER_NONE: Asset.Path.Textures + "Player/player_none.png",
            PLAYER_FRAME: Asset.Path.Textures + "Player/player_frame.png",
            PLAYER_MANA: Asset.Path.Textures + "Player/player_mana.png",

            // Opponent Frame
            OPPONENT_NONE: Asset.Path.Textures + "Player/player_none.png",
            OPPONENT_FRAME: Asset.Path.Textures + "Player/opponent_frame.png",

            // Card Portraits
            CARD_PORTRAIT_UNKNOWN: Asset.Path.Textures + "Card/Portraits/0.png",
            /*CARD_PORTRAIT_ONE: Asset.Path.Textures + "Card/Portraits/1.png",
            CARD_PORTRAIT_TWO: Asset.Path.Textures + "Card/Portraits/2.png",
            CARD_PORTRAIT_THREE: Asset.Path.Textures + "Card/Portraits/3.png",
            CARD_PORTRAIT_FOUR: Asset.Path.Textures + "Card/Portraits/4.png",
            CARD_PORTRAIT_FIVE: Asset.Path.Textures + "Card/Portraits/5.png",
            CARD_PORTRAIT_SIX: Asset.Path.Textures + "Card/Portraits/6.png",*/

            // Player Portraits
            PLAYER_PORTRAIT_UNKNOWN: Asset.Path.Textures + "Player/Portraits/0.png",
            PLAYER_PORTRAIT_ONE: Asset.Path.Textures + "Player/Portraits/1.png",
            PLAYER_PORTRAIT_TWO: Asset.Path.Textures + "Player/Portraits/2.png",
            PLAYER_PORTRAIT_THREE: Asset.Path.Textures + "Player/Portraits/3.png",
            PLAYER_PORTRAIT_FOUR: Asset.Path.Textures + "Player/Portraits/4.png",
            PLAYER_PORTRAIT_FIVE: Asset.Path.Textures + "Player/Portraits/5.png",
            PLAYER_PORTRAIT_SIX: Asset.Path.Textures + "Player/Portraits/6.png"
        }

        Asset.prototype.Sound =
            {
                CARD_PICKUP: Asset.Path.Sound + "fjopp.mp3",
                CARD_MOUNT: Asset.Path.Sound + "card_mount.mp3",
                END_TURN: Asset.Path.Sound + "scroll.mp3",
                ATTACK_1: Asset.Path.Sound + "attack_1.mp3",
                DEFEND_1: Asset.Path.Sound + "defend_1.mp3",
                CHAT_MESSAGE: Asset.Path.Sound + "chat_message.mp3",
                GAME_MUSIC: Asset.Path.Sound + "background-music.mp3"
            }

        Asset.prototype.GetTexture = function (texture) {
            return Pixi.Texture.fromImage(texture);
        }


        return new Asset();

    });