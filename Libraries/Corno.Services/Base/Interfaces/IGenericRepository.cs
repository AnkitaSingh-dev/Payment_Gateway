using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Base.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        #region -- Methods --

        void RefreshAllEntities();
        //void RefreshEntites();

        TEntity Create();
        IQueryable<TEntity> GetAll();

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetById(object id);
        int Count(Func<TEntity, bool> predicate);
        IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<TEntity> entities);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entity);
        void Update(TEntity entityToUpdate, string id);
        void UpdateRange(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities, string id);

        #endregion;
    }
}