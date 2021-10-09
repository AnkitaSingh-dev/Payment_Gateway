namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Segment
    {
        public int TripIndicator { get; set; }
        public int SegmentIndicator { get; set; }
        public Airline Airline { get; set; }
        public string AirlinePNR { get; set; }
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        public int Duration { get; set; }
        public int GroundTime { get; set; }
        public int Mile { get; set; }
        public bool StopOver { get; set; }
        public string StopPoint { get; set; }
        public string StopPointArrivalTime { get; set; }
        public string StopPointDepartureTime { get; set; }
        public string Craft { get; set; }
        public bool IsETicketEligible { get; set; }
        public string FlightStatus { get; set; }
        public string Status { get; set; }
    }
}