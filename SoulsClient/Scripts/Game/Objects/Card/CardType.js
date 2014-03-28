﻿define("cardtype", ['asset', 'stopwatch'], function (Asset, Stopwatch) {

    CardType = function () {

    }
    CardType.prototype.constructor = CardType;


    CardType.Textures = {
        0: Asset.Textures.CARD_ERROR,
        1: Asset.Textures.CARD_DARKNESS,
        2: Asset.Textures.CARD_VAMPIRIC,
        3: Asset.Textures.CARD_LIGHTBRINGER,
        4: Asset.Textures.CARD_FEROCIOUS,
    }


    CardType.prototype.GetCardTexture = function (cardType) {
        if (!CardType.Textures[cardType])
            console.log("CARD_TYPE TEXTURE DOES NOT EXIST")

        return Asset.GetTexture(CardType.Textures[cardType]);
    }


    CardType.prototype.Abilities =
        {
            "TAUNT": function () {
                //DO LOGIC
            },
            "HEAL": function () {
                // DO LOGICl
            }
        }


    return new CardType();
});
