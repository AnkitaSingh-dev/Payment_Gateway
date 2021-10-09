using System;
using System.Web;
using System.Web.Script.Serialization;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Base;
using Corno.Services.Base.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class WalletTransactionService : BaseService<WalletTransaction>, IWalletTransactionService
    {
        #region -- Constructors --
        public WalletTransactionService(IUnitOfWork unitOfWork, IGenericRepository<WalletTransaction> walletTransactionRepository)
            : base(unitOfWork, walletTransactionRepository)
        {
        }
        #endregion

        #region -- Methods --

        public int AddTransaction(RequestModel viewModel)
        {
            LogHandler.LogInfo("Creating model of Transaction", LogHandler.LogType.Notify);

            var model = new WalletTransaction
            {
                CyberPlatTransId =  viewModel.CyberPlatTransId,
                OperatorTransId = viewModel.OperatorTransId,
                PaymentTransactionId = viewModel.PgTxnId,
                Pnr = viewModel.Pnr,
                BookingId = viewModel.BookingId,
                TransactionDate = DateTime.Now,
                Service =  viewModel.Service,
                Operator = viewModel.Operator,
                PaymentMode = viewModel.PaymentMode,
                Source = viewModel.Service == ServiceConstants.BalanceTransfer ? (viewModel.Credit > viewModel.Debit) ? viewModel.ToUserName : viewModel.UserName : viewModel.UserName,
                Destination = viewModel.Number,
                Amount = viewModel.Amount,
                Commission = viewModel.Commission,
                OpeningBalance = viewModel.OpeningBalance,
                Credit = viewModel.Credit,
                Debit = viewModel.Debit,
                ClosingBalance = viewModel.ClosingBalance,
                Request = viewModel.Request,
                Response = viewModel.Response,
                UserName = viewModel.UserName,
                DeviceId = viewModel.DeviceId,
                EndUserIp = HttpContext.Current.Request.UserHostAddress,
                Status = viewModel.State == null ? "Success" : viewModel.State
            };

            LogHandler.LogInfo("Adding model of Transaction", LogHandler.LogType.Notify);
            Add(model);
            LogHandler.LogInfo("Saving model of Transaction", LogHandler.LogType.Notify);
            LogHandler.LogInfo(new JavaScriptSerializer().Serialize(model), LogHandler.LogType.Notify);
            Save();
            LogHandler.LogInfo("Returning the Id " + model.Id, LogHandler.LogType.Notify);

            return model.Id;
        }
       
        #endregion
    }
}