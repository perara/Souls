using System;
using System.Collections.Generic;

namespace ServerWBSCKTest.Models
{
    public partial class User_Hash
    {
        public int id { get; set; }
        public string hash { get; set; }
        public Nullable<int> fk_user_id { get; set; }
        public virtual User User { get; set; }
    }
}
