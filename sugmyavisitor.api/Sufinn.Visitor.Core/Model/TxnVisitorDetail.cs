using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Sufinn.Visitor.Core.Model
{
    public partial class TxnVisitorDetail
    {
        [Key]
        public int TxnId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long Number { get; set; }
        public int Purpose { get; set; }
        public string WhomToMeet { get; set; }
        public string Company { get; set; }
        public int? NumberOfPerson { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int? ForceCheckOut { get; set; }
        public string Mode { get; set; }
        public string Picture { get; set; }
        public int? LastLoginOtp { get; set; }

    }

}
