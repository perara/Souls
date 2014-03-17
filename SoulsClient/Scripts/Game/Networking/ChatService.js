define('chatService', ['jquery', 'jqueryUI', 'jquery.dialogExtend', "networkBase"], function ($, jQueryUI, dialogExtend, NetworkBase) {

    var that;
    ChatService = function (engine) {
        NetworkBase.call(this);
        console.log("> Chat loaded!");
        that = this;
        this.engine = engine;
        this.socket = this.engine.chatSocket;


        /// Chat
        this.RegisterResponseAction(["1004"], Response_NewGameRoom);

    };
    ChatService.prototype = Object.create(NetworkBase.prototype);
    ChatService.prototype.constructor = ChatService;

    ChatService.prototype.Login = function () {
        this.socket.send(this.message.CHAT.CHAT_LOGIN);
    }


    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    ////////////////////////////CHAT-RESPONSES////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    function Response_NewGameRoom(json) {

        console.log(json);

    }




    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////CHAT-REQUESTS///////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    ChatService.prototype.RequestNewGameRoom = function () {
        var json = that.message.CHAT.NEWGAMEROOM;
        this.socket.send(json);
    };

    ChatService.prototype.RequestInvite = function () {

    };

    ChatService.prototype.RequestKick = function () {

    };



















    ChatService.prototype.OpenChatWindow = function () {

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


    return ChatService;
});