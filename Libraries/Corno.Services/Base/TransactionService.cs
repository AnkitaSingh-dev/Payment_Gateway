using Corno.Services.Base.Interfaces;

namespace Corno.Services.Base
{
    public class TransactionService<TEntity> : BaseService<TEntity>, ITransactionService<TEntity>
        where TEntity : class
    {
        #region -- Constructors --

        public TransactionService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> entityRepository)
            : base(unitOfWork, entityRepository)
        {
            //this._unitOfWork = unitOfWork;
        }

        #endregion
    }
}