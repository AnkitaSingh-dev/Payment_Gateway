using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class OperatorSeriesRTService : BaseService<OperatorSeriesRT>, IOperatorSeriesRTService
    {
        #region -- Constructors --
        public OperatorSeriesRTService(IUnitOfWork unitOfWork, IGenericRepository<OperatorSeriesRT> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }

    public class PrepaidPlansService : BaseService<PrepaidPlans>, IPrepaidPlansService
    {
        #region -- Constructors --
        public PrepaidPlansService(IUnitOfWork unitOfWork, IGenericRepository<PrepaidPlans> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}