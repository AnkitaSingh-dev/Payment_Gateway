using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class DMTTransactionModel
    {
        public DATA DATA { get; set; }
    }

    public class DATA
    {
        public TRANSACTION_DETAILS[] TRANSACTION_DETAILS { get; set; }
    }

    public class TRANSACTION_DETAILS
    {
        public string CUSTOMER_REFERENCE_NO { get; set; }
        public string TRANSACTION_STATUS { get; set; }
    }
}
