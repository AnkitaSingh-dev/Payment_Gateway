using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface IWalletTransactionService : IBaseService<WalletTransaction>
    {
        #region -- Methods --

        int AddTransaction(RequestModel viewModel);
        //int AddTransaction(RequestModel viewModel);

        #endregion
    }
}