using System;
using Corno.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    [Table("MasterCommission")]
    public class MasterTransaction : BaseModel
    {
        public string Service { get; set; }
        public string Operator { get; set; }
        public double? MerchantCredit { get; set; }
        public string MerchantDebit { get; set; }
        public string ConvinienceCharges { get; set; }
        public int? TDS { get; set; }
        public int? GST { get; set; }
    }
}