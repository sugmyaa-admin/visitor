using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository;
using Sufinn.Visitor.Repository.Context;
using Sufinn.Visitor.Repository.Context.Interface;
using Sufinn.Visitor.Repository.Interface;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sufinn.Visitor.Services
{
    public class BaseService<TContext> : IBaseService where TContext : IDbContext, new()
    {
        private readonly IDbContext _ctx;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;
        private readonly AppDBContext _appDbContext;
        public BaseService(AppDBContext context)
        {
            _ctx = new TContext();
            _appDbContext = context;
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IBaseRepository<T> GetRespository<T>() where T : class
        {
            if (_repositories.Keys.Contains(typeof(T)))
            {
                return _repositories[typeof(T)] as IBaseRepository<T>;
            }

            var respository = new BaseRepository<T>(_ctx, _appDbContext);
            _repositories.Add(typeof(T), respository);

            return respository;
        }

        public int Save()
        {
            return _ctx.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;

            if (disposing)
            {
                _ctx.Dispose();

            }

            this._disposed = true;
        }


    }
}
