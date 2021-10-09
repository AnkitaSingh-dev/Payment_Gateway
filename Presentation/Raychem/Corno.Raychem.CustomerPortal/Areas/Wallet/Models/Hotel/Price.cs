using System.Collections.Generic;
namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Hotel
{
    public class Price
    {
        public string CurrencyCode { get; set; }
        public int Email { get; set; }
        public decimal RoomPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal PublishedPrice { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal AgentCommission { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal TDS { get; set; }
        public decimal ExtraGuestCharge { get; set; }
        public decimal OfferedPriceRoundedOff  { get; set; }
        public decimal PublishedPriceRoundedOff { get; set; }        
    }
}