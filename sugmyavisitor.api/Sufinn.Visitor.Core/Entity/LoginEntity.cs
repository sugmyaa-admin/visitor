using System;
using System.Collections.Generic;
using System.Text;

namespace Sufinn.Visitor.Core.Entity
{
    public class LoginEntity
    {
        public int loginId { get; set; }
        public string password { get; set; }
        public string oprFlag { get; set; }
        public string oldPassword { get; set; }
    }

    public class LoginData
    {
        public int loginId { get; set; }
        public string name { get; set; }
        public string email_id { get; set; }
    }
}
