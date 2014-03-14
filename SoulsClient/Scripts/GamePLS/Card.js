define("card", ["../pixi.dev"], function (PIXI) {

    Card = function (texture) {
        console.log((texture === undefined) ? "CARD:JS Error! Missing texture" : "");
        PIXI.Sprit.call(this, texture);
    };

    // Constructor
    Card.prototype = Object.create(PIXI.Sprite.prototype);
    Card.prototype.constructor = Card;


});