using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class SeatDetail
    {
        public int seatid { get; set; }
        public int busid { get; set; }
        public string seatname { get; set; }
        public string seattype { get; set; }
        public string rowno { get; set; }
        public string columnno { get; set; }
        public string seatstatus { get; set; }
        public string seatfare { get; set; }
        public string priceid { get; set; }
        public string isladies { get; set; }
        public string isupper { get; set; }
    }
}