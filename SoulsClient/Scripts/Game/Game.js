
function Souls() { }

// Data storage with "random data"
Souls.DataStorage = {};
Souls.DataStorage.Width = 1920;
Souls.DataStorage.Height = 1080;

Souls.Objects = {};


// Load Resource Loader
Souls.ResourceLoader = {}; //new AssetLoader();

// Method where all functions are stored
Souls.Func = {};

// CardFactory Node
Souls.Card = {};

// Create the window
Souls.Window = {};
Souls.Window.Renderer = new PIXI.WebGLRenderer(
    Souls.DataStorage.Width,
    Souls.DataStorage.Height
    );


// Main stage
Souls.Window.Stage = new PIXI.Stage; // The stage

// Container for each of the Object groups on the screen
Souls.Window.ViewGroup = {};


Souls.Func.Init = function () {
    // Initialize the Assetloader
    Souls.ResourceLoader = new AssetLoader(); //TODO callaback with progress??

    // Initialize Card
    Souls.Card.Factory = new CardFactory(Souls.ResourceLoader);

    // Add the window to game div
    $("#game-window").append(Souls.Window.Renderer.view);

    // Create all Displaye Objects
    Souls.Func.InitDisplayObjects();

}

Souls.Func.InitDisplayObjects = function()
{
    // Create the viewgroup
    Souls.Window.ViewGroup.Cards = new PIXI.DisplayObjectContainer();
    Souls.Window.ViewGroup.CardSnap = new PIXI.DisplayObjectContainer();
    Souls.Window.ViewGroup.Background = new PIXI.DisplayObjectContainer();

    // Add to stage
    Souls.Window.Stage.addChild(Souls.Window.ViewGroup.Background)
    Souls.Window.Stage.addChild(Souls.Window.ViewGroup.CardSnap)
    Souls.Window.Stage.addChild(Souls.Window.ViewGroup.Cards)

    // Add a Targeter
    var targeter = Targeter.Get();
    Souls.Window.Stage.addChild(targeter);
    Souls.Objects['targeter'] = targeter;


}


Souls.Func.Draw = function () {
    // Render the frame
    Souls.Window.Renderer.render(Souls.Window.Stage);
}

Souls.Card.Spawn = function (x, y) {
    Souls.Window.ViewGroup.Cards.addChild(
        CardFactory.prototype.generateCard(
            x,
            y,
            Souls.Func.Action.Card
        ));
}


Souls.Func.SetBackground = function () {

    var Sprite = Souls.ResourceLoader.Resource.Texture.Get(
        Souls.ResourceLoader.Resource.Texture.Enum.Arena
        )

    var bg = Sprite;
    bg.position.x = 0;
    bg.position.y = 0;
    bg.width = Souls.DataStorage.Width;
    bg.height = Souls.DataStorage.Height;
    Souls.Window.ViewGroup.Background.addChild(bg);


    $.each(Souls.Card.Factory.CardSlots.GenerateCardSlots(), function (key, val) {
        Souls.Window.ViewGroup.CardSnap.addChild(val);
    });

}

Souls.Func.GameLoop = function () {

    requestAnimationFrame(loop);
    function loop() {

        // Process everything with callback.
        Souls.Func.Process(function () {

        });

        // Draw Stuff
        Souls.Func.Draw();


        // New Frame
        requestAnimationFrame(loop);
    }
}

// Function which do all of the logic
Souls.Func.Process = function (callback) {



    callback();
}

///////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////
//********Callbacks from around the application***********/
///////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////

// Card
Souls.Func.Action = {};
Souls.Func.Action.Card = {};

// When a card is clicked
Souls.Func.Action.Card.Click = function (card) {
    // Show Cardslots which is not already occupied
    Souls.Window.ViewGroup.CardSnap.visible = true;
    $.each(Souls.Window.ViewGroup.CardSnap.children, function (key, obj) {
        if (card.slot.list[obj.id] != null) obj.visible = false;
        else obj.visible = true;
    });


    // Add card to last inded in the displaye (on top)
    Souls.Window.ViewGroup.Cards.removeChild(card);
    Souls.Window.ViewGroup.Cards.addChild(card);

    // positions of the click
    card.startClick =
        {
            x: card.x,
            y: card.y,
        }


}

Souls.Func.Action.Card.Release = function (card) {

    // Hide Cardslots
    Souls.Window.ViewGroup.CardSnap.visible = false;


    var resetPosition = function () {
        card.x = card.startClick.x;
        card.y = card.startClick.y;
    }


    // Must not have slot
    if (card.slot.id == null && card.slot.hover != null) {
        // Check if there is a card already in that slot.
        if (card.slot.list[card.slot.hover.id] == false ||
            card.slot.list[card.slot.hover.id] === undefined) {

            PIXIE.ScaleGraphics(1, 1, card.slot.hover);

            // Set card position to the hover position
            card.x = card.slot.hover.x;
            card.y = card.slot.hover.y;

            // Set slot to "used" in the slot list
            card.slot.list[card.slot.hover.id] = true;
            
            // Set the used slot it to the slot the card was hovered over
            card.slot.id = card.slot.hover.id;

        }
        else {
            resetPosition();
        }


    }


}


var resizeView = function () {
    Souls.Window.Renderer.view.style.width = $(window).width() + "px";
    Souls.Window.Renderer.view.style.height = $(window).height() - $(".navbar").height() + "px";
}

$(window).on("resize", resizeView);
$(window).on("ready", resizeView);

// Event handlers






// Restructure V2 . soon TM
/*
Souls.Engine

Souls.Callbacks

Souls.Type

Souls.Objects.
*/



















