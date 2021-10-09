using System.Collections;
using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class WalletDashboardViewModel
    {
        public string FullName { get; set; }
        public double? WalletBlance { get; set; }

        public IEnumerable Services { get; set; }
        public IEnumerable Operators { get; set; }
        public IEnumerable PaymentModes { get; set; }
        public IEnumerable Users { get; set; }
    }

    public class FCMResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<FCMResult> results { get; set; }
    }

    public class FCMResult
    {
        public string message_id { get; set; }
    }
}

