using Newtonsoft.Json;
using ServerWBSCKTest.Engine;
using ServerWBSCKTest.Tools;
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


        // Adds a chat room with a list of players, the first player in the list is leader. Room is dynamic by default. Leader may invite and kick.
        public bool addChatRoom(LinkedList<Player> clients)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);

            foreach (Player client in clients) 
                client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Someone created a new chat with you with room id " + roomCounter));
            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        // Adds a chat room with only two players. Intended for chat when in a game. This room is static by default. No one can be invited or kicked.
        public bool addChatRoom(Pair<Player> clients)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(clients);
            chatRooms.Add(roomCounter, chatRoom);

            clients.First.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Automatically made chatroom with id " + roomCounter));
            clients.Second.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "Automatically made chatroom with id " + roomCounter));
            Console.WriteLine("[CHAT] Gamechat made for " + clients.First.name + " and " + clients.Second.name);

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        // Adds a chat room with the input client as the leader. Room is dynamic by default. Leader may invite and kick.
        public bool addChatRoom(Player client)
        {
            // TODO Add callback or something
            ChatRoom chatRoom = new ChatRoom(client);
            chatRooms.Add(roomCounter, chatRoom);

            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "You made a new chat room with id " + roomCounter));
            Console.WriteLine("[CHAT] " + client.name + " created room with id: " + roomCounter);
            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        // NOT USED (future work, the invited have to accept invite)
        public void sendInvite(int userId, Player from, string msg) { }


        // Adds the player to the room automatically
        public bool Invite(Player inviter)
        {
            int room = inviter.context.payload.room;
            string name = inviter.context.payload.name;

            // Checks if client is leader
            if (!chatRooms.ContainsKey(room) || !chatRooms[room].isLeader(inviter) || chatRooms[room].isStatic)
            {
                inviter.context.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not the leader of room " + room));
                Console.WriteLine("[CHAT] " + inviter.name + " tried to invite without being leader");
                return false;
            }

            Player toInvite = General.OnlinePlayers.Where(x => x.Value.name == name).FirstOrDefault().Value;

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


        // Kicks the player from the specified room (if leader of that room)
        public bool Kick(Player kicker)
        {
            int room = kicker.context.payload.room;
            string name = kicker.context.payload.name;

            // Checks if client is leader
            if (!chatRooms[room].isLeader(kicker) || chatRooms[room].isStatic)
            {
                kicker.context.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not leader of room " + room));
                Console.WriteLine("[CHAT] " + kicker.name + " tried to kick without being leader");
                return false;
            }

            Player toKick = General.OnlinePlayers.FirstOrDefault(x => x.Value.name == name).Value;
            bool success = chatRooms[room].RemoveClient(toKick);

            if (success)
            {
                kicker.context.SendTo(new Response(ChatService.ResponseType.KICKED_CLIENT, "You kicked " + toKick.name + " from room " + room));
                toKick.context.SendTo(new Response(ChatService.ResponseType.GOT_KICKED, "You got kicked from room " + room + " by " + kicker.name));
                Console.WriteLine("[CHAT] " + kicker.name + " kicked " + name + " from room: " + room);
            }
            else Console.WriteLine("[CHAT] Problem kicking " + name + " from room " + room + ". Client not in room?");

            return true;
        }


        // Leaved the specified room, if you were the last one in that room, the room gets deleted
        public bool LeaveRoom(Player client)
        {
            int room = client.context.payload.room;
            bool wasLeader = chatRooms[room].isLeader(client);
            chatRooms[room].RemoveClient(client);

            if (chatRooms[room].clients.Count() == 0)
            {
                chatRooms.Remove(room);
                return true;
            }
            else if (chatRooms[room].clients.Contains(client))
            {
                client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ERROR,"Error leaving room " + room));
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


        //Broadcasts a message in the specified room
        public void SendMessage(Player client)
        {
            int room = client.context.payload.room;
            string message = client.name + ": " + client.context.payload.message;
            
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
                client.context.SendError("You have disabled chat");
            else if (chatRooms[room].clients.Contains(client))
            {
                chatRooms[room].Broadcast(new Response(ChatService.ResponseType.CHAT_MESSAGE, payload));
                Console.WriteLine("[CHAT] " + client.name + ": " + payload);
            }
            else client.context.SendError("You are not a member of this room (id:" + room + ")");
        }

        // Activates the chat for the specified client
        public void EnableChat(Player client)
        {
            client.chatActive = true;
            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_ENABLED, "Activated chat for " + client.name));
            Console.WriteLine("User \"" + client.name + "\" logged in to chat");
        }

        // Deactivates the chat for the specified client
        public void DisableChat(Player client)
        {
            client.chatActive = false;
            client.context.SendTo(new Response(ChatService.ResponseType.CHAT_DISABLED, "Deactivated chat for " + client.name));
            Console.WriteLine(client.name + " exited chat");
        }
    }
}
