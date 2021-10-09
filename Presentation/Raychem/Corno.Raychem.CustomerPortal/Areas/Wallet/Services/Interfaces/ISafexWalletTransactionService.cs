using Corno.Data.Common;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Services.Base.Interfaces;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ISafexWalletTransactionService : IBaseService<SafexWalletTransaction>
    {
        void AddSafexWalletTransaction(SafexWalletTransaction viewModel);
    }
}