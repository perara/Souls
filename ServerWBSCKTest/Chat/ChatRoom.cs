using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy.Classes;
using ServerWBSCKTest.Engine;

namespace ServerWBSCKTest.Chat
{
    public class ChatRoom
    {
        // Room ID
        public int id { get; set; }

        // If false, first in clients list is leader and may kick other clients
        public bool isStatic { get; set; }

        //List of clients in room. First in list is leader.
        public LinkedList<UserContext> clients { get; set; }

        public ChatRoom(UserContext client)
        {
            clients = new LinkedList<UserContext>();
            clients.AddFirst(client);
        }
        public ChatRoom(LinkedList<UserContext> clientList)
        {
            clients = new LinkedList<UserContext>();
            clients.Concat(clients);
        }

        public bool AddClient(UserContext client)
        {
            if (clients.Contains(client)) return false; // Return if client already is in room
            clients.AddLast(client); // Add client to room
            return (clients.Contains(client)) ? true : false; // Checks if the client was added successfully
        }

        public bool RemoveClient(UserContext client)
        {
            return (clients.Remove(client)) ? true : false;
        }
    }
}
