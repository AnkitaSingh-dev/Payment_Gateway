using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class PanRequest
    {
        public string TenantCode { get; set; }
        public string UserName { get; set; }
        public string PanNumber { get; set; }
        public metaData MetaData { get; set; }
        public int? Dt { get; set; }
    }

    public class metaData
    {
        public string Udc { get; set; }
        public string Pip { get; set; }
        public string Lot { get; set; }
        public string Lov { get; set; }
    }

    public class PanResponse
    {
        public string ErrorMessage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public string MiddleName { get; set; }
        public string Pan { get; set; }
        public string Rdt { get; set; }
        public string Rrn { get; set; }
        public string Status { get; set; }
    }

    public class AccountResponse
    {
        public string ErrorMsg { get; set; }
        public string ErrorCode { get; set; }
        public string Reason { get; set; }
    }

}
