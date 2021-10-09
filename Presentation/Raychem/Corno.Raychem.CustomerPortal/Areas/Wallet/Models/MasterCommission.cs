using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Base;
using System.ComponentModel;


namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    [Table("MasterCommission")]    
    public class MasterCommission: BaseModel
    {   
        //public int ID { get; set; }
        public string Service { get; set; }
        //[DisplayName("Operator")]
        public string Operator { get; set; }
        [DisplayName("Merchant Credit")]
        public double? MerchantCredit { get; set; }
        [DisplayName("Merchant Debit")]
        public string MerchantDebit { get; set; }
        [DisplayName("Convinience Fee")]
        public string ConvinienceCharges { get; set; }
        //[DisplayName("TDS")]
        public int? TDS { get; set; }
        //[DisplayName("GST")]
        public int? GST { get; set; }                
    }
}