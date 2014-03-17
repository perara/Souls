using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsServer.Chat;

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
        }

        public void addRoom(ChatRoom room)
        {
            memberRooms.Add(room.id, room);
        }





    }
}
