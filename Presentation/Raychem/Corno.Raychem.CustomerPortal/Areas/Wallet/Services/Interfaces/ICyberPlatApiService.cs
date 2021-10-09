
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICyberPlatApiService
    {
        #region -- Methods --

        string GetBalance();
        PrivKey GetSecretKey();
        string ValidationCheck(RequestModel viewModel);
        
        string Payment(RequestModel viewModel);

        string StatusCheck(RequestModel viewModel);

        //double GetCommission(string service, string vOperator, string userType);
        string GetCommissionOperator(string service, string vOperator);

        #endregion
    }
}