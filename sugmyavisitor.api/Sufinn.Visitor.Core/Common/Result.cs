using System;
using System.Collections.Generic;
using System.Text;

namespace Sufinn.Visitor.Core.Common
{
    public struct Result
    {
        public Result(bool success, string message) => (Success, Message) = (success, message);
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class Result<T>
    {
        public Result Status { get; set; }
        public T Data { get; set; }
    }
}
