
using Corno.Data.Base;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using System;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class SocietyRequestModel
    {
        public string CustomerNumber { get; set; }
        public string DeviceId { get; set; }
        public string CustomerName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string RequestTransactionId { get; set; }
        public string TokenId { get; set; }
        public int SocietyId { get; set; }
        public int Amount { get; set; }
        public int CNumber { get; set; }
    }

    public class SocietyResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string RequestTransactionId { get; set; }
        public string TokenId { get; set; }
        public string TransactionId { get; set; }
    }

    public class Society : BaseModel
    {
        public string SocietyName { get; set; }
        public string AdminUsername { get; set; }
    }

    public class SocietyUserRT : BaseModel
    {
        public string UserID { get; set; }
        public int? SocietyID { get; set; }
    }
}