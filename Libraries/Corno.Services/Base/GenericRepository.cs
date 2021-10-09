using Corno.Data.Context;
using Corno.Services.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Base
{
    public class GenericRepository<TEntity> :
        IGenericRepository<TEntity>
        where TEntity : class
    {
        #region -- Constructor --

        public GenericRepository(IUnitOfWork unitOfWork)
        {
            _dbContext = unitOfWork.DbContext;
            _dbSet = unitOfWork.DbContext.Set<TEntity>();
        }

        #endregion

        #region -- Data Members --

        private readonly BaseContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        #endregion

        #region -- Special Methods --
        private static void DetachAll(DbContext dbContext)
        {
            foreach (var dbEntityEntry in dbContext.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        private static DbContext RefreshEntites(DbContext dbContext, RefreshMode refreshMode, Type entityType)
        {
            DetachAll(dbContext);

            var objectContext = ((IObjectContextAdapter) dbContext).ObjectContext;
            var refreshableObjects = objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged)
                .Where(x => entityType == null || x.Entity.GetType() == entityType)
                .Where(entry => entry.EntityKey != null)
                .Select(e => e.Entity)
                .ToArray();

            objectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);

            return dbContext;
        }

        private static DbContext RefreshAllEntites(DbContext dbContext, RefreshMode refreshMode)
        {
            return RefreshEntites(dbContext: dbContext, refreshMode: refreshMode, entityType: null); //null entityType is a wild card
        }

        //private static DbContext RefreshEntites<TEntity>(DbContext dbContext, RefreshMode refreshMode)
        //{
        //    return RefreshEntites(dbContext: dbContext, refreshMode: refreshMode, entityType: typeof(TEntity));
        //}
        #endregion

        #region -- Methods --

        public void RefreshAllEntities()
        {
            RefreshAllEntites(_dbContext, RefreshMode.StoreWins);
        }

        //public void RefreshEntites()
        //{
        //    RefreshEntites<TEntity>(_dbContext, RefreshMode.StoreWins);
        //}

        public TEntity Create()
        {
            var entity = _dbSet.Create();
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {

            IQueryable<TEntity> query = _dbSet;
            return query;
        }

        /// <summary>
        ///     Get the entities by filters
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="includeProperties">Include Properties</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
                return orderBy(query).ToList();

            return query.ToList();
        }

        /// <summary>
        ///     Get the count of entities on the base of conditions as predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public int Count(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate).Count();
        }

        /// <summary>
        ///     Retrieve Entity by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        ///     Find Entity by the condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate);
        }

        /// <summary>
        ///     Insert single entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Add(TEntity entity)
        {
            Logger.LogHandler.LogInfo("_dbSet add", Logger.LogHandler.LogType.Notify);
            _dbSet.Add(entity);
            Logger.LogHandler.LogInfo("_dbSet added succesfully", Logger.LogHandler.LogType.Notify);
        }

        /// <summary>
        ///     Bulk Insert the list of entities in the DB Set.
        /// </summary>
        /// <param name="entities">Collection of entities</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Delete(entity);
        }

        /// <summary>
        ///     Delete entity identified by primary key
        /// </summary>
        /// <param name="id">ID as primary key</param>
        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        // Bulk Delete the list of entities in the DB Set.
        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);

            _dbSet.Remove(entityToDelete);
        }

        // Update single entity in the database.
        public virtual void Update(TEntity entityToUpdate)
        {
            Update(entityToUpdate, "Id");
        }

        public virtual void Update(TEntity entityToUpdate, string id)
        {
            //_dbSet.Attach(entityToUpdate);
            //_dbContext.Entry(entityToUpdate).State = EntityState.Modified;

            var entry = _dbContext.Entry(entityToUpdate);

            // Retreive the Id through reflection
            var pkey = _dbSet.Create().GetType().GetProperty(id).GetValue(entityToUpdate);

            if (entry.State != EntityState.Detached) return;

            var set = _dbContext.Set<TEntity>();
            var attachedEntity = set.Find(pkey); // access the key

            if (attachedEntity != null)
            {
                var attachedEntry = _dbContext.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entityToUpdate);
            }
            else
            {
                entry.State = EntityState.Modified; // attach the entity
            }
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, string id)
        {
            foreach (var entity in entities)
            {
                Update(entity, id);
            }
        }

        #endregion
    }
}