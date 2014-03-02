using System;
using System.Collections.Generic;

namespace ServerWBSCKTest.Models
{
    public partial class User
    {
        public User()
        {
            this.User_Hash = new List<User_Hash>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int rank { get; set; }
        public byte[] timestamp { get; set; }
        public virtual ICollection<User_Hash> User_Hash { get; set; }
    }
}
