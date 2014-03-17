using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Chat;
using SoulsServer.Engine;

namespace SoulsServer.Controller
{
    public class ChatPlayer
    {
        public Dictionary<int, ChatRoom> memberRooms { get; set; }
        public string name { get; set; }
        public Engine.General chatContext { get; set; }

        public ChatPlayer(string name)
        {
            this.name = name;
            memberRooms = new Dictionary<int, ChatRoom>();
        }

        public void addRoom(ChatRoom room)
        {
            memberRooms.Add(room.id, room);
        }


        /// <summary>
        /// Announces to all of the players member channels that he disconnected
        /// </summary>
        public void AnnounceDisconnect()
        {
            foreach (var room in this.memberRooms)
            {
                room.Value.Broadcast(new Response(ChatService.ResponseType.CHAT_CLIENT_DISCONNECT, "Player \"" + this.name + "\" disconnected!"));
            }
        }

        public void AnnounceConnect()
        {
            foreach (var room in this.memberRooms)
            {
                room.Value.Broadcast(new Response(ChatService.ResponseType.CHAT_CLIENT_CONNECT, "Player \"" + this.name + "\" connected!"));
            }
        }






    }
}
