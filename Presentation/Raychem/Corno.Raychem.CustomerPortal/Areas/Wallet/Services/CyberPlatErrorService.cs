using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CyberPlatErrorService : BaseService<CyberPlatError>, ICyberPlatErrorService
    {
        #region -- Constructors --
        public CyberPlatErrorService(IUnitOfWork unitOfWork, IGenericRepository<CyberPlatError> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}