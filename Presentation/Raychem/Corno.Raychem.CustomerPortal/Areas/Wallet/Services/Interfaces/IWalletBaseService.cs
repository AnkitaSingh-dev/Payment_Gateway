
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface IWalletBaseService
    {
        #region -- Methods --

        ApplicationUser ValidateServiceViewModel(RequestModel requestModel);
        ApplicationUser GetUser(string cypherText, IdentityManager identityManager);
        void ValidateNumber(RequestModel viewModel);
        string Decrypt(string cypherText, string initVector);
        string Encrypt(string value, string initVector);
        void ValidateUserAgent(string agent);
        void ValidateDeviceId(RequestModel requestModel, ApplicationUser user);

        #endregion
    }
}