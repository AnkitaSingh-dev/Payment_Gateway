using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using System.Web;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Services.Bootstrapper;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CommissionApiServices : ICommissionApiServices
    {
        #region -- Constructors --

        public CommissionApiServices(ICommissionService commissionService, IMasterTransactionService masterTransactionService,
            ILogService logService)
        {
            _masterTransactionService = masterTransactionService;
            _commissionService = commissionService;
            _logService = logService;
        }
        #endregion

        #region -- Data Members --

        private readonly IMasterTransactionService _masterTransactionService;
        private static ICommissionService _commissionService;
        private readonly ILogService _logService;
        #endregion

        #region -- Methods --
        public double GetConvinienceCharges(string service, string oper, double amount)
        {
            var _masterTransactionServ = (IMasterTransactionService)Bootstrapper.GetService(typeof(MasterTransactionService));
            var masterTransactions = _masterTransactionServ.Get(a => a.Service == service && a.Operator == oper).FirstOrDefault();

            if (null == masterTransactions)
                throw new Exception("Validation Url for service ( " + service + " ) and operator ( " + oper + " ) is not available in the system.");

            return masterTransactions.ConvinienceCharges.Contains("%") ?
                       ((Convert.ToDouble(masterTransactions.ConvinienceCharges.Replace('%', ' ')) / 100) * amount) :
                        (Convert.ToDouble(masterTransactions.ConvinienceCharges));
        }
        public double GetGst (double amount)
        {
            return amount * (0.18);
        }
        public double GetCommissionDmt(double amount, ApplicationUser user)
        {
            try
            {
                var _masterTransactionServ = (IMasterTransactionService)Bootstrapper.GetService(typeof(MasterTransactionService));
                var masterTransactions = _masterTransactionServ.Get(a => a.Service == ServiceConstants.Dmt && a.Operator == ServiceConstants.DmtOperator).FirstOrDefault();

                var commission = new Commission();

                commission.ServiceId = masterTransactions.Id;
                var cc = masterTransactions.ConvinienceCharges.Contains("%") ?
                        ((Convert.ToDouble(masterTransactions.ConvinienceCharges.Replace('%', ' ')) / 100) * amount) :
                         (Convert.ToDouble(masterTransactions.ConvinienceCharges));
                commission.ConvinienceFee = cc > 20 ? cc : 20;
                commission.Gst = masterTransactions.GST;
                commission.GstAmount = commission.ConvinienceFee * (commission.Gst / 100);
                commission.Tds = masterTransactions.TDS;

                commission.TransactionAmount = Math.Round((double)(amount + commission.ConvinienceFee),4);
                cc = Math.Round(( masterTransactions.MerchantDebit.Contains("%") ?
                    ((Convert.ToDouble(masterTransactions.MerchantDebit.Replace('%', ' ')) / 100) * amount) : Convert.ToDouble(masterTransactions.MerchantDebit)), 4);

                commission.GrossComission = user.UserType == UserTypeConstants.Customer ? 0.0 : cc > 10 ? cc : 10 ; //UserTypeConstants.Merchant ? cc > 10 ? cc : 10 : 0.0;
                commission.BasicComission = commission.GrossComission;
                commission.TdsAmount = ((Convert.ToDouble(masterTransactions.TDS)) / 100) * commission.GrossComission;
                commission.PayableAmount = Math.Round((double)(commission.GrossComission - commission.TdsAmount), 4); ;

                commission.Code = user.Id;
                commission.CreatedBy = user.FirstName + user.LastName;
                commission.ModifiedBy = user.UserName;

                var _commissionService = (ICommissionService)Bootstrapper.GetService(typeof(CommissionService));
                _commissionService.AddCommission(commission);

                return (double)commission.PayableAmount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public double? GetCommission(string service, string oper, double amount, ApplicationUser user)
        {
            try
            {
                var _masterTransactionService = (IMasterTransactionService)Bootstrapper.GetService(typeof(MasterTransactionService));
                var masterTransactions = _masterTransactionService.Get(a => a.Service == service && a.Operator == oper).FirstOrDefault();

                if (null == masterTransactions)
                    throw new Exception("Validation Url for service ( " + service + " ) and operator ( " + oper + " ) is not available in the system.");

                var commission = new Commission();

                commission.ServiceId = masterTransactions.Id;

                commission.ConvinienceFee = masterTransactions.ConvinienceCharges.Contains("%") ?
                        ((Convert.ToDouble(masterTransactions.ConvinienceCharges.Replace('%', ' ')) / 100) * amount) :
                         (Convert.ToDouble(masterTransactions.ConvinienceCharges));

                commission.TransactionAmount = masterTransactions.ConvinienceCharges.Contains("%") ?
                    amount + ((Convert.ToDouble(masterTransactions.ConvinienceCharges.Replace('%', ' ')) / 100) * amount) :
                    amount + (Convert.ToDouble(masterTransactions.ConvinienceCharges));

                commission.Gst = masterTransactions.GST;
                commission.GstAmount = commission.ConvinienceFee * ((Convert.ToDouble(masterTransactions.GST)) / 100);

                commission.GrossComission = user.UserType == UserTypeConstants.Customer ? 0.0 : commission.TransactionAmount * (Convert.ToDouble(masterTransactions.MerchantCredit / 100)); //UserTypeConstants.Merchant ? commission.TransactionAmount * (Convert.ToDouble(masterTransactions.MerchantCredit / 100)) : 0.0;
                commission.BasicComission = user.UserType == UserTypeConstants.Customer ? 0.0 : commission.GrossComission + Math.Round((masterTransactions.MerchantDebit.Contains("%") ? ((Convert.ToDouble(masterTransactions.MerchantDebit.Replace('%', ' ')) / 100) * amount) : Convert.ToDouble(masterTransactions.MerchantDebit)), 4); //UserTypeConstants.Merchant ? commission.GrossComission + Math.Round((masterTransactions.MerchantDebit.Contains("%") ? ((Convert.ToDouble(masterTransactions.MerchantDebit.Replace('%', ' ')) / 100) * amount) : Convert.ToDouble(masterTransactions.MerchantDebit)), 4):0.0;

                commission.Tds = masterTransactions.TDS;
                commission.TdsAmount = commission.BasicComission * ((Convert.ToDouble(masterTransactions.TDS)) / 100);

                commission.PayableAmount = Math.Round((double)(commission.BasicComission - commission.TdsAmount),4);

                commission.Code = user.Id;
                commission.CreatedBy = user.FirstName + user.LastName;
                commission.ModifiedBy = user.UserName;

                var _commissionService = (ICommissionService)Bootstrapper.GetService(typeof(CommissionService));
                _commissionService.AddCommission(commission);

                return commission.PayableAmount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion
    }
}