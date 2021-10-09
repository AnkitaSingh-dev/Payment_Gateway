using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CyberPlatUrlService : BaseService<CyberPlatUrl>, ICyberPlatUrlService
    {
        #region -- Constructors --
        public CyberPlatUrlService(IUnitOfWork unitOfWork, IGenericRepository<CyberPlatUrl> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}