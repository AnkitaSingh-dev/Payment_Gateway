using Corno.Data.Common;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Common.Interfaces;

namespace Corno.Services.Common
{
    public class CityService : MasterService<City>, ICityService
    {
        public CityService(IUnitOfWork unitOfWork, IGenericRepository<City> repository)
            : base(unitOfWork, repository)
        {
        }
    }
}