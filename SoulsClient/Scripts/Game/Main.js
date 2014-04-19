//Tutorials RequireJS: http://javascriptplayground.com/blog/2012/07/requirejs-amd-tutorial-introduction/
//http://www.sitepoint.com/understanding-requirejs-for-effective-javascript-module-loading/

require.config({
    //baseUrl: "Scripts/GamePLS",
    paths: {
        // Third party
        "jquery": "/Scripts/jquery-1.11.0.min",
        "jqueryUI": "/Scripts/jquery-ui-1.10.4.min",
        "jquery.dialogExtend": "/Scripts/jquery.dialogextend.min",


        "pixi": "/Scripts/pixi.dev", // PIXI.js
        "tweenjs": "/Scripts/tweenjs-0.5.1.min",
        "easeljs": "/Scripts/easeljs-0.7.1.min",
        "soundjs": "/Scripts/soundjs-0.5.2.min",

        "stats": "/Scripts/stats.min",
        "stopwatch": "/Scripts/stopwatch",

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
        "iAnimation" : "Objects/Animation/AnimationInterface",
        //// Card
        "card": "Objects/Card/Card",
        "cardtype": "Objects/Card/CardType",
        "cardslot": "Objects/Card/CardSlot",
        "cardmanager": "Objects/Card/CardManager",
        //// Environment
        "background": "Objects/Environment/Background",
        "endturnbutton": "Objects/Environment/EndTurnButton",
        "queue": "Objects/Environment/Queue",
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
define('conf', ['/Player/hash?callback=define'], function (define) {

    function Conf() {
        this.mouse = { x: -10000, y: -10000 };
        this.width = 1920;
        this.height = 1080;
        this.Frame = 0;
        this.FPS = 60;
        this.hash = define.hash;
    }

    return new Conf();
});


//Engine class
require(['jquery', 'pixi', 'asset', 'conf', 'gamestate', 'game', 'socket', 'stats'], function ($, Pixi, Asset, Conf, Gamestate, Game, Socket, stats) {


    function Initialize() {
        // Create a stage and the renderer
        this.stage = new Pixi.Stage(0x000000, true);

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

        // Set current state to LOAD
        Conf.currentState = Gamestate.LOADING;

        // Create the game engine
        this.gameEngine;

        // Preload all assets
        Asset.PreLoad(function (percent) {

            console.log("[AssetLoader]: Status " + percent * 100);
        },
        function () {

            gameEngine = new Game();
            Conf.currentState = Gamestate.GAME;
        });

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

    function OnResize() {
        var width = $(window).width();
        var height = $(window).height();

        this.renderer.view.style.width = $(window).width() + "px";
        this.renderer.view.style.height = $(window).height() - $(".navbar").height() + "px";

    }


    function GameLoop() {
    
        if (Conf.currentState == Gamestate.LOADING)
        {
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
