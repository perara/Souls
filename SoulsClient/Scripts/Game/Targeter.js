

function Targeter() { }
Targeter.prototype.constructor = Targeter.prototype;

Targeter.Get = function()
{
    // Create the targeter
    var targeter = This.assetLoader.Resource.Texture.Get(This.assetLoader.Resource.Texture.Enum.Card_Attack);
    targeter.anchor = { x: 0.5, y: 0.5 };
    targeter.width = 50;
    targeter.height = 50;
    targeter.x = 0;
    targeter.y = 0;
    targeter.visible = false;

    return targeter
}