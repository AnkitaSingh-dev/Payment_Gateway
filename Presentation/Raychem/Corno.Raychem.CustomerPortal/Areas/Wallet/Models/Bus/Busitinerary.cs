using System.Collections.Generic;
namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class Busitinerary
    {
        public int busid { get; set; }
        public string ticketno { get; set; }
        public string traveloperatorpnr { get; set; }
        public int sourceid { get; set; }
        public int destinationid { get; set; }
        public string sourcename { get; set; }
        public string destinationname { get; set; }
        public string dateofjourney { get; set; }
        public string bussource { get; set; }
        public bool isdomestic { get; set; }
        public string currency { get; set; }
        public string routeid { get; set; }
        public string bustype { get; set; }
        public string servicename { get; set; }
        public string travelname { get; set; }
        public int blockduration { get; set; }
        public int noofseats { get; set; }
        public string departuretime { get; set; }
        public string arrivaltime { get; set; }
        public decimal totalfare { get; set; }

        public BoardingPointDetails boardingpointdetails { get; set; }
        public List<SeatDetail> seatdetail { get; set; }
        public List<CancelPolicy> cancelpolicy { get; set; }
        public PaxDetail paxdetail { get; set; }
    }
}