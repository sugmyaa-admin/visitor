using System;
using System.Collections.Generic;
using System.Text;

namespace Sufinn.Visitor.Core.Common
{
    public static class Common<T>
    {
        public static Result<T> getResponse(bool flag, string message, T data)
        {

            return new Result<T>()
            {
                Status = new Result()
                {
                    Success = flag,
                    Message = message
                },
                Data = data
            };
        }
    }
}
