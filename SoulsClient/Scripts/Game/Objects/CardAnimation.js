define("cardanimation", ['tweenjs', 'easeljs', 'asset'], function (_, CreateJS, Asset) {

    CardAnimation = function () {

    }

    CardAnimation.prototype.constructor = CardAnimation;


    /// <summary>
    /// Animation function for Attack which resides in Card.JS
    /// </summary>
    /// <param name="attacker">The attacker card object</param>
    /// <param name="defender">The dender card object</param>
    /// <param name="attackerData">JSONformatted attack data from server</param>
    /// <param name="defenderData">JSONformatted defender data from server</param>
    /// <param name="callbacks">Callbacks which is defined in Card.js->Attack</param>
    CardAnimation.prototype.Attack = function (attacker, defender, attackerData, defenderData, callbacks) {

        console.log(attackerData)
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

        // Call the sequence :)
        AttackerCharge();


        // This is the tween which animates the Attacker's charge
        function AttackerCharge() {

            // This is the temporary tween object
            var values = attackerOrigin;

            attacker.interactive = false;

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
                callbacks.ChangeHealth();

                // Run the DEATH animation
                if (defenderData.isDead) {
                    DeathAnimation(defender);
                }
                else {
                    DefenderShake(defender);
                }
                if (attackerData.isDead) {
                    attacker.engine.SwapFromToGroup(attacker, "Attacker", (attacker.owner.isPlayer) ? "Card-Player" : "Card-Opponent");
                    CreateJS.Tween.removeTweens(values);
                    DeathAnimation(attacker);
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


            return attackTween;
        }

        function DeathAnimation(card) {

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

            return;
        }

        // This tween creates the "shaking" animation of the defender
        function DefenderShake() {
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

            return shakeTween;
        }


    }

    CardAnimation.prototype.Die = function (card) {

        var rot = Math.PI / 180;

        // Do the tween.
        var shakeTween = CreateJS.Tween.get(card, { override: false })
        .to({ x: 900, y: 700 }, 2000, CreateJS.Ease.elasticOut)
        .to(rot, 2000, CreateJS.Ease.elasticOut)


        return shakeTween;

    }

    CardAnimation.prototype.Defend = function (card) {



    }

    CardAnimation.prototype.ToSlot = function (card) {

    }






    return new CardAnimation();
});