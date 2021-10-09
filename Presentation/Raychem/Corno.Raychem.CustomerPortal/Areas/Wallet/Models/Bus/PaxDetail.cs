using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class PaxDetail
    {
        public int busid { get; set; }
        public int paxid { get; set; }
        public string title { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public int age { get; set; }
        public string phoneno { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int gender { get; set; }
    }
}