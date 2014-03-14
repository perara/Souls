//Tutorials RequireJS: http://javascriptplayground.com/blog/2012/07/requirejs-amd-tutorial-introduction/
//http://www.sitepoint.com/understanding-requirejs-for-effective-javascript-module-loading/
require.config({
	paths: {
		"jquery": "http://code.jquery.com/jquery-1.11.0.min",
	}
});

//This defines a global class "EngineConf" for the whole application. This is specifically targetet to the Engine.X namespace.
var Engine = {};
define('EngineConf',[],function(){ 
	var MainConf = {};
	MainConf.Engine = Engine;
	return MainConf;
});

//Engine class
require(['card', 'gamestate', 'jquery', '../pixi.dev'], function (Card, GameState, $, PIXI) {
	Engine.width = 0;
	Engine.height = 0;
	Engine.mouseX = 0;
	Engine.mouseY = 0;
	Engine.window = null;
	Engine.stage = null; 

	// This function initialize all required dependencies for the game to work. Window, Resources etc

	function initialize() {
		// Create a stage and the renderer
		Engine.stage = new PIXI.Stage(0x000000, true);
		Engine.window = new PIXI.WebGLRenderer(Engine.width, Engine.height);
		$("#game-window").html(Engine.window.view);
		console.log(Engine.stage);
		
		
		// Fire onReady after init stuff
		onReady();
	}

	function onReady() {
		console.log("Game is ready");
		
		// start the game loop
		gameLoop();
	}

	function onResize() {
		var width = $(window).width();
		var height = $(window).height();

	}

	function gameLoop() {
		requestAnimFrame(gameLoop);
		
		Engine.window.render(Engine.stage);
	}

	requestAnimFrame(gameLoop);


	// Run the game
	$(document).ready(initialize());
	$(window).resize(onResize);
});




