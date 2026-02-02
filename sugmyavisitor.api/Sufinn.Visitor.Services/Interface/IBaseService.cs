using Sufinn.Visitor.Core.Entity;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository.Interface;

namespace Sufinn.Visitor.Services.Interface
{
    public interface IBaseService
    {
        IBaseRepository<T> GetRespository<T>() where T : class;
        int Save();
    }


}
