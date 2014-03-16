define('chat', ['jquery', 'jqueryUI', 'jquery.dialogExtend', 'messages'], function ($, jQueryUI, dialogExtend, Message) {

    Chat = function (engine) {
        console.log("> Chat loaded!");
        this.engine = engine;
        this.socket = this.engine.chatSocket;

    };



    Chat.prototype.Connect = function () {
        this.socket.connect();

        // Recieves all Data from server
       // this.socket.onMessage(TrafficHandler);
    }

    Chat.prototype.Login = function () {
        this.socket.send(Message.CHAT.CHAT_LOGIN);
    }

    Chat.prototype.OpenChatWindow = function () {

        $("#game-window").append(
            "<div id='chat'>" +
            "<textarea id='chat_window' disabled style='float:left'></textarea>" +
            "<textarea id ='chat_writing' placeholder='Write here...' style=''></textarea><br>" +
            "<select disabled multiple class='form-control' style='width:auto; display:inline'>" +
            "<option>1</option>" +
            "<option>2</option>" +
            "<option>3</option>" +
            "<option>4</option>" +
            "<option>5</option>" +
            "</select>" +
            "</div>");
        $("#chat").dialog();

        $('textarea#chat_writing').keydown(function (e) {
            if (e.keyCode === 13 && e.ctrlKey) {
                $(this).val(function (i, val) {
                    return val + "\n";
                });
            }
        }).keypress(function (e) {
            if (e.keyCode === 13 && !e.ctrlKey) {
                $("#chat_window").append("[NAME]:" + $(this).val() + "\n");
                $(this).val(undefined);
                return false;
            }
        }).keyup(function (e) {
            if (e.keyCode === 17) {
                ctrlKeyDown = false;
            }
        });

        //Bind to event by type
        //NOTE : You must bind() the <dialogextendload> event before dialog-extend is created
        $("#chat")
          .bind("dialogextendload", function (evt) { console.log("LOAD"); })
            .dialogExtend({
                "closable": true,
                "maximizable": true,
                "minimizable": true,
                "collapsable": true,
                "dblclick": "collapse",
                // "titlebar": "transparent",
                "minimizeLocation": "right",
                "icons": {
                    "close": "ui-icon-circle-close",
                    "maximize": "ui-icon-circle-plus",
                    "minimize": "ui-icon-circle-minus",
                    "collapse": "ui-icon-triangle-1-s",
                    "restore": "ui-icon-bullet"
                },
                "load": function (evt, dlg) { },
                "beforeCollapse": function (evt, dlg) { },
                "beforeMaximize": function (evt, dlg) { },
                "beforeMinimize": function (evt, dlg) { },
                "beforeRestore": function (evt, dlg) { },
                "collapse": function (evt, dlg) { },
                "maximize": function (evt, dlg) { },
                "minimize": function (evt, dlg) { },
                "restore": function (evt, dlg) { }
            });
    };

    Chat.prototype.RequestNewRoom = function () {

    };

    Chat.prototype.RequestInvite = function () {

    };

    Chat.prototype.RequestKick = function () {

    };

    Chat.prototype.constructor = Chat;

    return Chat;
});