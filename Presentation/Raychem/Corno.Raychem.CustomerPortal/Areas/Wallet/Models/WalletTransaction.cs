using System;
using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class WalletTransaction : BaseModel
    {
        public string CyberPlatTransId { get; set; }
        public string OperatorTransId { get; set; }
        public string PaymentTransactionId { get; set; }
        public string Pnr { get; set; }
        public string BookingId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Service { get; set; }
        public string Operator { get; set; }
        public string PaymentMode { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public double Amount { get; set; }
        public double? Commission { get; set; }
        public double? OpeningBalance { get; set; }
        public double? Credit { get; set; }
        public double? Debit { get; set; }
        public double? ClosingBalance { get; set; }
        public string UserName { get; set; }
        public string DeviceId { get; set; }
        public string EndUserIp { get; set; }

        public string Request { get; set; }
        public string Response { get; set; }    
    }

    public class SafexWalletTransaction : BaseModel {

        public string UserId { get; set; }
        public string SafexRefId { get; set; }
        public string SwitchRefId { get; set; }
        public double? Amount { get; set; }
        public string UserName { get; set; }
        public string WalletTransactionId { get; set; }
    }
}
