using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace Souls.Client.Classes
{
    public class Toolkit
    {
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }


        public static bool PingHost(string _HostURI, int _PortNumber)
        {

            var client = new TcpClient();
            var result = client.BeginConnect(_HostURI, _PortNumber, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (!success)
            {
                return false;
            }
            // we have connected
            client.EndConnect(result);


            return true;



        }


        public static long getTimestamp()
        {
            return Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        }
    }
}