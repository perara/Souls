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
                QUEUE: 100,
                ATTACK: 200,
                USECARD: 201,
                NEXTROUND: 202,
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
                CHAT_LOGIN: 1007,
                CHAT_LOGOUT: 1008
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
                    "gameId": 0,
                    "hash": Conf.hash,
                    "attacker": 2,
                    "defender": 1,
                    "cardAttackPlayer": false,
                    "playerAttackCard": false
                }
            },
            USECARD: {
                "Type": Messages.prototype.Type.Game.USECARD,
                "Payload": {
                    "gameId": undefined,
                    "hash": Conf.hash,
                    "cardId": undefined,
                    "slotId": undefined,
                    "card_genid": undefined
                }
            },
            NEXTCARD: {
                "Type": Messages.prototype.Type.Game.NEXTROUND,
                "Payload": {
                    "gameId": 0,
                    "hash": Conf.hash
                }
            },
            QUEUE: {
                "Type": Messages.prototype.Type.Game.QUEUE,
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
                    "room": 0, ///// TODO
                    "hash": Conf.hash, //// TODO
                    "message": "HELLO TODO MESSAGE" ///// TODO
                }
            },
            NEWROOM: {
                "Type": Messages.prototype.Type.Chat.NEWROOM,
                "Payload": {
                    "hash": Conf.hash
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
                "Type": Messages.prototype.Type.Chat.CHAT_LOGIN,
                "Payload": {
                    "hash": Conf.hash
                }
            },
            CHAT_LOGOUT: {
                "Type": Messages.prototype.Type.Chat.CHAT_LOGOUT,
                "Payload": {
                    "hash": Conf.hash
                }
            }
        };




    return new Messages();
});