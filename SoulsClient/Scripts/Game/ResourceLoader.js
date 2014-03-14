// DEFINE Types of resources
AssetLoader.prototype.Resource = {};
// Image
AssetLoader.prototype.Resource.Texture = {};
AssetLoader.prototype.Resource.Loader = {};
// Sound
// Here


// DEFINE Path for each of the resources
AssetLoader.prototype.AssetPath = {};
AssetLoader.prototype.AssetPath.Texture = "Content/Images/";


function AssetLoader() {
    AssetLoader.prototype.initLoader();
    console.log("AssetLoader: OK");
}

AssetLoader.prototype.initLoader = function () {

    var fileNames = new Array()
    $.each(AssetLoader.prototype.Resource.Texture.Enum, function (key, index) {
        fileNames.push(AssetLoader.prototype.AssetPath.Texture + index);
    });

    new PIXI.AssetLoader(fileNames)
}

AssetLoader.prototype.Resource.Texture.Get = function (textureEnumItem) {
    return new PIXI.Sprite(PIXI.Texture.fromImage(AssetLoader.prototype.AssetPath.Texture + textureEnumItem.filename));
}

AssetLoader.prototype.Resource.Texture.Enum =
    {
        Rat:
        {
            filename: "rat.png"
        },
        Arena:
        {
            filename: "arena.jpg"
        },
        CardBackground:
        {
            filename: "Card/card_bg.jpg"
        },
        CardRobot:
        {
            filename: "Card/dr_robot.png"
        },
        CardImageBorder:
        {
            filename: "Card/card_image_frame.png"
        },
        Card_Health:
        {
            filename: "Card/card_health.png"
        },
        Card_Mana:
        {
            filename: "Card/card_mana.png"
        },
        Card_Attack:
        {
            filename: "Card/card_attack.png"
        },
        Card_AbilityPane:
        {
            filename: "Card/card_ability_pane.png"
        },
        Card_NamePane:
        {
            filename: "Card/card_name_pane.png"
        },
    }



/////USAGE EXAMPLE
// Get resource
// AssetLoader.prototype.Resource.Texture.Get(AssetLoader.prototype.Resource.Texture.Enum.Rat)