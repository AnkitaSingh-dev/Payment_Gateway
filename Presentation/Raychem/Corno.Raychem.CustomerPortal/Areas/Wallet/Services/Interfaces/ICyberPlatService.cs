using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using System;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ICyberPlatService : IWalletBaseService
    {
        #region -- Methods --

        ResponseModel Validate(RequestModel viewModel);
        ResponseModel Payment(RequestModel viewModel);
        string GetBalance();
        void MposTransactionCapture(MposResponseModel mposModel);
        string GetRSA(RequestModel viewModel);
        ResponseModel SaveFCMToken(RequestModel viewModel);
        bool Notification(Notification viewModel, string sendType);
        ResponseModel FetchElectricityBillDetails(RequestModel viewModel);
        ResponseModel FetchOperator(RequestModel viewModel);
        ResponseModel FetchPrepaidPlans(RequestModel viewModel);

        #endregion
    }
}