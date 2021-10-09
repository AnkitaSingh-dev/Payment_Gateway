using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface IMobileWareApiService
    {
        #region -- Methods --
        
        PanResponse PANValidateURL(KYCModel viewModel);
        AccountResponse AccountValidateURL(KYCModel req, string hash, ApplicationUser user);

        #endregion
    }
}