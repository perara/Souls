define('messages', ['conf'], function (Conf) {

    function Messages() {
        console.log("> Messages Loaded!");

        // Add the request tree
        this.Request = Messages.Request;
    }


    Messages.prototype.Type =
        {
            General: {
                LOGIN: 0,
                LOGOUT: 1,
                HEARTBEAT: 2
            },
            Game: {
                NORMAL_QUEUE: 100,
                PRACTICE_QUEUE: 101,
                ATTACK: 200, // CARD CARD attack
                USECARD: 201,
                NEXT_TURN: 226,
                NOT_YOUR_TURN: 202,
                MOVE_CARD: 203,
                RELEASE_CARD: 204
            },
            Chat: {
                ENABLE: 1000,
                DISABLE: 1001,
                MESSAGE: 1002,
                NEWROOM: 1003,
                INVITE: 1004,
                KICK: 1005,
                LEAVE: 1006,
                NEWGAMEROOM: 1009,
                CHAT_LIST_ATTENDEES: 1010
            }
        }

    Messages.prototype.GENERAL =
        {
            LOGIN: {
                "Type": Messages.prototype.Type.General.LOGIN,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            LOGOUT: {
                "Type": Messages.prototype.Type.General.LOGOUT,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            HEARTBEAT: {
                "Type": Messages.prototype.Type.General.HEARTBEAT,
                "Payload": {
                    "heartbeat": 0,
                    "last": 0,
                    "hash": Conf.hash
                }
            }
        };

    Messages.prototype.GAME =
        {
            ATTACK: {
                "Type": Messages.prototype.Type.Game.ATTACK,
                "Payload": {
                    "source": undefined,
                    "target": undefined,
                    "type" : -1
                }
            },
            USECARD: {
                "Type": Messages.prototype.Type.Game.USECARD,
                "Payload": {
                    "cid": undefined,
                    "slotId": undefined
                }
            },
            NEXT_TURN: {
                "Type": Messages.prototype.Type.Game.NEXT_TURN,
                "Payload": {
                }
            },
            NORMAL_QUEUE: {
                "Type": Messages.prototype.Type.Game.NORMAL_QUEUE,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            PRACTICE_QUEUE: {
                "Type": Messages.prototype.Type.Game.PRACTICE_QUEUE,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            MOVE_CARD: {
                "Type": Messages.prototype.Type.Game.MOVE_CARD,
                "Payload": {
                    "gameId": undefined,
                    "cid": undefined,
                    "x": undefined,
                    "y": undefined,
                    "hash": Conf.hash
                }
            },
            RELEASE_CARD: {
                "Type": Messages.prototype.Type.Game.RELEASE_CARD,
                "Payload": {
                    "gameId": undefined,
                    "cid": undefined,
                    "hash": Conf.hash
                }
            }
        };
    Messages.prototype.CHAT =
        {
            ENABLE: {
                "Type": Messages.prototype.Type.Chat.ENABLE,
                "Payload": {
                    "id": 0, /// TODO
                    "hash": Conf.hash
                }
            },
            DISABLE: {
                "Type": Messages.prototype.Type.Chat.DISABLE,
                "Payload": {
                    "id": 0, //// TODO
                    "hash": Conf.hash
                }
            },
            MESSAGE: {
                "Type": Messages.prototype.Type.Chat.MESSAGE,
                "Payload": {
                    "room": undefined,
                    "message": undefined
                }
            },
            NEWROOM: {
                "Type": Messages.prototype.Type.Chat.NEWROOM,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            NEWGAMEROOM: {
                "Type": Messages.prototype.Type.Chat.NEWGAMEROOM,
                "Payload": {
                    //"hash": Conf.hash
                }
            },
            INVITE: {
                "Type": Messages.prototype.Type.Chat.INVITE,
                "Payload": {
                    "room": 0, ///// TODO
                    "name": "HanselDEMO", ///// TODO
                    "hash": Conf.hash
                }
            },
            KICK: {
                "Type": Messages.prototype.Type.Chat.KICK,
                "Payload": {
                    "room": 0, //// TODO
                    "name": "HanselDEMO", //// TODO
                    "hash": Conf.hash
                }
            },
            LEAVE: {
                "Type": Messages.prototype.Type.Chat.LEAVE,
                "Payload": {
                    "room": 0, ///// TODO
                    "hash": Conf.hash ///// todo
                }
            },
            CHAT_LOGIN: {
                "Type": Messages.prototype.Type.General.LOGIN,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            CHAT_LOGOUT: {
                "Type": Messages.prototype.Type.General.LOGOUT,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            CHAT_LIST_ATTENDEES: {
                "Type": Messages.prototype.Type.Chat.CHAT_LIST_ATTENDEES,
                "Payload": {
                    "hash": Conf.hash,
                    "room" : undefined
                }
            }

};




return new Messages();
});