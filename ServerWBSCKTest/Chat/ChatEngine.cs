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
        // Hopeully will the dictionary get smaller when chatrooms are removed :>
        public Dictionary<int, ChatRoom> chatRooms { get; set; }
        public int roomCounter { get; set; }

        public ChatEngine()
        {
            chatRooms = new Dictionary<int, ChatRoom>();
            roomCounter = 0;
        }

        public bool addChatRoom(LinkedList<Player> clients)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }
        public bool addChatRoom(Player client)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(client);
            chatRooms.Add(roomCounter, chatRoom);

            Console.WriteLine(client.name + " created room with id: " + roomCounter);
            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }

        public bool removeChatRoom(int roomId)
        {
            chatRooms.Remove(roomId);
            return (chatRooms.ContainsKey(roomId)) ? true : false;
        }

        public void sendInvite(int userId, Player from, string msg)
        {
            string message = from.name + ": " + msg;
            Response response = new Response(ChatService.ResponseType.MESSAGE, message);
        }
    }
}
