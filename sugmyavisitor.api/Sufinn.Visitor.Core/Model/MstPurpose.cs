using System;
using System.Collections.Generic;

#nullable disable

namespace Sufinn.Visitor.Core.Model
{
    public partial class MstPurpose
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
