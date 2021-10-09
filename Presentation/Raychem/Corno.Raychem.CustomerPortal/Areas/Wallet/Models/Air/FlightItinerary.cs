using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class FlightItinerary
    {
        public int BookingId { get; set; }
        public string PNR { get; set; }
        public bool IsDomestic { get; set; }
        public int Source { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string AirlineCode { get; set; }
        public string ValidatingAirlineCode { get; set; }
        public string AirlineRemark { get; set; }
        public string AirlineTollFreeNo { get; set; }
        public bool IsLCC { get; set; }
        public bool NonRefundable { get; set; }
        public string FareType { get; set; }
        public Fare Fare { get; set; }
        public List<Passenger> Passenger { get; set; }
        public List<Segment> Segments { get; set; }
        public List<FareRule> FareRules { get; set; }
        public int Status { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceCreatedOn { get; set; }
        public int TicketStatus { get; set; }
        public object Message { get; set; }
    }
}