using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface IPrePaidCardService : IWalletBaseService
    {
        string GetToken(string request);
        CardResponse DecryptResponse(string responsePayload, string encyType);
        ResponseModel CreateUser(RequestModel viewModel);
        ResponseModel MobileOTPVerification(RequestModel viewModel);
        ResponseModel AadhaarRegistration(RequestModel viewModel);
        ResponseModel PanRegistration(RequestModel viewModel);
       //ResponseModel VerifyAadhaarOTP(RequestModel viewModel);
        ResponseModel FetchCardUserProfile(RequestModel viewModel);
        ResponseModel FetchBankWalletKYCStatus(RequestModel viewModel);
        ResponseModel BlockUnblockCloseCardWallet(RequestModel viewModel);
        ResponseModel SubmitFullKYCRequest(RequestModel viewModel);
        ResponseModel ReflectTransaction(RequestModel viewModel);       
        ResponseModel UpdateCustomerDetails(UserViewModel viewModel);
        ResponseModel AddBeneficiary(RequestModel viewModel);
        ResponseModel FetchBeneficiary(RequestModel viewModel);
        //ResponseModel GetCardDetails(RequestModel viewModel);
        ResponseModel UpdateAadhaar(RequestModel viewModel);
        ResponseModel IsCardUser(RequestModel viewModel);
        ResponseModel AddPhyscicalCard(RequestModel viewModel);
        ResponseModel PhyscialCardLUB(RequestModel viewModel);
        ResponseModel PhysicalCardSetPin(RequestModel viewModel);
        ResponseModel FetchVirtualCard(RequestModel viewModel);
        ResponseModel BlockVirtualCard(RequestModel viewModel);
        ResponseModel GetCheckPartnerBalance();
        ResponseModel ReQuery(RequestModel viewModel);
        ResponseModel TransactionHistory(RequestModel viewModel);
        bool CardTransactionStatus(int Id);
        string Encrypt(string number);
        string Decrypt(string number);
    }
}
