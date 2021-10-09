using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class CancelPolicy
    {        
        public int busid { get; set; }
        public string timebeforedept { get; set; }
        public string cancellationchargetype { get; set; }
        public decimal cancellationcharge { get; set; }        
    }
}