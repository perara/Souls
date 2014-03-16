define("inputmanager", ["jquery", "pixi"], function ($, pixi) {


    InputManager = function (engine) {

        this.engine = InputManager.prototype.engine = engine;

        this.position = InputManager.prototype.position =
            {
                x: 0,
                y: 0
            }



    }
    // Constructor
    InputManager.prototype.constructor = InputManager;


    InputManager.prototype.Process = function () {

    }




    return InputManager;

});