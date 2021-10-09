namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class FareRule
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string FareBasisCode { get; set; }
        public string FareRuleDetail { get; set; }
        public object FareRestriction { get; set; }
    }
}