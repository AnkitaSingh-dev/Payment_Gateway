using Corno.Data.Context;
using Corno.Services.Base.Interfaces;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Corno.Services.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        protected BaseContext BaseContext;

        private bool _disposed;
        protected string ConnectionString;

        public UnitOfWork()
        {
        }

        public UnitOfWork(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public UnitOfWork(BaseContext dbContext)
        {
            BaseContext = dbContext;
        }

        public virtual BaseContext DbContext
        {
            get
            {
                if (BaseContext != null) return BaseContext;

                BaseContext = new BaseContext(ConnectionString);
                return BaseContext;
            }
        }

        public virtual void Save()
        {
            try
            {
                BaseContext.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (var item in exception.EntityValidationErrors)
                {
                    // Get entry
                    var entry = item.Entry;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            break;
                    }
                }

                throw;
            }
        }

        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    BaseContext?.Dispose();
                }
            }
            _disposed = true;
        }
    }
}