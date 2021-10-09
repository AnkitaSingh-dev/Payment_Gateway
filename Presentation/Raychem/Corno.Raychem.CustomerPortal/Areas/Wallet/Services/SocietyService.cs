using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Bootstrapper;
using System;
using System.Linq;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class SocietyService : BaseService<Society>, ISocietyService
    {
        #region -- Constructors --
        public SocietyService(IUnitOfWork unitOfWork, IGenericRepository<Society> repository)
            : base(unitOfWork, repository)
        {
        }
        #endregion
    }
}