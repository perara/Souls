
$(window).ready(function () {
    console.log(this);

    function Souls() { }

    Souls.Properties =
    {
        Width: /*1920,*/ $(window).width(),
        Height: /*1080,*/ $(window).height() - $(".navbar").height(),
        x: 0,
        y: 0,
    }


    Souls.Game = new Phaser.Game(
        Souls.Properties.Width,
        Souls.Properties.Height,
        Phaser.AUTO,
        'game-window',
        {
            preload: preload,
            create: create,
            update: update,
        });

    var friendAndFoe;
    var enemies;

    function preload() {
        // Init resources config tree
        initResources()

        // Load all Image Resources
        var count = 0;
        $.each(Souls.Resources.Image.Data, function (key, val) {
            Souls.Game.load.image(key, Souls.Resources.Image.Location + val)
            Souls.Resources.Image.Indexes[val] = key;
        });

        // Create Groups (Object groups)
        Souls.Groups.Background = new Phaser.Group(Souls.Game);
        Souls.Groups.Card = new Phaser.Group(Souls.Game);
        Souls.Groups.CardDock = new Phaser.Group(Souls.Game);

    }

    function createBackground() {

        var background = new Phaser.Sprite(Souls.Game, 0, 0, 'Arena');
        background.width = Souls.Properties.Width;
        background.height = Souls.Properties.Height;
        Souls.Groups.Background.add(background);
    }



    CardGroup = function (game, action) {
        Phaser.Group.call(this, game);
        // The background with border
        var cSize = { width: 120, height: 150 };

        var cContainer = Souls.Game.add.graphics(200, 200);
        cContainer.beginFill(0x000000);
        cContainer.lineStyle(2, 0xC0EC0EE);
        cContainer.drawRect(0, 0, cSize.width, cSize.height);
        cContainer.endFill();
        cContainer.anchor = { x: 0.5, y: 0.5 };

        var cBackground = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.CardBackground]);
        cBackground.x = 3;
        cBackground.y = 3;
        cBackground.width = cSize.width - 5;
        cBackground.height = cSize.height - 5;

        // AbilityPane
        var cAbilityPane = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.Card_AbilityPane]);
        cAbilityPane.anchor = { x: 1, y: 0 };
        cAbilityPane.x = cSize.width;
        cAbilityPane.y = cSize.height / 2;
        cAbilityPane.width = cSize.width;
        cAbilityPane.height = cSize.height / 2;

        // CardFactory Health Image
        cHealth = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.Card_Health]);
        cHealth.anchor = { x: 0.5, y: 1 };
        cHealth.width = cSize.width / 4;
        cHealth.height = cSize.height / 4;
        cHealth.x = cSize.width;
        cHealth.y = cSize.height + (cHealth.height / 3)

        // CardFactory Mana Image
        cMana = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.Card_Mana]);
        cMana.anchor = { x: 0.5, y: 0.5 };
        cMana.width = cSize.width / 3 - 5;
        cMana.height = cSize.height / 3 - 15;
        cMana.x = 0;
        cMana.y = 0;

        // CardFactory Attack Image
        cAttack = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.Card_Attack]);
        cAttack.anchor = { x: 0.5, y: 1 };
        cAttack.width = cSize.width / 3 - 10;
        cAttack.height = cSize.height / 3 - 20;
        cAttack.x = 0;
        cAttack.y = cSize.height + (cAttack.height / 3)


        // CardFactory portrait border
        cPortraitBorder = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.CardImageBorder]);
        cPortraitBorder.anchor = { x: 0.5, y: 0.5 };
        cPortraitBorder.width = cSize.width;
        cPortraitBorder.height = cSize.height;
        cPortraitBorder.x = cSize.width / 2;
        cPortraitBorder.y = (cPortraitBorder.height / 6);

        // CardFactory portrait wrapper
        cPortraitWrapper = Souls.Game.add.graphics(0, 0);
        cPortraitWrapper.beginFill(0xFCCFFF);
        cPortraitWrapper.drawEllipse(0, 0, cPortraitBorder.width / 3.5, cPortraitBorder.height / 4 + 7);
        cPortraitWrapper.endFill();
        cPortraitWrapper.x = cContainer.x / 4 + 10;
        cPortraitWrapper.y = cContainer.y / 6 - 6;

        // CardFactory portrait image
        cPortraitImage = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.CardRobot]);
        cPortraitImage.anchor = { x: 0.5, y: 0.5 };
        cPortraitImage.x = cSize.width / 2;
        cPortraitImage.y = (cPortraitImage.height / 10);
        cPortraitImage.width = cPortraitBorder.width - 50
        cPortraitImage.height = cPortraitBorder.height - 50

        // Nameplate
        cNamePlate = Souls.Game.add.sprite(0, 0, Souls.Resources.Image.Indexes[Souls.Resources.Image.Data.Card_NamePane]);
        cNamePlate.x = 0;
        cNamePlate.y = (cSize.width / 8) * 3 - 2;
        cNamePlate.width = cSize.width;
        cNamePlate.height = (cSize.height / 5);

        // Name Text // TODO POSITIONING
        var cNameText = Souls.Game.add.text(0, 0, "Dr. Robotnik", {
            font: "14px Arial Black",
            fill: "black",
            wordWrap: true,
            align: 'center',
            wordWrapWidth: cNamePlate.width
        });
        cNameText.anchor.setTo(0, 0);        cNameText.x = cNamePlate.x + 10;        cNameText.y = cNamePlate.y + 7;


        // General Style for all of the Attack,Health and Mana thingies
        var globeTxtStyle = { font: "bold 16px Arial", fill: "#ffffff", align: "center", stroke: "#258acc", strokeThickness: 2 };

        var cManaText = Souls.Game.add.text(0, 0, "50", globeTxtStyle);
        cManaText.anchor.setTo(0.5, 0.5);        cManaText.x = cMana.x;        cManaText.y = cMana.y;        var cHealthText = Souls.Game.add.text(0, 0, "99", globeTxtStyle);
        cHealthText.anchor.setTo(0.5, 0.5);        cHealthText.x = cHealth.x;        cHealthText.y = cHealth.y - 10;        var cAttackText = Souls.Game.add.text(0, 0, "33", globeTxtStyle);
        cAttackText.anchor.setTo(0.5, 0.5);        cAttackText.x = cAttack.x;        cAttackText.y = cAttack.y - 10;        var cAbilityText = Souls.Game.add.text(0, 0, "Charge wildly into a nigger", {
            font: "14px Arial Black",
            fill: "black",
            wordWrap: true,
            align: 'center',
            wordWrapWidth: cAbilityPane.width
        });
        cAbilityText.anchor.setTo(0, 1);        cAbilityText.x = 10;        cAbilityText.y = cAbilityPane.x + (cAbilityPane.x / 5);
        console.log(cBackground);
        CardGroup.prototype.add(cBackground);
        CardGroup.prototype.add(cAbilityPane);
        CardGroup.prototype.add(cHealth);
        CardGroup.prototype.add(cMana);
        CardGroup.prototype.add(cAttack);
        CardGroup.prototype.add(cPortraitWrapper);
        CardGroup.prototype.add(cPortraitImage);
        CardGroup.prototype.add(cPortraitBorder);
        CardGroup.prototype.add(cNamePlate);
        CardGroup.prototype.add(cAbilityText);

        // Text
        CardGroup.prototype.add(cNameText);
        CardGroup.prototype.add(cManaText)
        CardGroup.prototype.add(cHealthText);
        CardGroup.prototype.add(cAttackText);
        cPortraitImage.mask = cPortraitWrapper;


    };

    CardGroup.prototype = Object.create(Phaser.Group.prototype);
    CardGroup.prototype.constructor = CardGroup;



    function createCard() {
        CardGroup(Souls.Game, function () {
            console.log("rara");
        });
    }



    function create() {

        // Add the background
        createBackground();

        // Construct Card
        createCard();

    }    function update() {

    }    function initResources() {        ////////////////////////////////////        ////////////////////////////////////        ///////////////////////////////////        Souls.Resources =
            {
                Image:
                    {
                        Location: "Content/Images/",
                        Data:
                        {
                            Rat: "rat.png",

                            // Background Stuff
                            Arena: "arena.jpg",

                            // Card Stuff
                            CardBackground: "Card/card_bg.jpg",
                            CardRobot: "Card/dr_robot.png",
                            CardImageBorder: "Card/card_image_frame.png",
                            Card_Health: "Card/card_health.png",
                            Card_Mana: "Card/card_mana.png",
                            Card_Attack: "Card/card_attack.png",
                            Card_AbilityPane: "Card/card_ability_pane.png",
                            Card_NamePane: "Card/card_name_pane.png",
                        },
                        Indexes: {},
                    },

                Sound:
        {
            LOCATION: "Content/Sound/",
        }

            }        ////////////////////////////////////        ////////////////////////////////////        ///////////////////////////////////        Souls.Groups =            {
                Background: {},
                Card: {},
                CardDock: {},

            }

    } // Initresources end
});
