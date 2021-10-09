using System;
using Corno.Data.Base;
using Corno.Services.Base.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Corno.Services.Base
{
    public class MasterService<TEntity> : BaseService<TEntity>, IMasterService<TEntity>
        where TEntity : class
    {
        #region -- Constructors --

        public MasterService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> repository)
            : base(unitOfWork, repository)
        {
        }

        #endregion

        #region -- Methods --
        public override void Add(TEntity entity)
        {
            var masterModel = entity as MasterModel;
            if (null == masterModel)
                throw new Exception("Invalid Entity");
            if (string.IsNullOrEmpty(masterModel.Name))
                throw new Exception("Name is required.");

            base.Add(entity);
        }

        public IList<MasterViewModel> GetIdNameList()
        {
            var entities = EntityRepository.Get().ToList().ConvertAll(x => x as MasterModel);
            return entities.Select(m => new MasterViewModel
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();
        }

        public string GetName(int id)
        {
            var entity = EntityRepository.GetById(id) as MasterModel;
            return entity?.Name;
        }
        #endregion
    }
}