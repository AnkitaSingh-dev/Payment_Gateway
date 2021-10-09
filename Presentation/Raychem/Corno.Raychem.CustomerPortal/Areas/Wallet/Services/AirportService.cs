using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class AirportService : BaseService<Airport>, IAirportService
    {
        #region -- Constructors --
        public AirportService(IUnitOfWork unitOfWork, IGenericRepository<Airport> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}