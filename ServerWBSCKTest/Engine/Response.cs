using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWBSCKTest.Engine
{
    /// <summary>
    /// Defines the response object to send back to the client
    /// </summary>
    public class Response
    {
        public dynamic Type { get; set; }
        public dynamic Payload { get; set; }

        public Response(object type, dynamic data)
        {
            //if (!(type is int)) throw new NotSupportedException("Wrong responsetype");
            this.Type = type;
            this.Payload = data;
        }

        public Response() { }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
