using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Chat
{
    public class ChatDataHandler
    {
        ChatEngine chatEngine = new ChatEngine();
        public ChatDataHandler(ChatEngine engine)
        {
            chatEngine = engine;
        }
        public void trafficHandler(UserContext client, dynamic chat)
        {
            bool success;

            dynamic payload = chat.Payload;
            int room;
            string name;

            switch ((ChatType)chat.Type)
            {
                
                // CHAT REQUESTS
                case ChatType.ACTIVATE:
                    SocketServer.OnlinePlayers[client].chatActive = true;
                    Console.WriteLine("User \"" + SocketServer.OnlinePlayers[client].name + "\" logged in to chat");
                    break;

                case ChatType.DEACTIVATE:
                    SocketServer.OnlinePlayers[client].chatActive = false;
                    Console.WriteLine(SocketServer.OnlinePlayers[client].name + " exited chat");
                    break;

                case ChatType.MESSAGE:
                    room = payload.room;

                    LinkedList<UserContext> list = chatEngine.chatRooms[room].clients;
                    string message = payload.message;
                    SocketServer.Response response = new SocketServer.Response(ChatDataHandler.ResponseType.MESSAGE, message);
                    
                    chatEngine.cbkSendChat(list, response);

                    Console.WriteLine(SocketServer.OnlinePlayers[client].name + ": " + chat.message);
                    break;

                case ChatType.NEWROOM:
                    chatEngine.addChatRoom(client);
                    
                    break;

                case ChatType.INVITE:
                    room = payload.room;

                    // Checks if client is leader
                    if (!chatEngine.isLeader(client, room))
                    {
                        Console.WriteLine(SocketServer.OnlinePlayers[client].name + " tried to invite without being leader");
                        return;
                    }
                    
                    UserContext invite = SocketServer.OnlinePlayers.Where(x => x.Value.name == (string)payload.name).FirstOrDefault().Key;
                    success = chatEngine.chatRooms[room].AddClient(invite);

                    if (success) Console.WriteLine(SocketServer.OnlinePlayers[client].name + " invited " + payload.name + " to room: " + room);
                    else Console.WriteLine("Problem adding " + payload.name + " to room " + room + ". Already in room?");
                    break;

                case ChatType.KICK:
                    room = payload.room;
                    name = payload.name;

                    // Checks if client is leader
                    if (!chatEngine.isLeader(client, room))
                    {
                        Console.WriteLine(SocketServer.OnlinePlayers[client].name + " tried to kick without being leader");
                        return;
                    }

                    UserContext kick = SocketServer.OnlinePlayers.FirstOrDefault(x => x.Value.name == (string)payload.name).Key;
                    success = chatEngine.chatRooms[room].RemoveClient(kick);

                    if (success) Console.WriteLine(SocketServer.OnlinePlayers[client].name + " kicked " + payload.name + " from room: " + room);
                    else Console.WriteLine("Problem kicking " + payload.name + " from room " + room + ". Client not in room?");
                    break;

                case ChatType.LEAVE:
                    room = payload.room;

                    chatEngine.chatRooms[room].RemoveClient(client);
                    if (chatEngine.chatRooms[room].clients.Count() == 0) chatEngine.chatRooms.Remove(room);
                    Console.WriteLine(payload.name + " left room: " + room);
                    break;
            }
        
        }

        public enum ResponseType
        {
            // General
            MESSAGE = 3,
            ERROR = 25
        }

        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        public enum ChatType
        {
            ACTIVATE = 0,
            DEACTIVATE = 1,
            MESSAGE = 2,
            NEWROOM = 3,
            INVITE = 4,
            KICK = 5,
            LEAVE = 6
        }
    }
}
