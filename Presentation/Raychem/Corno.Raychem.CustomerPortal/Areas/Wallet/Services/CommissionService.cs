using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;
using Corno.Services.Bootstrapper;
using System.Linq;
using System;
using Corno.Data.Login;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CommissionService : BaseService<Commission>, ICommissionService
    {
        #region -- Constructors --
        public CommissionService(IUnitOfWork unitOfWork, IGenericRepository<Commission> commissionRepository)
            : base(unitOfWork, commissionRepository)
        {
        }
        #endregion
        public void AddCommission(Commission viewModel)
        {
            Add(viewModel);
            Save();
        }
    }
}