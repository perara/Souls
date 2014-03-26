define("cardtype", ['asset', 'stopwatch'], function (Asset, Stopwatch) {

    CardType = new function()
    {

    }
    CardType.prototype.constructor = CardType;

    CardType.Textures = {
        "DARKNESS": Asset.Textures.CARD_DARKNESS,
        "LIGHTBRINGER": Asset.Textures.CARD_LIGHTBRINGER,
        "FEROCIOUS": Asset.Textures.CARD_FEROCIOUS,
        "VAMPIRIC": Asset.Textures.CARD_VAMPIRIC
    }

    CardType.prototype.GetCardTexture = function(cardType)
    {
        if(!CardType.Textures[cardType])
            console.log("CARD_TYPE TEXTURE DOES NOT EXIST")

        return Asset.GetTexture(CardType.Textures[cardType]);
    }


    CardType.prototype.Abilities = 
        {
            "TAUNT" : function()
            {
                //DO LOGIC
            },
            "HEAL" : function()
            {
                // DO LOGICl
            }
        }


    return CardType;
});