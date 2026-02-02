using System;
using System.Collections.Generic;

#nullable disable

namespace Sufinn.Visitor.Core.Model
{
    public partial class MstLoginVisitorDetail
    {
        public int LoginId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string MobileNum { get; set; }
        public DateTime? PasswordResetDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string AccountLock { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? RetryCount { get; set; }
    }
}
