using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ServerWBSCKTest.Engine
{
    class ClientData
    {
        //public string Service { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Payload { get; set; }


        public ClientData(dynamic data)
        {
            if (data.ServiceData.Type == null) throw new NullReferenceException("ServiceDataType is NULL");
            this.Type = data.ServiceData.Type;
            string inndata = JsonConvert.SerializeObject(data.ServiceData.Payload);

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, string>>(inndata);

            Payload = dict;
        }

    }
}
