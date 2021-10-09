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
    public interface IFederalPrePaidCardService : IWalletBaseService
    {
        string GetToken(string request);
        CardResponse DecryptResponse(string responsePayload, string encyType);
        ResponseModel UserRegistration(RequestModel viewModel);
        ResponseModel SCRegistration(RequestModel viewModel);
        ResponseModel UserCardList(RequestModel viewModel);
        ResponseModel CardLUB(RequestModel viewModel);
        ResponseModel ResetPin(RequestModel viewModel);
        ResponseModel UserCardData(RequestModel viewModel);
        ResponseModel GetWalletBalance(RequestModel viewModel);
        ResponseModel ShowUserProfile(RequestModel viewModel);
        ResponseModel ReflectTransaction(RequestModel viewModel);
        ResponseModel SubmitFullKYC(RequestModel viewModel);
        ResponseModel FetchCardTransactions(RequestModel viewModel);
        string Encrypt(string number);
    }
}
