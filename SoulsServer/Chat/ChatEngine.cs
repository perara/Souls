using Newtonsoft.Json;
using SoulsServer.Engine;
using SoulsServer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsServer.Chat
{
    public class ChatEngine
    {
        /// <summary>
        /// A dictionary containing all chat rooms
        /// </summary>
        public Dictionary<int, ChatRoom> chatRooms { get; set; }

        /// <summary>
        /// Counts up 1 for each room created
        /// </summary>
        public int roomCounter { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public ChatEngine()
        {
            chatRooms = new Dictionary<int, ChatRoom>();
            roomCounter = 0;
        }


        /// <summary>
        /// Adds a chat room with a list of players, the first player in the list is leader. Room is dynamic by default. Leader may invite and kick.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public bool AddChatRoom(LinkedList<Player> clients)
        {
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);

            foreach (Player client in clients)
                client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Someone created a new chat with you with room id " + roomCounter));

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        /// <summary>
        /// Adds a chat room with only two players. Intended for chat when in a game. This room is static by default. No one can be invited or kicked.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public bool AddChatRoom(Pair<Player> clients)
        {
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);

            clients.First.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Automatically made chatroom with id " + roomCounter));
            clients.Second.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Automatically made chatroom with id " + roomCounter));
            Console.WriteLine("[CHAT] Gamechat made for " + clients.First.name + " and " + clients.Second.name);

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        /// <summary>
        /// Adds a chat room with the input client as the leader. Room is dynamic by default. Leader may invite and kick.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool AddChatRoom(Player client)
        {
            ChatRoom chatRoom = new ChatRoom(client);
            chatRooms.Add(roomCounter, chatRoom);

            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "You made a new chat room with id " + roomCounter));
            Console.WriteLine("[CHAT] " + client.name + " created room with id: " + roomCounter);

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        /// <summary>
        /// NOT YET USED (future work, the invited client have to accept an invite)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="from"></param>
        /// <param name="msg"></param>
        public void SendInvite(int userId, Player from, string msg) { }


        /// <summary>
        /// Adds a player by name to the given room, from JSON elements: name and room. Inviter must be leader of the room
        /// </summary>
        /// <param name="inviter"></param>
        /// <returns></returns>
        public bool Invite(Player inviter)
        {
            int room = inviter.context.payload.room;
            string name = inviter.context.payload.name;

            // Checks if client is leader
            if (!chatRooms.ContainsKey(room) || !chatRooms[room].IsLeader(inviter) || chatRooms[room].isStatic)
            {
                inviter.context.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not the leader of room " + room));
                Console.WriteLine("[CHAT] " + inviter.name + " tried to invite without being leader");
                return false;
            }

            Player toInvite = OnlinePlayers.GetInstance().list.Where(x => x.Value.name == name).FirstOrDefault().Value;

            if (toInvite != null && !toInvite.chatActive)
            {
                inviter.context.SendTo(new Response(ChatService.ResponseType.INVITED_CLIENT, toInvite.name + " has deactivated the chat"));
                return false;
            }
            else if (chatRooms[room].AddClient(toInvite))
            {
                inviter.context.SendTo(new Response(ChatService.ResponseType.INVITED_CLIENT, "You invited " + toInvite.name + " to room " + room));
                toInvite.context.SendTo(new Response(ChatService.ResponseType.GOT_INVITED, "You got invited to room " + room + " by " + inviter.name));
                Console.WriteLine("[CHAT] " + inviter.name + " invited " + name + " to room: " + room);
                return true;
            }
            else
            {
                inviter.context.SendTo(new Response(ChatService.ResponseType.CLIENT_NOT_FOUND, "Problem adding " + name + " to room " + room + ". Is the player online?"));
                Console.WriteLine("[CHAT] Problem adding " + name + " to room " + room + ". Already in room?");
                return false;
            }
        }


        /// <summary>
        /// Kicks a player by name from the given room, from JSON elements: name and room. Kicker must be leader of the room
        /// </summary>
        /// <param name="kicker"></param>
        /// <returns></returns>
        public bool Kick(Player kicker)
        {
            int room = kicker.context.payload.room;
            string name = kicker.context.payload.name;

            // Checks if client is leader
            if (!chatRooms[room].IsLeader(kicker) || chatRooms[room].isStatic)
            {
                kicker.context.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not leader of room " + room));
                Console.WriteLine("[CHAT] " + kicker.name + " tried to kick without being leader");
                return false;
            }

            Player toKick = OnlinePlayers.GetInstance().list.FirstOrDefault(x => x.Value.name == name).Value;

            if (chatRooms[room].RemoveClient(toKick))
            {
                kicker.context.SendTo(new Response(ChatService.ResponseType.KICKED_CLIENT, "You kicked " + toKick.name + " from room " + room));
                toKick.context.SendTo(new Response(ChatService.ResponseType.GOT_KICKED, "You got kicked from room " + room + " by " + kicker.name));
                Console.WriteLine("[CHAT] " + kicker.name + " kicked " + name + " from room: " + room);
            }
            else Console.WriteLine("[CHAT] Problem kicking " + name + " from room " + room + ". Client not in room?");
            return true;
        }


        /// <summary>
        /// Makes the specified client leave the given room, from JSON: room. If client was the last one in that room, the room gets deleted
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool LeaveRoom(Player client)
        {
            int room = client.context.payload.room;
            bool wasLeader = chatRooms[room].IsLeader(client);

            chatRooms[room].RemoveClient(client);

            if (chatRooms[room].clients.Count() == 0)
            {
                chatRooms.Remove(room);
                return true;
            }
            else if (chatRooms[room].clients.Contains(client))
            {
                client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ERROR, "Error leaving room " + room));
                Console.WriteLine("[CHAT] Error removing " + client.name + " from room " + room);
                return false;
            }
            else
            {
                chatRooms[room].Broadcast(new Response(ChatService.ResponseType.CHAT_MESSAGE, client.name + " left the room"));
                if (wasLeader)
                    chatRooms[room].clients.First().context.SendTo(new Response(ChatService.ResponseType.MADE_LEADER, "You are now the leader of room " + room));
                client.context.SendTo(new Response(ChatService.ResponseType.LEFT_ROOM, "You left room " + room));
                Console.WriteLine("[CHAT] " + client.name + " left room " + room);
                return true;
            }
        }


        /// <summary>
        /// Broadcasts a message to all members in the specified room unless they have deactivated chat (checked in Broadcast function)
        /// </summary>
        /// <param name="client"></param>
        public void SendMessage(Player client)
        {
            int room = client.context.payload.room;
            string message = client.name + ": " + client.context.payload.message;

            // TODO Solve the id,message payload issue in a better way, throws exceptions in console  o.O
            Dictionary<string, dynamic> elements = new Dictionary<string, dynamic>();
            elements.Add("id", room);
            elements.Add("message", message);
            dynamic payload = elements;

            if (!chatRooms.ContainsKey(room))
            {
                client.context.SendError("Room does not exist");
                return;
            }
            else if (payload == null)
            {
                client.context.SendError("You didn't provide a message");
                return;
            }
            else if (!client.chatActive)
            {
                client.context.SendError("You have disabled chat");
            }
            else if (chatRooms[room].clients.Contains(client))
            {
                chatRooms[room].Broadcast(new Response(ChatService.ResponseType.CHAT_MESSAGE, payload));
                Console.WriteLine("[CHAT] " + client.name + ": " + payload);
            }
            else client.context.SendError("You are not a member of room " + room);
        }


        /// <summary>
        /// Activates the chat for the specified client
        /// </summary>
        /// <param name="client"></param>
        public void EnableChat(Player client)
        {
            client.chatActive = true;
            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ENABLED, "Activated chat for " + client.name));
            Console.WriteLine("[CHAT] User \"" + client.name + "\" logged in to chat");
        }


        /// <summary>
        /// Deactivates the chat for the specified client
        /// </summary>
        /// <param name="client"></param>
        public void DisableChat(Player client)
        {
            client.chatActive = false;
            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_DISABLED, "Deactivated chat for " + client.name));
            Console.WriteLine("[CHAT] User \"" + client.name + "\" exited chat");
        }


        public void ChatLogin(General client)
        {
            string hash = client.payload.hash;

            Console.WriteLine("Should be +1: " + OnlinePlayers.GetInstance().list.Count());

            KeyValuePair<General,Player> cli = OnlinePlayers.GetInstance().list.Where(x => x.Value.hash == hash).FirstOrDefault();
            if (cli.Key != null)
            {
                Console.WriteLine("> [CHAT]: Found game connection. Adding link Chat.friendCon<-->Game.friendCon client!");
                cli.Key.friendCon = client;
                client.friendCon = cli.Key;

            }





        }

        public void ChatLogout(General client)
        {

        }
    }
}
