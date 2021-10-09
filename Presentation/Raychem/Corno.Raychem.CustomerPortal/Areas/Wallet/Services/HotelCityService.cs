using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class HotelCityService : BaseService<HotelCity>, IHotelCityService
    {
        #region -- Constructors --
        public HotelCityService(IUnitOfWork unitOfWork, IGenericRepository<HotelCity> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}