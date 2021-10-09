using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Services
{
    public class AspNetUserService : BaseService<AspNetUser>, IAspNetUserService
    {
        public AspNetUserService(IUnitOfWork unitOfWork, IGenericRepository<AspNetUser> aspnetuserRepository)
            : base(unitOfWork, aspnetuserRepository)
        {
        }
    }
}