using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class Mpos : BaseModel
    {
        public string MposSerialNo { get; set; }
        public string Mid { get; set; }
        public string Tid { get; set; }
        public string Version { get; set; }
    }
}
