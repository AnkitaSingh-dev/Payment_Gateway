using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class MasterTransactionService : BaseService<MasterTransaction>, IMasterTransactionService
    {
        #region -- Constructors --
        public MasterTransactionService(IUnitOfWork unitOfWork, IGenericRepository<MasterTransaction> masterTransactionRepository)
            : base(unitOfWork, masterTransactionRepository)
        {
        }
        #endregion
    }
}