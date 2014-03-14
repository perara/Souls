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
        /// <summary>
        /// Room ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// If this is false the first client in the "clients" list is leader and may kick or invite other clients
        /// </summary>
        public bool isStatic { get; set; }

        /// <summary>
        /// List of clients in room. First in list is leader.
        /// </summary>
        public LinkedList<Player> clients { get; set; }


        /// <summary>
        /// Creates a dynamic chat room with the specified player as leader
        /// </summary>
        /// <param name="client"></param>
        public ChatRoom(Player client)
        {
            isStatic = false;
            clients = new LinkedList<Player>();
            clients.AddFirst(client);
            
        }


        /// <summary>
        /// Creates a dynamic chat room with multiple clients from a linked list. The first client in the list becomes leader
        /// </summary>
        /// <param name="clientList"></param>
        public ChatRoom(LinkedList<Player> clientList)
        {
            isStatic = false;
            clients = new LinkedList<Player>();
            clients.Concat(clients);
        }


        /// <summary>
        /// Creates a chat room designed for use during gameplay. This is a static room with only the 2 competing players
        /// </summary>
        /// <param name="clientList"></param>
        public ChatRoom(Pair<Player> clientList)
        {
            isStatic = true;
            clients = new LinkedList<Player>();
            clients.Concat(clients);
        }


        /// <summary>
        /// Adds the specified client to this room unless it is null or already a member
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool AddClient(Player client)
        {
            if (clients.Contains(client) || client == null) return false; // Prevents adding duplicates or nulls
            clients.AddLast(client); // Add client to room
            return (clients.Contains(client)) ? true : false; // Checks if the client was added successfully
        }


        /// <summary>
        /// Removes the specified client from this room
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool RemoveClient(Player client)
        {
            return (clients.Remove(client)) ? true : false;
        }


        /// <summary>
        /// Checks if the specified client is the leader of this room
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsLeader(Player client)
        {
            return (this.clients.First().Equals(client)) ? true : false;
        }


        /// <summary>
        /// Sends a message to all clients in this room
        /// </summary>
        /// <param name="response"></param>
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
