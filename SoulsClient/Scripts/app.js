var form = document.getElementById('message-form');
var messageField = document.getElementById('message');
var messagesList = document.getElementById('messages');
var closeBtn = document.getElementById('close');
var socketStatus = document.getElementById('status');
var gameId;

$(document).ready(function () {
    $("body").css("padding-bottom", "70px", "important");

    // Whenever its connected
    sClient.onOpen(function (event) {

        $("#status").html('Connected to: ' + event.currentTarget.URL);
        $("#status").attr("class", "open");

    });

    sClient.receive(function (packet) {
        var message = packet.data;
        $("#messages").append('<li class="received"><span>Received:</span>' + packet.data + '</li>');
        console.log(JSON.parse(packet.data));
    });


    // Send a message when the form is submitted.

    $("#message-form").submit(function (e) {
        e.preventDefault();

        var txaMessage = $("#message").val();
        //var cmbMessage = $("#select_query").val();

       
        //if (txaMessage.length < 5) {
        sClient.send(txaMessage);
        console.log(txaMessage);

        $("#messages").append('<li class="sent"><span>ComboQuery Sent:</span>' + txaMessage + '</li>');
            $('#select_query option:eq(0)').prop('selected', true);
        return false;
    });

    $("#select_query").on("change", function () {
        $("#message").val($("#select_query").val());
    });




    // Close the WebSocket connection when the close button is clicked.
    $("#close").on("click", function (e) {
        e.preventDefault();
        socket.close();
        return false;
    });


    var room = 0;
    var thisPlayerId = 0;
    var playerName = "per";
    var chatMessage = "HELLO IS THIS ROOM?"
    var inputMessage = "HER";

    General = {
        LOGIN: 0,
        LOGOUT: 1,
        GAME: 2,
        CHAT: 3,
        HEARTBEAT: 4,
    }

    Game = {
        QUEUE: 100,
        ATTACK: 200,
        USECARD: 201,
        NEXTROUND: 202,
    }
    Chat = {
        ENABLE: 1000,
        DISABLE: 1001,
        MESSAGE: 1002,
        NEWROOM: 1003,
        INVITE: 1004,
        KICK: 1005,
        LEAVE: 1006
    }

    var GENERAL = 0;
    var GAME = 1;
    var CHAT = 2;

    var obj = new Array();
    obj.GENERAL = {};
    obj.GAME = {};
    obj.CHAT = {};

 // CONF is in Index cshtml

    obj.GENERAL.Login = {
        "Type": General.LOGIN,
        "Payload": {
            "hash": Conf.Data.hash
        }
    }; obj.GENERAL.logout = {
        "Type": General.LOGOUT,
        "Payload": {
            "hash": Conf.Data.hash
        }
    }; obj.GAME.queue = {
        "Type": Game.QUEUE,
        "Payload": {
            "hash": Conf.Data.hash
        }
    }; obj.GAME.Attack = {
        "Type": Game.ATTACK,
        "Payload": {
            "gameId": 0,
            "hash": Conf.Data.hash,
            "attacker": 2,
            "defender": 1,
            "cardAttackPlayer": false,
            "playerAttackCard": false
        }
    }; obj.GAME.UserCard = {
        "Type": Game.USECARD,
        "Payload": {
            "gameId": 0,
            "hash": Conf.Data.hash,
            "slot": 2,
            "card": 1
        }
    }; obj.GAME.nextcard = {
        "Type": Game.NEXTROUND,
        "Payload": {
            "gameId": 0,
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.Enable = {
        "Type": Chat.ENABLE,
        "Payload": {
            "id": 0, /// TODO
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.Disable = {
        "Type": Chat.DISABLE,
        "Payload": {
            "id": 0, //// TODO
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.messagee = {
        "Type": Chat.MESSAGE,
        "Payload": {
            "room": 0, ///// TODO
            "hash": Conf.Data.hash, //// TODO
            "message": chatMessage ///// TODO
        }
    }; obj.CHAT.newroom = {
        "Type": Chat.NEWROOM,
        "Payload": {
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.invite = {
        "Type": Chat.INVITE,
        "Payload": {
            "room": 0, ///// TODO
            "name": playerName, ///// TODO
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.kick = {
        "Type": Chat.KICK,
        "Payload": {
            "room": 0, //// TODO
            "name": playerName, //// TODO
            "hash": Conf.Data.hash
        }
    }; obj.CHAT.leave = {
        "Type": Chat.LEAVE,
        "Payload": {
            "room": 0, ///// TODO
            "hash": Conf.Data.hash ///// todo
        }
    };







    for (var index in obj) {
        var option = document.createElement("option");
        option.text = index;
        option.val = index;
        $("#service").append(option);

    };

    $("#service").on("change", function () {
        var key = $(this).val();

        $("#select_query").html("");


        var option = document.createElement("option");
        option.text = "Velg: ";
        $("#select_query").append(option);
        for (var index in obj[key]) {
            console.log(index);

            var option = document.createElement("option");
            option.text = index;
            option.value = JSON.stringify(obj[key][index]);

            $("#select_query").append(option);

        }

    });




    //setTimeout(sClient.send(JSON.stringify(clientCommands.AUTHENTICATE)), 300);
    //setTimeout(sClient.send(JSON.stringify(clientCommands.QUEUE)), 900);

});