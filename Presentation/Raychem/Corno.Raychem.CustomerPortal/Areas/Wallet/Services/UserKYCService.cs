using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class UserKYCService : BaseService<UserKYCModel>, IUserKYCService
    {
        #region -- Constructors --
        public UserKYCService(IUnitOfWork unitOfWork, IGenericRepository<UserKYCModel> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}