using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Services
{
    public class AspNetRoleService : BaseService<AspNetRole>, IAspNetRoleService
    {
        public AspNetRoleService(IUnitOfWork unitOfWork, IGenericRepository<AspNetRole> aspnetroleRepository)
            : base(unitOfWork, aspnetroleRepository)
        {
        }
    }
}