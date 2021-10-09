using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class MposService : BaseService<Mpos>, IMposService
    {
        #region -- Construcotrs --
        public MposService(IUnitOfWork unitOfWork, IGenericRepository<Mpos> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}