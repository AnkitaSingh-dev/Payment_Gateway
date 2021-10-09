using System.Collections.Generic;
namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class BoardingPointDetails
    {
        public int citypointid { get; set; }
        public int busid { get; set; }
        public string citypointname { get; set; }
        public string citypointlocation { get; set; }
        public string citypointlandmark { get; set; }
        public string citypointaddress { get; set; }
        public string citypointcontactnumber { get; set; }
        public string citypointtime { get; set; }     
    }
}