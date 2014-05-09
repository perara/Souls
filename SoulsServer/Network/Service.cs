using Souls.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Souls.Server.Tools;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using Souls.Server.Objects;

namespace Souls.Server.Network
{
    public class Service : Client
    {

        protected override void OnMessage(MessageEventArgs e)
        {
            // Process the JSON
            JObject payload = JObject.Parse(e.Data);
            this.payload = payload["Payload"];
            this.type = int.Parse(payload.GetValue("Type").ToString());

            // Run the Process Method
            this.Process();
        }

        virtual public void Process()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }

        virtual public void Login()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }

        virtual public void Logout()
        {
            Logging.Write(Logging.Type.ERROR, "This function should be overloaded!");
            throw new AccessViolationException();
        }


        protected override void OnOpen()
        {
            userEndpoint = Context.UserEndPoint.ToString();
            Logging.Write(this.logType, "Client: " + Context.UserEndPoint + " connected.");
            Console.WriteLine("Client: " + Context.UserEndPoint + " connected. (" + Clients.GetInstance().gameList.Count() + ")");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Logging.Write(Logging.Type.ERROR, "Client: " + e.Message.ToString());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Logging.Write(this.logType, "Client: " + this.userEndpoint + " disconnected. Reason: " + e.Reason);
            Console.WriteLine("Client: " + this.userEndpoint + " disconnected. Reason: " + e.Reason);
            CloseConnection();

            this.loggedIn = false;

        }

        virtual public void CloseConnection()
        {

        }

    }
}
