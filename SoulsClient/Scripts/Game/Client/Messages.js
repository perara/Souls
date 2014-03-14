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
                HEARTBEAT: 2,
            },
            Game: {
                QUEUE: 100,
                ATTACK: 200,
                USECARD: 201,
                NEXTROUND: 202,
                MOVE_CARD: 203
            },
            Chat: {
                ACTIVATE: 0,
                DEACTIVATE: 1,
                MESSAGE: 2,
                NEWROOM: 3,
                INVITE: 4,
                KICK: 5,
                LEAVE: 6
            }
        }

    Messages.prototype.Message =
        {
            GENERAL: {
                LOGIN: {
                    "Type": Messages.prototype.Type.General.LOGIN,
                    "Payload": {
                        "hash": Conf.Data.hash
                    }
                },
                LOGOUT: {
                    "Type": Messages.prototype.Type.General.LOGOUT,
                    "Payload": {
                        "hash": Conf.Data.hash
                    }
                },
                HEARTBEAT: {
                    "Type": Messages.prototype.Type.General.HEARTBEAT,
                    "Payload": {
                        "heartbeat": 0,
                        "last": 0,
                        "hash": Conf.Data.hash
                    }
                }
            },

            GAME: {
                ATTACK: {
                    "Type": Messages.prototype.Type.Game.ATTACK,
                    "Payload": {
                        "gameId": 0,
                        "hash": Conf.Data.hash,
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
                        "hash": Conf.Data.hash,
                        "cardId": undefined,
                        "slotId": undefined,
                        "card_genid": undefined,
                    }
                },
                NEXTCARD: {
                    "Type": Messages.prototype.Type.Game.NEXTROUND,
                    "Payload": {
                        "gameId": 0,
                        "hash": Conf.Data.hash
                    }
                },
                QUEUE: {
                    "Type": Messages.prototype.Type.Game.QUEUE,
                    "Payload": {
                        "hash": Conf.Data.hash
                    }
                },
                MOVE_CARD: {
                    "Type": Messages.prototype.Type.Game.MOVE_CARD,
                    "Payload": {
                        "gameId" : undefined,
                        "cid": undefined,
                        "x": undefined,
                        "y": undefined,
                        "hash": Conf.Data.hash
                    }
                }
            },
            CHAT: {
                ACTIVATE: {
                    "Type": Messages.prototype.Type.Chat.ACTIVATE,
                    "Payload": {
                        "id": 0, /// TODO
                        "hash": Conf.Data.hash
                    }
                },
                DEACTIVATE: {
                    "Type": Messages.prototype.Type.Chat.DEACTIVATE,
                    "Payload": {
                        "id": 0, //// TODO
                        "hash": Conf.Data.hash
                    }
                },
                MESSAGE: {
                    "Type": Messages.prototype.Type.Chat.MESSAGE,
                    "Payload": {
                        "room": 0, ///// TODO
                        "hash": Conf.Data.hash, //// TODO
                        "message": "HELLO TODO MESSAGE" ///// TODO
                    }
                },
                NEWROOM: {
                    "Type": Messages.prototype.Type.Chat.NEWROOM,
                    "Payload": {
                        "hash": Conf.Data.hash
                    }
                },
                INVITE: {
                    "Type": Messages.prototype.Type.Chat.INVITE,
                    "Payload": {
                        "room": 0, ///// TODO
                        "name": "HanselDEMO", ///// TODO
                        "hash": Conf.Data.hash
                    }
                },
                KICK: {
                    "Type": Messages.prototype.Type.Chat.KICK,
                    "Payload": {
                        "room": 0, //// TODO
                        "name": "HanselDEMO", //// TODO
                        "hash": Conf.Data.hash
                    }
                },
                LEAVE: {
                    "Type": Messages.prototype.Type.Chat.LEAVE,
                    "Payload": {
                        "room": 0, ///// TODO
                        "hash": Conf.Data.hash ///// todo
                    }
                }
            }
        };




    return Messages;
});