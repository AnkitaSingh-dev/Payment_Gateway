using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Airport : BaseModel
    {
        public string AirportName { get; set; }
        public string AirportCode { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Nationalty { get; set; }
        public string Currency { get; set; }
    }
}
