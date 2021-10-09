using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class CyberPlatError : BaseModel
    {
        public string Service { get; set; }
        public string Type { get; set; }
        public string ApiDescription { get; set; }
        public string SimpleDescription { get; set; }
    }
}
