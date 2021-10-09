using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class BusCityService : BaseService<BusCity>, IBusCityService
    {
        #region -- Constructors --
        public BusCityService(IUnitOfWork unitOfWork, IGenericRepository<BusCity> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}