using Souls.Server.Game;
using Souls.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using Souls.Server.Tools;
using Souls.Server.Objects;
using Souls.Server.Chat;

namespace SoulsServer.Network
{
    public class ChatService : Service
    {

        /// <summary>
        /// Responses is 1000 ++
        /// </summary>
        public new enum ResponseType
        {
            // General
            CHAT_ENABLED = 1000,
            CHAT_DISABLED = 1001,
            CHAT_MESSAGE = 1003,
            CHAT_ROOM_MADE = 1004,
            INVITED_CLIENT = 1005,
            KICKED_CLIENT = 1006,
            LEFT_ROOM = 1007,
            GOT_INVITED = 1008,
            GOT_KICKED = 1009,
            MADE_LEADER = 1010,

            CHAT_CLIENT_CONNECT = 1094,
            CHAT_CLIENT_DISCONNECT = 1095,
            CHAT_NOT_MEMBER = 1096,
            NOT_LEADER = 1097,
            CLIENT_NOT_FOUND = 1098,
            CHAT_ERROR = 1099,

        }

        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// Chat IDS = 1000++++
        /// </summary>
        public enum ChatType
        {
            ENABLE = 1000,
            DISABLE = 1001,
            MESSAGE = 1002,
            NEWROOM = 1003,
            INVITE = 1004,
            KICK = 1005,
            LEAVE = 1006,
            CHAT_LOGIN = 1007,
            CHAT_LOGOUT = 1008,
            NEWGAMEROOM = 1009,
        }

        public ChatEngine engine;

        public ChatService(ChatEngine engine)
        {
            this.engine = engine;
            this.logType = Logging.Type.CHAT;
        }

        public override void Process()
        {
            if (this.loggedIn)
            {
                switch ((ChatType)type)
                {
                    // CHAT REQUESTS
                    case ChatType.ENABLE:
                        //engine.EnableChat(OnlinePlayers.GetInstance().gameList[this]);
                        break;

                    case ChatType.DISABLE:
                        //engine.DisableChat(OnlinePlayers.GetInstance().gameList[this]);
                        break;

                    case ChatType.MESSAGE:
                        engine.SendMessage(Clients.GetInstance().chatList[this].chPlayer);
                        break;

                    case ChatType.NEWROOM:
                        engine.Request_NewGameRoom(Clients.GetInstance().chatList[this].chPlayer);
                        break;
                    case ChatType.NEWGAMEROOM:

                        //////////////////////////////////////////////////////////////////////////
                        //TODO CLEANUP Should check that this.player != null


                        Player requestPlayer = Clients.GetInstance().chatList[this];
                        // Go via the game player object to get opponent context.
                        Player opponentPlayer = requestPlayer.GetOpponent();

                        engine.Request_NewGameRoom(new Pair<ChatPlayer>(requestPlayer.chPlayer, opponentPlayer.chPlayer));



                        break;
                    case ChatType.INVITE:
                        engine.Invite(Clients.GetInstance().gameList[this].chPlayer);
                        break;

                    case ChatType.KICK:
                        engine.Kick(Clients.GetInstance().gameList[this].chPlayer);
                        break;

                    case ChatType.LEAVE:
                        engine.LeaveRoom(Clients.GetInstance().gameList[this].chPlayer);
                        break;
                }
            }
            switch ((SERVICE)this.type)
            {
                case SERVICE.LOGIN:
                    Login();
                    break;
                case SERVICE.HEARTBEAT:
                    HeartBeat();
                    break;
            }
        }

        public void Login()
        {
            string hash = this.payload["hash"].ToString();

            //Logging.Write(Logging.Type.CHAT, "" + OnlinePlayers.GetInstance().gameList.Count());

            KeyValuePair<Client, Player> chClient = Clients.GetInstance().chatList.Where(x => x.Value.hash == hash).FirstOrDefault(); //Chat Record
            // Remove chat client if it already exists //TODO?
            if (chClient.Key != null)
            {
                Player p;
                Clients.GetInstance().chatList.TryRemove(chClient.Key, out p);
            }

            KeyValuePair<Client, Player> gClient = Clients.GetInstance().gameList.Where(x => x.Value.hash == hash).FirstOrDefault(); //Game Record
            if (gClient.Key != null)
            {
                Logging.Write(Logging.Type.CHAT, "Found open game connection Linking.....");

                Player existingPlayer = gClient.Value;

                // Update the playerObject contexts
                existingPlayer.chatContext = this;


                // Insert the chatConnection record
                Clients.GetInstance().chatList.TryAdd(this, existingPlayer);

                // Create a new chatPlayer
                if (existingPlayer.chPlayer == null)
                {
                    ChatPlayer chPlayer = new ChatPlayer(existingPlayer.name);
                    existingPlayer.chPlayer = chPlayer;
                    chPlayer.chatContext = existingPlayer.chatContext;
                }
                else
                {
                    // Update the context
                    existingPlayer.chPlayer.chatContext = existingPlayer.chatContext;
                    existingPlayer.chPlayer.AnnounceConnect();
                }


            }

            else // No existing game Player was found, create new player
            {
                // General function for creating player
            }

        }

    }
}
