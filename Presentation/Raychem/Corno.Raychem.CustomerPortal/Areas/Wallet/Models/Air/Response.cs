namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Response
    {
        public string PNR { get; set; }
        public int BookingId { get; set; }
        public bool SSRDenied { get; set; }
        public object SSRMessage { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsTimeChanged { get; set; }
        public FlightItinerary FlightItinerary { get; set; }
        public int ResponseStatus { get; set; }
        public string TraceId { get; set; }
    }
}