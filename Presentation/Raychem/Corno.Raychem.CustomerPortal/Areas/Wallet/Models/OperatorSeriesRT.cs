using Corno.Data.Base;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using System;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class OperatorSeriesRT : BaseModel
    {
        public string Series { get; set; }
        public string Operator { get; set; }
        public string Circle { get; set; }
    }

    public class PrepaidPlans : BaseModel
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Validity { get; set; }
        public string Operator { get; set; }
        public string Circle { get; set; }
        public string Category { get; set; }
    }
}