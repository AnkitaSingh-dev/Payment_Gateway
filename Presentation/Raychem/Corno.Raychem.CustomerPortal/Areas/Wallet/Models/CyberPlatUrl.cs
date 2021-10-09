using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class CyberPlatUrl : BaseModel
    {
        public string Service { get; set; }
        public string Operator { get; set; }
        public string Verification { get; set; }
        public string Payment { get; set; }
        public string StatusCheck { get; set; }
        public double? CustomerCommission { get; set; }
        public double? MerchantCommission { get; set; }
        public string OperatorComissionName { get; set; }
    }
}
