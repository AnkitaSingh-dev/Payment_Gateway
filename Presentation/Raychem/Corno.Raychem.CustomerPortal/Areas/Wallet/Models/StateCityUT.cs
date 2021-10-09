using Corno.Data.Base;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using System;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class StateCityUT : BaseModel
    {
        public string City { get; set; }
        public string State { get; set; }
        public string FederalState { get; set; }
    }

    public class JugadUserSafexUserRT: BaseModel
    {
        public string UserName { get; set; }
        public string CustId { get; set; }
        public string PKitNo { get; set; }
        public bool IsCardUser { get; set; }
        public bool IsVCardBlock { get; set; }
        public string VCardId { get; set; }
        public string PCardId { get; set; }
        public string VStatus { get; set; }
        public string PStatus { get; set; }
    }

    public class PrePaidCardKitNo : BaseModel
    {
        public string KitNo { get; set; }
        public string Username { get; set; }
        public bool IsAllocated { get; set; }
        public string ValidationNumber { get; set; }
    }

    public class ElectricityDetails
    {
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string DueDate { get; set; }
        public double Amount { get; set; }
    }
}