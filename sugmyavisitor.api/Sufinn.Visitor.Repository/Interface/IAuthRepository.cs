using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<Result<string>> authenticate(int employeeCode, string password);
        Task<Result<string>> changeInPassword(int employeeCode, string password, char oprFlag, string oldPassword);
        UserEntity findById(int employeeCode);
        void logout(int employeeCode);
    }
}
