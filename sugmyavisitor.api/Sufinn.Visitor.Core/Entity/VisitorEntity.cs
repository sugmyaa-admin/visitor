using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufinn.Visitor.Core.Entity
{
    public class VisitorEntity
    {
        public string txnId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string number { get; set; }
        public string purpose { get; set; }
        [JsonPropertyName("whomToMeet")]
        public string employee_name { get; set; }
        public string company { get; set; }
        [JsonPropertyName("numberOfPerson")]
        public string number_of_person { get; set; }
        [JsonPropertyName("checkIn")]
        public string check_in { get; set; }
        [JsonPropertyName("checkOut")]
        public string check_out { get; set; }
        public string picture { get; set; }


    }
}
