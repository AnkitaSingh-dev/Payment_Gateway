
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICyberPlatDmtService : IWalletBaseService
    {
        #region -- Methods --

        ResponseModel Validate(RequestModel viewModel);
        ResponseModel Payment(RequestModel viewModel);
        ResponseModel IsUserValidAgent(RequestModel viewModel);
        RequestModel GetTransactionDetail(string transactionId);
        ResponseModel Refund(RequestModel viewModel);

        #endregion
    }
}