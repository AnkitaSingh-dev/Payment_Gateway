using Corno.Data.Common;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICommissionService : IBaseService<Commission>
    {
        void AddCommission(Commission viewModel);
    }
}