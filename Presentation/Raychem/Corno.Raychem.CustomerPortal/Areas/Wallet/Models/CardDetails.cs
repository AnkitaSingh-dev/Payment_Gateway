using Corno.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class CardDetails
    {
        public string CardNumber { get; set; }
        public string Card_Price { get; set; }
        public string Expiry_Date { get; set; }
        public string CVV { get; set; }
        public string Name { get; set; }
    }

    public class Transaction
    {
        public Order_Status_Result Order_Status_Result { get; set; }
    }

    [Table("PGTransactions")]
    public class Order_Status_Result : BaseModel
    {
        public string order_gtw_id { get; set; }
        public string order_no { get; set; }
        public string order_amt { get; set; }
        public string order_card_name { get; set; }
        public string order_ship_name { get; set; }
        public string order_date_time { get; set; }
        public string order_ip { get; set; }
        public string order_option_type { get; set; }
        public string order_bank_ref_no { get; set; }
        public string reference_no { get; set; }
        public string order_bank_response { get; set; }
    }
}