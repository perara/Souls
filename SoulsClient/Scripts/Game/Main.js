//Tutorials RequireJS: http://javascriptplayground.com/blog/2012/07/requirejs-amd-tutorial-introduction/
//http://www.sitepoint.com/understanding-requirejs-for-effective-javascript-module-loading/

require.config({
    //baseUrl: "Scripts/GamePLS",
    paths: {
        "jquery": "/Scripts/jquery-2.1.0",
        "pixi": "/Scripts/pixi.dev", // PIXI.js
        "tween": "/Scripts/tween.min",
        "soundjs": "/Scripts/soundjs-0.5.2.min",
        "stats" : "/Scripts/stats.min",

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

        // Objects
        "card": "Objects/Card",
        "cardslot": "Objects/CardSlot",
        "player": "Objects/Player",
        "playerbase": "Objects/PlayerBase",
        "arrow": "Objects/Arrow",
        "opponent": "Objects/Opponent"
    },
    shim: {
        'pixi': {
            exports: 'PIXI'
        }
    }
});


//This defines a global class "EngineConf" for the whole application. This is specifically targetet to the Engine.X namespace.
var Engine = {};
define('conf', ['/Player/hash?callback=define'], function (define) {

    this.Data = null;
    function Conf() {
        this.Data = new Object();
        this.Data.width = 1920;
        this.Data.height = 1080;
        this.Data.frame = 0;
        this.Data.fps = 1;
        this.Data.mouseX = 0;
        this.Data.mouseY = 0;

        // These configurations origins from JSONP service
        this.Data.hash = define.hash;
    }


    Conf.prototype.get = function (obj) {
        return this.Data.obj;
    }

    Conf.prototype.save = function (obj, data) {
        this.Data.obj = data;
    }

    return new Conf();
});


//Engine class
require(['jquery', 'pixi', 'asset', 'conf', 'gamestate', 'game', 'socket'], function ($, Pixi, Asset, Conf, Gamestate, Game, Socket) {

    Engine.window = null;
    Engine.stage = null;

    // This function initialize all required dependencies for the game to work. Window, Resources etc                  
    function Initialize() {
        // Create a stage and the renderer
        Engine.stage = new Pixi.Stage(0x000000, true);
        Engine.window = new Pixi.autoDetectRenderer(Conf.Data.width, Conf.Data.height, null, false,true);
        $("#game-window").html(Engine.window.view);


        // Set start to game
        Conf.currentState = Gamestate.GAME;

        TempSocketStuff();

        // Fire onReady after init stuff
        OnResize();
        OnReady();

    }

    function TempSocketStuff() {
        // Connect to server


        Socket.send(Socket.Message.GENERAL.LOGIN);
        Socket.send(Socket.Message.GAME.QUEUE);
    }

    function OnReady() {
        console.log("> All classes loaded");

        Game.Process();

        // start the game loop
        setInterval(function () { GameLoop() }, 1000 / Conf.fps);
    }

    function OnResize() {
        var width = $(window).width();
        var height = $(window).height();

        Engine.window.view.style.width = $(window).width() + "px";
        Engine.window.view.style.height = $(window).height() - $(".navbar").height() + "px";

    }

    function GameLoop() {

        // console.log(Game);

        switch (Conf.currentState) {

            case Gamestate.MENU:
                //console.log("MENU")
                break;
            case Gamestate.GAME:
                Engine.window.render(Game.Process());
                break;
            case Gamestate.PAUSED:
                // console.log("PAUSED")
                break;
        }
    }

    /*
    Socket.onMessage(function (data) {

    });*/

    // Run the game
    $(document).ready(Initialize());
    $(window).resize(OnResize);
});
