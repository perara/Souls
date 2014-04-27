define('chatService', ['jquery', 'jqueryUI', 'jquery.dialogExtend', "messages", "asset"], function ($, jQueryUI, dialogExtend, Message, Asset) {

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
        this.RegisterResponseAction([1003], Response_Message);
        this.RegisterResponseAction([1095], Response_ClientDisconnected);
        this.RegisterResponseAction([1094], Response_ClientConnected);
        this.RegisterResponseAction([1093], Response_GetAttendees)

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
        $(".chat-messages").append("[" + json.Payload.name + "]: " + json.Payload.message + "\n");

        $(".chat-input").attr("chRoomId", json.Payload.chRoomId);

    }

    function Response_Message(json) {
        $(".chat-messages").append("[" + json.Payload.name + "]: " + json.Payload.message + "<br>");
        Asset.GetSound(Asset.Sound.CHAT_MESSAGE).play();
    }

    function Response_ClientDisconnected(json) // 1095
    {
        $(".chat-messages").append("[SERVER]: " + json.Payload + "\n");
    }

    function Response_ClientConnected(json) // 1094
    {
        $(".chat-messages").append("[SERVER]: Player " + json.Payload.name + " connected! \n");
        $(".chat-input").attr("chRoomId", json.Payload.room);

        that.GetAttendees(json.Payload.room);

    }

    function Response_GetAttendees(json) // 1093
    {
        $(".chat-clients").html("");

        $.each(json.Payload, function (key, value) {
            $(".chat-clients").append(value + "<br>");
        });
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

    ChatService.prototype.Message = function (roomId, msg) // 1002
    {
        var json = this.message.CHAT.MESSAGE;

        json.Payload.room = roomId;
        json.Payload.message = msg;


        this.Send(json);

        // Also get room attendees (update)
        this.GetAttendees(roomId);
    }

    ChatService.prototype.GetAttendees = function (roomId) // 1010
    {
        var json = this.message.CHAT.CHAT_LIST_ATTENDEES;

        json.Payload.room = roomId;

        this.Send(json);
    }



    ChatService.prototype.OpenChatWindow = function () {
        var that = this;

        $("#game-window").append("" +
                "<div class='chat-window'>" +
                    "<div class='chat-content-wrapper col-lg-12'>" +

                        "<div class='chat-messages col-lg-9 col-md-9 col-sm-9'>" +
                        "</div>" +

                        "<div class='chat-clients col-lg-3 col-md-3 col-sm-3'>" +
                        "</div>" +

                        "<div class='chat-input-div col-lg-12'>" +
                            "<textarea class='chat-input form-control' placeholder='Write here...'></textarea>" +
                        "</div>" +
                    "</div>" +
                "</div>"
            );

        $(".chat-window").tabs();
        $(".chat-window").dialog({
            height: 325,
            width: 500,
            "title": "Chat"

        })
            .dialogExtend({
                "closable": false,
                "maximizable": true,
                "minimizable": true,
                "collapsable": true,
                "dblclick": "minimize",
                "titlebar": "transparent",
                "minimizeLocation": "right",
                "icons": {
                    "close": "ui-icon-circle-close",
                    "maximize": "ui-icon-circle-plus",
                    "minimize": "ui-icon-circle-minus",
                    "collapse": "ui-icon-triangle-1-s",
                    "restore": "ui-icon-bullet"
                },
                "load": function (evt, dlg) {
                    $(".chat-window").dialogExtend("minimize");
                },
                "beforeCollapse": function (evt, dlg) { },
                "beforeMaximize": function (evt, dlg) { },
                "beforeMinimize": function (evt, dlg) { },
                "beforeRestore": function (evt, dlg) { },
                "collapse": function (evt, dlg) { },
                "maximize": function (evt, dlg) { },
                "minimize": function (evt, dlg) { },
                "restore": function (evt, dlg) { }
            });
        $(".chat-window").parent().css("opacity", 0.2);

        $(".chat-window").parent().mouseenter(function () {
            $(this).fadeTo("fast", 1);

        });
        $(".chat-window").parent().mouseleave(function () {
            $(this).fadeTo("fast", 0.2);
        });

        $('textarea.chat-input').keydown(function (e) {
            if (e.keyCode === 13 && e.ctrlKey) {
                $(this).val(function (i, val) {
                    return val + "\n";
                });
            }
        }).keypress(function (e) {
            if (e.keyCode === 13 && !e.ctrlKey) {

                // Send message!
                that.Message($(this).attr("chroomid"), $(this).val());

                $(this).val(undefined);

                return false;
            }
        }).keyup(function (e) {
            if (e.keyCode === 17) {
                ctrlKeyDown = false;
            }
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