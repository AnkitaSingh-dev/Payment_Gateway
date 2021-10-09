
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICyberPlatGiftCardService : IWalletBaseService
    {
        #region -- Methods --

        ResponseModel Validate(RequestModel viewModel);
        ResponseModel Payment(RequestModel viewModel);

        #endregion
    }
}