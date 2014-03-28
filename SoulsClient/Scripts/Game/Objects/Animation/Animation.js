define("animation", ['tweenjs', 'easeljs', 'asset'], function (_, CreateJS, Asset) {

    Animation = function () {

    }

    Animation.prototype.constructor = Animation;
    Animation.prototype.Card = {};
    Animation.prototype.Player = {};




    Animation.prototype.Card.Defend = function (defender) {

        // Original Position and Scale of the defender
        var defenderOrigin = {
            scaleX: defender.scale.y,
            scaleY: defender.scale.x,
            x: defender.x,
            y: defender.y
        };

        var leftShake = { x: defenderOrigin.x - 25 };
        var rightShake = { x: defenderOrigin.x + 25 };
        var speed = 100;

        // Play the defender scream
        Asset.GetSound(Asset.Sound.DEFEND_1).play();

        // Do the tween.
        var shakeTween = CreateJS.Tween.get(defender, { override: false })
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(leftShake, speed, CreateJS.Ease.elasticOut)
        .to(rightShake, speed, CreateJS.Ease.elasticOut)
        .to(defenderOrigin, speed, CreateJS.Ease.elasticOut)
        .call(function () {
            if (defender.owner == defender.engine.player) {
                defender.interactive = true
            }

        })
    }

    Animation.prototype.Card.Attack = function (attacker, defender, callbacks) {

        // Distance between the cards
        var delta =
            {
                x: defender.x - attacker.x,
                y: defender.y - attacker.y
            }

        // Original Position and Scale of the attacker
        var attackerOrigin = {
            scaleX: attacker.scale.y,
            scaleY: attacker.scale.x,
            x: attacker.x,
            y: attacker.y
        };

        // Original Position and Scale of the defender
        var defenderOrigin = {
            scaleX: defender.scale.y,
            scaleY: defender.scale.x,
            x: defender.x,
            y: defender.y
        };

        // Set interactive to false on both
        attacker.interactive = false;
        defender.interactive = false;


        // This is the temporary tween object
        var values = attackerOrigin;

        // Change the card to the attacker group
        Asset.GetSound(Asset.Sound.ATTACK_1).play();

        // Swap group to the Attacker Group (Which is top level group)
        attacker.engine.SwapFromToGroup(attacker, (attacker.owner.isPlayer) ? "Card-Player" : "Card-Opponent", "Attacker");

        var attackTween = CreateJS.Tween.get(values, {
            override: false,
            onChange: function onChange(e) {
                attacker.scale.y = values.scaleY;
                attacker.scale.x = values.scaleX;
                attacker.x = values.x;
                attacker.y = values.y;
            }
        })
        // Jump Half Way
        .to(
        {
            scaleX: attacker.scale.y + 1,
            scaleY: attacker.scale.x + 1,
            x: attackerOrigin.x + delta.x,
            y: attackerOrigin.y + (delta.y / 2)
        }, 700)
        // Land on opponent
        .to(
        {
            scaleX: attackerOrigin.scaleX,
            scaleY: attackerOrigin.scaleY,
            x: attackerOrigin.x + delta.x,
            y: attackerOrigin.y + (delta.y / 1.5)
        }, 300)
        // Hits the card, call Defend on the target
        .call(function () {

            // Change health on the object
            callbacks.ChangeHealth();

            // Check alive status
            if (defender.isDead) {
                defender.Animation.Death(defender);
            }
            else {
                defender.Animation.Defend(defender);
            }

            // Check alive status attacker
            if (attacker.isDead) {
                attacker.engine.SwapFromToGroup(attacker, "Attacker", (attacker.owner.isPlayer) ? "Card-Player" : "Card-Opponent");
                CreateJS.Tween.removeTweens(values);
                attacker.Animation.Death(attacker);
            }


        })
        // Wait abit before returning
        .wait(200)

        // Return to origin position.
        .to(
        {
            scaleX: attackerOrigin.scaleX,
            scaleY: attackerOrigin.scaleY,
            x: attackerOrigin.x,
            y: attackerOrigin.y
        }, 150)
        // Animation done, call reset functions flags
        .call(function () {

            // Set back to correct group
            attacker.engine.SwapFromToGroup(attacker, "Attacker", (attacker.owner.isPlayer) ? "Card-Player" : "Card-Opponent");

            // Activate the interactive again if its a player
            if (attacker.owner.isPlayer) {
                attacker.interactive = true
            }
        });

    }



    Animation.prototype.Card.Death = function (card) {
        card.owner.holdingCard = undefined;
        card.inSlot.Reset();

        var values = {
            scaleY: card.scale.y,
            scaleX: card.scale.x,
            x: card.x,
            y: card.y,
            rotation: card.rotation
        }

        // Location of the card
        var xEndpoint = card.owner.engine.conf.width + 200;
        var yEndpoint = undefined;

        // Spins in different direction depending on attacker\defender
        yEndpoint = (card.owner.isPlayer) ? card.owner.engine.conf.height : 0;

        // Execute tween (Move, rotate, scale)
        var deathTween = CreateJS.Tween.get(values, {
            override: false,
            onChange: function onChange(e) {
                card.scale.y = values.scaleY;
                card.scale.x = values.scaleX;
                card.x = values.x;
                card.y = values.y;
                card.rotation = values.rotation;
            }
        })
        .to(
        {
            scaleX: 3,
            scaleY: 3,
            rotation: 10,
            x: xEndpoint,
            y: yEndpoint
        }, 1000)
        .call(function () {
            card.owner.cardManager.RemoveCard(card);
        })

        Asset.GetSound(Asset.Sound.DEFEND_1).play();

    }

    Animation.prototype.Card.MoveBack = function (c) {

        c.OrderOriginalPosition();
        var target =
            {
                x: c.position.originX,
                y: c.position.originY
            }

        CreateJS.Tween.get(c, { override: true })
            .to(target, 1000, CreateJS.Ease.elasticOut)
            .call(onComplete);

        function onComplete() {
            // Set Mount variable to true
            c.interactive = true;
            c._awaitRequest = false;
        }
    }

    Animation.prototype.Card.MoveTo = function () {

    }

    Animation.prototype.Card.PutIn = function () {
        // TODO
    }



    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ////////////////////////PLAYER STUFF///////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////

    Animation.prototype.Player.Death = function (player) {



        var die = CreateJS.Tween.get(player, { override: false })
        .to({rotation: 20}, 1000, CreateJS.Ease.QuadIn)


    }

    Animation.prototype.Player.Attack = function () {

    }

    Animation.prototype.Player.Defend = function (player) {

        var speed = 200;

        // Do the tween.
        var blink = CreateJS.Tween.get(player, { override: false })
        .call(function () { red() })
        .wait(speed)
        .call(function () { none() })
        .wait(speed)
         .call(function () { red() })
        .wait(speed)
        .call(function () { none() })
        .wait(speed)
        .call(function () { red() })
        .wait(speed)
        .call(function () { none() })
        .wait(speed)

        function red() {
            console.log(player);
            player.portrait.tint = 0xFF0000;
        }
        function none() {
            player.portrait.tint = 0xFFFFFF;
        }


    }









    return new Animation();
});