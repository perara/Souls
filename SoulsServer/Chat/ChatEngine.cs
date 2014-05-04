using Newtonsoft.Json;
using Souls.Server.Objects;
using Souls.Server.Engine;
using Souls.Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Souls.Server.Network;


namespace Souls.Server.Chat
{
    public class ChatEngine
    {
        /// <summary>
        /// A dictionary containing all chat rooms
        /// </summary>
        public static Dictionary<int, ChatRoom> chatRooms { get; set; }

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
        /// Adds a chat room with only two players. Intended for chat when in a game. This room is static by default. No one can be invited or kicked.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public bool Request_NewGameRoom(Pair<ChatPlayer> clients)
        {

            ChatRoom chatRoom = new ChatRoom(clients);
            chatRoom.id = roomCounter;
            chatRooms.Add(roomCounter, chatRoom);



            clients.First.addRoom(chatRoom);
            clients.Second.addRoom(chatRoom);

            JObject returnObj = new JObject(
                new JProperty("name", "Server"),
                new JProperty("message", "Made chatroom \"" + roomCounter + "\""),
                new JProperty("chRoomId", roomCounter)
                );


            clients.First.chatContext.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, returnObj));
            clients.Second.chatContext.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, returnObj));
            Logging.Write(Logging.Type.CHAT, "Game chat made for " + clients.First.name + " and " + clients.Second.name);

            return (chatRooms[roomCounter++].Equals(chatRoom)) ? true : false;
        }


        /// <summary>
        /// Adds a chat room with the input client as the leader. Room is dynamic by default. Leader may invite and kick.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool Request_NewGameRoom(ChatPlayer client)
        {
            ChatRoom chatRoom = new ChatRoom(client);
            chatRooms.Add(roomCounter, chatRoom);

            client.addRoom(chatRoom);

            client.chatContext.SendTo(new Response(ChatService.ResponseType.CHAT_ROOM_MADE, "You made a new chat room with id " + roomCounter));
            Logging.Write(Logging.Type.CHAT, client.name + " created room with id: " + roomCounter);

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
        public bool Invite(ChatPlayer inviter)
        {
            int room = int.Parse(inviter.chatContext.payload["room"].ToString());
            string name = inviter.chatContext.payload["name"].ToString();

            // Checks if client is leader
            if (!chatRooms.ContainsKey(room) || !chatRooms[room].IsLeader(inviter) || chatRooms[room].isStatic)
            {
                inviter.chatContext.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not the leader of room " + room));
                Logging.Write(Logging.Type.CHAT, inviter.name + " tried to invite without being leader");
                return false;
            }

            ChatPlayer toInvite = Clients.GetInstance().gameList.Where(x => x.Value.name == name).FirstOrDefault().Value.chPlayer;

            if (toInvite != null && toInvite.chatContext == null)
            {
                inviter.chatContext.SendTo(new Response(ChatService.ResponseType.INVITED_CLIENT, toInvite.name + " has deactivated the chat"));
                return false;
            }
            else if (chatRooms[room].AddClient(toInvite))
            {
                inviter.chatContext.SendTo(new Response(ChatService.ResponseType.INVITED_CLIENT, "You invited " + toInvite.name + " to room " + room));
                toInvite.chatContext.SendTo(new Response(ChatService.ResponseType.GOT_INVITED, "You got invited to room " + room + " by " + inviter.name));
                Logging.Write(Logging.Type.CHAT, inviter.name + " invited " + name + " to room: " + room);
                return true;
            }
            else
            {
                inviter.chatContext.SendTo(new Response(ChatService.ResponseType.CLIENT_NOT_FOUND, "Problem adding " + name + " to room " + room + ". Is the player online?"));
                Logging.Write(Logging.Type.CHAT, "Problem adding " + name + " to room " + room + ". Already in room?");
                return false;
            }
        }


        /// <summary>
        /// Kicks a player by name from the given room, from JSON elements: name and room. Kicker must be leader of the room
        /// </summary>
        /// <param name="kicker"></param>
        /// <returns></returns>
        public bool Kick(ChatPlayer kicker)
        {
            int room = int.Parse(kicker.chatContext.payload["room"].ToString());
            string name = kicker.chatContext.payload["name"].ToString();

            // Checks if client is leader
            if (!chatRooms[room].IsLeader(kicker) || chatRooms[room].isStatic)
            {
                kicker.chatContext.SendTo(new Response(ChatService.ResponseType.NOT_LEADER, "You are not leader of room " + room));
                Logging.Write(Logging.Type.CHAT, kicker.name + " tried to kick without being leader");
                return false;
            }

            ChatPlayer toKick = Clients.GetInstance().gameList.FirstOrDefault(x => x.Value.name == name).Value.chPlayer;

            if (chatRooms[room].RemoveClient(toKick))
            {
                kicker.chatContext.SendTo(new Response(ChatService.ResponseType.KICKED_CLIENT, "You kicked " + toKick.name + " from room " + room));
                toKick.chatContext.SendTo(new Response(ChatService.ResponseType.GOT_KICKED, "You got kicked from room " + room + " by " + kicker.name));
                Logging.Write(Logging.Type.CHAT, kicker.name + " kicked " + name + " from room: " + room);
            }
            else Console.WriteLine("[CHAT] Problem kicking " + name + " from room " + room + ". Client not in room?");
            return true;
        }


        /// <summary>
        /// Makes the specified client leave the given room, from JSON: room. If client was the last one in that room, the room gets deleted
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool LeaveRoom(ChatPlayer client)
        {
            int room = int.Parse(client.chatContext.payload["room"].ToString());
            bool wasLeader = chatRooms[room].IsLeader(client);

            chatRooms[room].RemoveClient(client);

            if (chatRooms[room].clients.Count() == 0)
            {
                chatRooms.Remove(room);
                return true;
            }
            else if (chatRooms[room].clients.Contains(client))
            {
                client.chatContext.SendTo(new Response(ChatService.ResponseType.CHAT_ERROR, "Error leaving room " + room));
                Logging.Write(Logging.Type.CHAT, "Error removing " + client.name + " from room " + room);
                return false;
            }
            else
            {
                chatRooms[room].Broadcast(new Response(ChatService.ResponseType.CHAT_MESSAGE, client.name + " left the room"));
                if (wasLeader)
                    chatRooms[room].clients.First().chatContext.SendTo(new Response(ChatService.ResponseType.MADE_LEADER, "You are now the leader of room " + room));
                client.chatContext.SendTo(new Response(ChatService.ResponseType.LEFT_ROOM, "You left room " + room));
                Logging.Write(Logging.Type.CHAT, client.name + " left room " + room);
                return true;
            }
        }

        public void GetRoomAttendees(ChatPlayer client)
        {
            int roomId = int.Parse(client.chatContext.payload["room"].ToString());


            ChatRoom room = chatRooms[roomId];

            List<string> clientNames = new List<string>();
            foreach(ChatPlayer ch in room.clients)
            {
                clientNames.Add(ch.name);
            }

            client.chatContext.SendTo(
                new Response(ChatService.ResponseType.CHAT_LIST_MEMBERS, clientNames));

        }


        /// <summary>
        /// Broadcasts a message to all members in the specified room unless they have deactivated chat (checked in Broadcast function)
        /// </summary>
        /// <param name="client"></param>
        public void SendMessage(ChatPlayer client)
        {
            int room = int.Parse(client.chatContext.payload["room"].ToString());
            string message = client.chatContext.payload["message"].ToString();
            string name = client.name;


            ChatRoom chRoom;
            bool success = client.memberRooms.TryGetValue(room, out chRoom);

            if (success)
            {

                JObject retObj = new JObject(
                    new JProperty("message", message),
                    new JProperty("room", room),
                    new JProperty("name", name)
                    );

                chRoom.Broadcast(new Response(ChatService.ResponseType.CHAT_MESSAGE, retObj));
            }
            else
            {
                client.chatContext.SendTo(new Response(ChatService.ResponseType.CHAT_NOT_MEMBER, "You are not a member of that room!"));
            }

        }


        /// <summary>
        /// Activates the chat for the specified client
        /// </summary>
        /// <param name="client"></param>
        /*  public void EnableChat(Player client)
          {
              client.chatActive = true;
              client.gameContext.SendTo(new Response(ChatService.ResponseType.CHAT_ENABLED, "Activated chat for " + client.name));
              Console.WriteLine("[CHAT] User \"" + client.name + "\" logged in to chat");
          }*/


        /// <summary>
        /// Deactivates the chat for the specified client
        /// </summary>
        /// <param name="client"></param>
        /* public void DisableChat(Player client)
         {
             client.chatActive = false;
             client.gameContext.SendTo(new Response(ChatService.ResponseType.CHAT_DISABLED, "Deactivated chat for " + client.name));
             Console.WriteLine("[CHAT] User \"" + client.name + "\" exited chat");
         }*/
    }
}
