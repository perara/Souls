﻿//Tutorials RequireJS: http://javascriptplayground.com/blog/2012/07/requirejs-amd-tutorial-introduction/
//http://www.sitepoint.com/understanding-requirejs-for-effective-javascript-module-loading/

require.config({
    //baseUrl: "Scripts/GamePLS",
    paths: {
        // Third party
        "jquery": "/Scripts/jquery-1.11.0.min",
        "jqueryUI": "/Scripts/jquery-ui-1.10.4.min",
        "jquery.dialogExtend": "/Scripts/Game/Vendor/jquery.dialogextend.min",


        "pixi": "/Scripts/Game/Vendor/pixi.dev", // PIXI.js
        "tweenjs": "/Scripts/Game/Vendor/tweenjs-0.5.1.min",
        "easeljs": "/Scripts/Game/Vendor/easeljs-0.7.1.min",
        "soundjs": "/Scripts/Game/Vendor/soundjs-0.5.2.min",

        "stats": "/Scripts/Game/Vendor/stats.min",
        "stopwatch": "/Scripts/Game/Vendor/stopwatch",

        // Toolbox - General Tools
        "toolbox": "Toolbox/Toolbox",
        "toolbox_rectangle": "Toolbox/Rectangle",

        // Toolbox - Seperated (Important and core stuff)
        "asset": "Toolbox/assetLoader",
        "socket": "Client/Socket",
        "messages": "Client/Messages",


        // States
        "state": "States/State", // Abstract state class which everything should extend
        "gamestate": "States/GameState",
        "game": "States/Game",

        // Networking
        "gameService": "Networking/GameService",
        "chatService": "Networking/ChatService",
        "networkBase": "Networking/NetworkBase",


        // Objects
        //// Animation
        "animation": "Objects/Animation/Animation",
        "iAnimation": "Objects/Animation/AnimationInterface",
        //// Card
        "card": "Objects/Card/Card",
        "cardtype": "Objects/Card/CardType",
        "cardslot": "Objects/Card/CardSlot",
        "cardmanager": "Objects/Card/CardManager",
        //// Environment
        "background": "Objects/Environment/Background",
        "endturnbutton": "Objects/Environment/EndTurnButton",
        "queue": "Objects/Environment/Queue",
        "audio": "Objects/Environment/AudioButton",
        //// Player
        "player": "Objects/Player/Player",
        "playerbase": "Objects/Player/PlayerBase",
        "arrow": "Objects/Player/Arrow",
        "opponent": "Objects/Player/Opponent",
        // Other
        "chat": "Objects/Chat"


    },
    shim: {
        'pixi': {
            exports: 'PIXI'
        },
        'jqueryUI':
            {
                deps: ['jquery']
            },
        'jquery.dialogExtend':
            {
                deps: ['jquery', 'jqueryUI']
            },
        'easeljs': {
            exports: 'createjs'
        },
        'tweenjs': {
            deps: ['easeljs'],
            exports: 'Tween'
        },
        'game': {
            deps: ['asset']
        }




    }
});


//This defines a global class "EngineConf" for the whole application. This is specifically targetet to the Engine.X namespace.
var Engine = {};
define('conf', ['/Player/hash?callback=define', '/Card/CardTextures?callback=define', 'cardtype', 'asset'], function (define, cards, CardType, Asset) {



    function Conf() {
        this.mouse = { x: -10000, y: -10000 };
        this.width = 1920;
        this.height = 1080;
        this.Frame = 0;
        this.FPS = 60;
        this.hash = define.hash;

        // console.log(CardType);
        $.each(cards.data, function (key, value) {

            CardType.Portrait[value.id] = value.portrait;
            Asset.Textures[value.id] = value.portrait;
        });
        // console.log(CardType.Portrait);


        /*$.each(cards, function () {
            console.log(cards);
        });*/
    }

    return new Conf();
});


