namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Baggage
    {
        public int WayType { get; set; }
        public string Code { get; set; }
        public int Description { get; set; }
        public int Weight { get; set; }
        public string Currency { get; set; }
        public int Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}