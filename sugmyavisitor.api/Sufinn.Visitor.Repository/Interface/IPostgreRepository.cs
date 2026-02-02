using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository.Interface
{
    public interface IPostgreRepository
    {
        Task<Result<string>> Save(string storedProcedure, Dictionary<string, object> parameters);
    }
}
