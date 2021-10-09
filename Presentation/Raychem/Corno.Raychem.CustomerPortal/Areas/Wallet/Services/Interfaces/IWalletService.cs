
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface IWalletService : IWalletBaseService
    {
        #region -- Methods --

        void CheckWalletBalanceInLimit(ApplicationUser user, double amount);
        ResponseModel Credit(RequestModel viewModel);
        ResponseModel Debit(RequestModel viewModel);
        ApplicationUser GetSocietyAdmin(int societyId, IdentityManager identityManager);
        void ValidateSocietyRequest(SocietyRequestModel requestModel);
        ResponseModel CheckLimit(RequestModel viewModel);
        IEnumerable<WalletTransaction> GetRecentTransaction(RequestModel viewModel);
        ResponseModel BalanceTransfer(RequestModel viewModel);
        ResponseModel WalletCardInterTransaction(RequestModel viewModel);
        ResponseModel CardCredit(RequestModel viewModel);
        ResponseModel CardDebit(RequestModel viewModel);

        #endregion
    }
}