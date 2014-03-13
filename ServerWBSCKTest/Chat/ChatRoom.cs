using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;

namespace ServerWBSCKTest.Chat
{
    public class ChatRoom
    {
        // Room ID
        public int id { get; set; }

        // If false, first client in clients list is leader and may kick other clients
        public bool isStatic { get; set; }

        //List of clients in room. First in list is leader.
        public LinkedList<Player> clients { get; set; }

        public ChatRoom(Player client)
        {
            isStatic = false;
            clients = new LinkedList<Player>();
            clients.AddFirst(client);
            
        }
        public ChatRoom(LinkedList<Player> clientList)
        {
            isStatic = false;
            clients = new LinkedList<Player>();
            clients.Concat(clients);
        }

        public ChatRoom(Pair<Player> clientList)
        {
            isStatic = true;
            clients = new LinkedList<Player>();
            clients.Concat(clients);
        }

        public bool AddClient(Player client)
        {
            if (clients.Contains(client) || client == null) return false; // Prevents adding duplicates or nulls
            clients.AddLast(client); // Add client to room
            return (clients.Contains(client)) ? true : false; // Checks if the client was added successfully
        }

        public bool RemoveClient(Player client)
        {
            return (clients.Remove(client)) ? true : false;
        }

        public bool isLeader(Player client)
        {
            return (this.clients.First().Equals(client)) ? true : false;
        }

        public void Broadcast(Response response)
        {
            
            foreach (Player client in clients)
            {
                if (!client.chatActive) continue;
                client.context.SendTo(response);
            }
        }
    }
}
