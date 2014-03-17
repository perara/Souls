define('chatService', ['jquery', 'jqueryUI', 'jquery.dialogExtend', "messages"], function ($, jQueryUI, dialogExtend, Message) {

    var that;
    ChatService = function (engine) {
        console.log("> Chat loaded!");
        that = this;
        this.engine = engine;
        this.socket = this.engine.chatSocket;

        // Chat window elements
        this.chatWindow = ChatService.prototype.chatWindow = $("#chat");

        // Create what is needed for the network to work
        this.responseAction = new Object();
        this.networkBuffer = ChatService.prototype.networkBuffer = new Array();
        this.message = Message;

        // Response callbacks
        this.RegisterResponseAction([1004], Response_NewGameRoom);

    };
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
        $("#chat_window").append(json.Payload);
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
          .bind("dialogextendload", function (evt) {  })
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



    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    //NETWORKBASEHERPDERPFUCKINHERITANCE.
    ////////////////////////////////////////////////////////////////
    ChatService.prototype.RegisterResponseAction = function (responseArray, func) {
        for (var response in responseArray) {
            this.responseAction[responseArray[response]] = func;
        }
    }

    ChatService.prototype.GetResponseAction = function (responseId) {
        if (!this.responseAction[responseId]) {
            console.log("Could not find RESPONSE!" + responseId)

        }
        return this.responseAction[responseId];
    }


    ChatService.prototype.Process = function () {
        if (ChatService.prototype.networkBuffer.length > 0) {
            // Get a packet
            var packet = ChatService.prototype.networkBuffer.shift();

            if (!!this.GetResponseAction(packet.Type))
                this.GetResponseAction(packet.Type)(packet);
        }

    }

    ChatService.prototype.Send = function (json) {
        this.socket.send(json);
    }

    ChatService.prototype.Connect = function () {
        this.socket.connect();

        // Recieves all Data from server
        this.socket.onMessage(ChatService.prototype.TrafficHandler);
    }


    ChatService.prototype.TrafficHandler = function (json) {
        ChatService.prototype.networkBuffer.push(JSON.parse(json.data));
    }









    return ChatService;
});