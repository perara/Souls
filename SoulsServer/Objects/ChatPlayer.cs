using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Chat;
using Souls.Server.Engine;
using Souls.Server.Network;

namespace Souls.Server.Objects
{
    public class ChatPlayer
    {
        public Dictionary<int, ChatRoom> memberRooms { get; set; }
        public string name { get; set; }
        public General chatContext { get; set; }

        public ChatPlayer(string name)
        {
            this.name = name;
            memberRooms = new Dictionary<int, ChatRoom>();
        }

        public void addRoom(ChatRoom room)
        {
            room.id = memberRooms.Count();
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
