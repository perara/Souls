using Alchemy.Classes;
using ServerWBSCKTest.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Chat
{
    public class ChatEngine
    {
        public Action<LinkedList<UserContext>, SocketServer.Response> cbkSendChat;

        public Dictionary<int, ChatRoom> chatRooms { get; set; } // Hopeully will the dictionary get smaller when chatrooms are removed :>

        public int roomCounter { get; set; }

        public ChatEngine()
        {
            chatRooms = new Dictionary<int, ChatRoom>();
            roomCounter = 0;
        }

        public bool addChatRoom(LinkedList<UserContext> clients)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);
            roomCounter++;

            return (chatRooms[roomCounter].Equals(chatRoom)) ? true : false;
        }
        public bool addChatRoom(UserContext client)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(client);
            chatRooms.Add(roomCounter, chatRoom);

            Console.WriteLine(SocketServer.OnlinePlayers[client].name + " created room " + roomCounter);
            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }

        public bool removeChatRoom(int roomId)
        {
            chatRooms.Remove(roomId);
            return (chatRooms.ContainsKey(roomId)) ? true : false;
        }

        public void sendMessage(int chatRoom, UserContext fromClient, string msg)
        {
            string message = SocketServer.OnlinePlayers[fromClient].name + ": " + msg;
            SocketServer.Response response = new SocketServer.Response(ChatDataHandler.ResponseType.MESSAGE, message);

            cbkSendChat(chatRooms[chatRoom].clients, response);
        }
        public void sendInvite(int userId, UserContext fromClient, string msg)
        {
            string message = SocketServer.OnlinePlayers[fromClient].name + ": " + msg;
            SocketServer.Response response = new SocketServer.Response(ChatDataHandler.ResponseType.MESSAGE, message);
        }

        public bool isLeader(UserContext client, int room)
        {
            return (chatRooms[room].clients.First().Equals(client)) ? true : false;
        }

        public void addCallbacks(Action<LinkedList<UserContext>, SocketServer.Response> cbkSendChat)
        {
            this.cbkSendChat = cbkSendChat;
        }
    }
}