//Engine class
require(['jquery', 'pixi', 'asset', 'conf', 'gamestate', 'game', 'socket', 'stats'], function ($, Pixi, Asset, Conf, Gamestate, Game, Socket, stats) {


    function Initialize() {
        // Create a stage and the renderer
        this.stage = new Pixi.Stage(0x000000, true);

        var textureBg = Pixi.Texture.fromImage("/Content/Images/bg_faded.png");
        var logo = new Pixi.Sprite(textureBg);
        logo.anchor = { x: 0, y: 0 }
        logo.x = 0;
        logo.y = 0;
        logo.width = Conf.width;
        logo.height = Conf.height;
        this.stage.addChild(logo);

        var textureLogo = Pixi.Texture.fromImage("/Content/Images/Logo.png");
        var logo = new Pixi.Sprite(textureLogo);
        logo.anchor = { x: 0.5, y: 0 }
        logo.x = (Conf.width / 2) - 50;
        logo.y = 25;
        this.stage.addChild(logo);

        // Create a loading message
        var txtLoading = new Pixi.Text("Loading...",
            {
                font: "120px Arial bold",
                fill: "white",
                stroke: '#FFFFFF',
                wordWrap: true,
                align: 'center',

                wordWrapWidth: this.width
            });
        txtLoading.anchor = { x: 0.5, y: 0.5 };
        txtLoading.x = Conf.width / 2;
        txtLoading.y = Conf.height / 2;
        this.stage.addChild(txtLoading);



        this.renderer = new Pixi.autoDetectRenderer(Conf.width, Conf.height, null, false, true);

        var gameWindow = $("#game-window").clone();
        gameWindow.html(this.renderer.view);

        $(".page-wrapper").remove();
        $("body").append(gameWindow);

        // Deactivate right click menu
        var blockContextMenu = function (evt) {
            evt.preventDefault();
        };

        // window = document.querySelector('#game-window');
        window.addEventListener('contextmenu', blockContextMenu);


        // Set current state to LOAD
        Conf.currentState = Gamestate.LOADING;

        // Create the game engine
        this.gameEngine;


        this.isTextureLoaded = this.isSoundLoaded = false;
        this.texturePercent = this.soundPercent = 0

        var that = this;
        // Preload all assets
        Asset.PreLoad(
            function (percent) {
                console.log("[AssetLoader]: Status " + percent * 100);
                that.texturePercent = percent * 100;




            },
            function () {
                // OnComplete
                that.isTextureLoaded = true;

            });
        // Load all sound into memory (Asset.soundList)
        Asset.LoadSound(function (percent) { // On progress
            that.soundPercent = percent;
        },
        function () {
            // OnComplete
            that.isSoundLoaded = true;
        });

        var loadChecker = setInterval(function () {
            txtLoading.setText("Loading... (" + ((that.texturePercent + that.soundPercent) / 2).toFixed(0) + "%)");


            if (isTextureLoaded && isSoundLoaded) {
                console.log("Everything is loaded...")
                Conf.currentState = Gamestate.MENU;
                ShowGameSelect(); // Show the MENU

                clearInterval(loadChecker);
            }
        }, 50);


        // Show stats
        var stats = new Stats();
        stats.setMode(1); // 0: fps, 1: ms

        // Align top-left
        stats.domElement.style.position = 'absolute';
        stats.domElement.style.left = '0px';
        stats.domElement.style.top = '0px';
        document.body.appendChild(stats.domElement);

        // Fire onResize after init stuff
        OnResize();

        // GameLoop
        setInterval(function () {
            stats.begin();
            GameLoop();
            stats.end();
        }, 1000 / Conf.FPS);

        // Running keepalive each minute
        setInterval(function () { KeepAlive() }, 60000);

    }

    function ShowGameSelect() {
        var menuSound = Asset.GetSound(Asset.Sound.MENU);
        menuSound.play({ loop: 9001, volume: 0.1 });
        this.stage = new Pixi.Stage(0x000000, true);

        var textureBg = Asset.GetTexture(Asset.Textures.GAME_LOAD_BG);
        var logo = new Pixi.Sprite(textureBg);
        logo.anchor = { x: 0, y: 0 }
        logo.x = 0;
        logo.y = 0;
        logo.width = Conf.width;
        logo.height = Conf.height;
        this.stage.addChild(logo);

        var textureLogo = Asset.GetTexture(Asset.Textures.GAME_LOAD_LOGO);
        var logo = new Pixi.Sprite(textureLogo);
        logo.anchor = { x: 0.5, y: 0 }
        logo.x = (Conf.width / 2) - 50;
        logo.y = 25;
        this.stage.addChild(logo);

        var practiceTexture = Asset.GetTexture(Asset.Textures.GAME_LOAD_PRACTICE_OFF);
        var practiceFocusTexture = Asset.GetTexture(Asset.Textures.GAME_LOAD_PRACTICE_ON);
        practiceTexture.width = 200;
        practiceTexture.height = 150;
        var practiceGame = new Pixi.Sprite(practiceTexture);
        practiceGame.interactive = true;
        practiceGame.width = 200;
        practiceGame.height = 150;
        practiceGame.anchor = { x: 0.5, y: 0.5 }
        practiceGame.x = (Conf.width / 3);
        practiceGame.y = (Conf.height / 2);
        this.stage.addChild(practiceGame);

        var normalTexture = Asset.GetTexture(Asset.Textures.GAME_LOAD_PVP_OFF);
        var normalFocusTexture = Asset.GetTexture(Asset.Textures.GAME_LOAD_PVP_ON);
        normalTexture.width = 200;
        normalTexture.height = 150;
        var normalGame = new Pixi.Sprite(normalTexture);
        normalGame.interactive = true;
        normalGame.width = 200;
        normalGame.height = 150;
        normalGame.anchor = { x: 0.5, y: 0.5 }
        normalGame.x = (Conf.width / 2) + normalGame.width + 50;
        normalGame.y = (Conf.height / 2);
        this.stage.addChild(normalGame);

        // Workaround since interactive = false dont work properly?
        function clearInteractive() {
            menuSound.stop();
            practiceGame.interactive = false; // Should work (Doesnt)
            that.stage.removeChild(practiceGame); // Should ALSO work (Doesnt)
            practiceGame.click = practiceGame.tap = undefined; // Holy Fuck, i give up

            normalGame.interactive = false; // Should work (Doesnt)
            that.stage.removeChild(normalGame); // Should ALSO work (Doesnt)
            normalGame.click = normalGame.tap = undefined; // Holy Fuck, i give up

        }


        var that = this;
        practiceGame.click = practiceGame.tap = function (mouseData) {
            clearInteractive();

            gameEngine = new Game(false);
            Conf.currentState = Gamestate.GAME;

        }

        practiceGame.mouseover = function (mouseData) {
            practiceGame.texture = practiceFocusTexture;

        }
        practiceGame.mouseout = function (mouseData) {
            practiceGame.texture = practiceTexture;
        }

        normalGame.click = normalGame.tap = function (mouseData) {
            clearInteractive();


            gameEngine = new Game(true);
            Conf.currentState = Gamestate.GAME;
        }

        normalGame.mouseover = function (mouseData) {
            normalGame.texture = normalFocusTexture;
        }
        normalGame.mouseout = function (mouseData) {
            normalGame.texture = normalTexture;
        }



    }

    function OnResize() {
        var width = $(window).width();
        var height = $(window).height();

        this.renderer.view.style.width = $(window).width() + "px";
        this.renderer.view.style.height = $(window).height() - $(".navbar").height() + "px";

    }


    function GameLoop() {

        if (Conf.currentState == Gamestate.LOADING) {
            this.renderer.render(this.stage);
        }
        else if (Conf.currentState == Gamestate.MENU) {
            this.renderer.render(this.stage);
        }
        else if (Conf.currentState == Gamestate.GAME) {
            this.stage = this.gameEngine.Process();
            this.renderer.render(this.stage);
        }

        else if (Conf.currentState == Gamestate.NOT_SUPPORTED) {
            console.log("Not supported!");
        }
    }

    function KeepAlive() {
        $.ajax(
            {
                type: 'GET',
                dataType: 'jsonp',
                url: '/Player/hash?callback=define'
            })
        .done(function (define) {
            if (define.hash == null) {
                console.log('%c [ASP-KEEPALIVE]: Hash is NULL (NotLoggedIn)! ', 'background: #222; color: #bada55');
            }
            else {
                console.log('%c [ASP-KEEPALIVE]: Hash OK! ', 'background: #222; color: #bada55');
            }
        });

    }

    // Run the game
    $(document).ready(Initialize());
    $(window).resize(OnResize);
});
