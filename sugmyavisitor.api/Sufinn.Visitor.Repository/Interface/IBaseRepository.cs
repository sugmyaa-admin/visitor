using Sufinn.Visitor.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        void Save(T obj);
        void Update(T obj);
        void Delete(T obj);
        bool IsExist(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAsync(bool isProcedure, string spName, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Filter(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedure, Dictionary<string, object> parameters, bool isCustomEntity = false);
        int SaveChanges();
    }
}
