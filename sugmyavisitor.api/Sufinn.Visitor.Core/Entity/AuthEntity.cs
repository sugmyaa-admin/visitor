using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufinn.Visitor.Core.Entity
{
    public class AuthEntity
    {
        public string mobileNumber { get; set; }
        public string otp { get; set; }

    }
    public class EmployeeEntity
    {
        [JsonPropertyName("employeeId")]
        public string employee_Id { get; set; }
        [JsonPropertyName("employeeName")]
        public string employee_Name { get; set; }
    }
   
}
