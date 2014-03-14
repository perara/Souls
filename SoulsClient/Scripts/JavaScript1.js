var x = {

    "Service": GENERAL,
    "ServiceData":
    {
        "Type": General.LOGIN,
        "hash": playerHash
    },

    "Service": GENERAL,
    "ServiceData":
    {
        "Type": General.LOGOUT,
        "hash": playerHash
    },
    "Service": General.GAME,
    "ServiceData":
    {
        "Type": Game.QUEUE,
        "Payload":
        {
            "hash": playerHash
        }
    },
    "Service": General.GAME,
    "ServiceData":
    {
        "Type": Game.ATTACK,
        "Payload":
        {
            "gameId": 0,
            "hash": playerHash,
            "attacker": 2,
            "defender": 1,
            "cardAttackPlayer": false,
            "playerAttackCard": false
        }
    },
    "Service": General.GAME,
    "ServiceData":
    {
        "Type": Game.USECARD,
        "Payload":
        {
            "gameId": 0,
            "hash": playerHash,
            "slot": 2,
            "card": 1
        }
    },
    "Service": General.GAME,
    "ServiceData":
    {
        "Type": Game.NEXTROUND,
        "Payload":
        {
            "gameId": 0,
            "hash": playerHash
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.ACTIVATE,
        "Payload":
        {
            "id": thisPlayerId,
            "hash": playerHash
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.DEACTIVATE,
        "Payload":
        {
            "id": thisPlayerId,
            "hash": playerHash
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.MESSAGE,
        "Payload":
        {
            "room": room,
            "hash": playerHash,
            "message": chatMessage
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.NEWROOM,
        "Payload":
        {
            "hash": playerHash
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.INVITE,
        "Payload":
        {
            "room": room,
            "name": playerName,
            "hash": playerHash
        }
    },
    "Service": General.CHAT,
    "ServiceData":
    {
        "Type": Chat.INVITE,
        "Payload":
        {
            "room": room,
            "name": playerName,
            "hash": playerHash
        }
    }
};

