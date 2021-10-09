using Corno.Services.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Corno.Services.Base
{
    public class BaseService<TEntity> : IBaseService<TEntity>
        where TEntity : class
    {
        #region -- Constructors --

        public BaseService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> entityRepository)
        {
            UnitOfWork = unitOfWork;
            EntityRepository = entityRepository;
        }

        #endregion

        #region -- Data Members --

        protected IUnitOfWork UnitOfWork;
        protected IGenericRepository<TEntity> EntityRepository;

        #endregion

        #region -- Methods --

        public void RefreshAllEntities()
        {
            EntityRepository.RefreshAllEntities();
        }

        //public void RefreshEntites()
        //{
        //    EntityRepository.RefreshEntites();
        //}

        public TEntity Create()
        {
            return EntityRepository.Create();
        }

        public IQueryable<TEntity> GetAll()
        {
            return EntityRepository.GetAll();
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
            return EntityRepository.Get(filter, orderBy, includeProperties);
        }

        /// <summary>
        ///     Get the count of entities on the base of conditions as predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public int Count(Func<TEntity, bool> predicate)
        {
            return EntityRepository.Count(predicate);
        }

        /// <summary>
        ///     Retrieve Entity by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public virtual TEntity GetById(object id)
        {
            return EntityRepository.GetById(id);
        }

        /// <summary>
        ///     Find Entity by the condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return EntityRepository.Find(predicate);
        }

        /// <summary>
        ///     Insert single entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Add(TEntity entity)
        {
            Logger.LogHandler.LogInfo("Entity add", Logger.LogHandler.LogType.Notify);
            EntityRepository.Add(entity);
            Logger.LogHandler.LogInfo("Added succesfully", Logger.LogHandler.LogType.Notify);
        }

        /// <summary>
        ///     Bulk Insert the list of entities in the DB Set.
        /// </summary>
        /// <param name="entities">Collection of entities</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            EntityRepository.AddRange(entities);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            EntityRepository.DeleteRange(entities);
        }

        /// <summary>
        ///     Delete entity identified by primary key
        /// </summary>
        /// <param name="id">ID as primary key</param>
        public virtual void Delete(object id)
        {
            EntityRepository.Delete(id);
        }

        /// <summary>
        ///     Bulk Delete the list of entities in the DB Set.
        /// </summary>
        public virtual void Delete(TEntity entityToDelete)
        {
            EntityRepository.Delete(entityToDelete);
        }

        /// <summary>
        ///     Update single entity in the database.
        /// </summary>
        public virtual void Update(TEntity entityToUpdate)
        {
            EntityRepository.Update(entityToUpdate);
        }

        public virtual void Update(TEntity entityToUpdate, string id)
        {
            EntityRepository.Update(entityToUpdate, id);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            EntityRepository.UpdateRange(entities);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, string id)
        {
            EntityRepository.UpdateRange(entities, id);
        }

        // <summary>
        /// Check whether entity exists by id
        public virtual bool Exists(int id)
        {
            return null != EntityRepository.GetById(id);
        }

        public virtual TEntity Trim(TEntity entity)
        {
            var type = typeof(TEntity);

            var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var property in properties)
            {
                var value = (string) property.GetValue(entity);

                if (string.IsNullOrEmpty(value)) continue;

                var stringLengthAttribute = property.GetAttribute<StringLengthAttribute>();

                if (stringLengthAttribute != null && value.Length > stringLengthAttribute.MaximumLength)
                    value = value.Substring(0, stringLengthAttribute.MaximumLength);
                property.SetValue(entity, value);
            }
            return entity;
        }

        public virtual IEnumerable<TEntity> Trim(IEnumerable<TEntity> collection)
        {
            if (null == collection) return null;

            collection = collection.ToList();

            var type = typeof(TEntity);

            var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var entity in collection.ToList())
            {
                foreach (var property in properties)
                {
                    var value = (string) property.GetValue(entity);

                    if (string.IsNullOrEmpty(value)) continue;

                    value = value.TrimEnd();
                    property.SetValue(entity, value);
                }
            }

            return collection;
        }

        public virtual void Save()
        {
            UnitOfWork.Save();
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWork.Dispose();
            }
        }

        #endregion
    }
}