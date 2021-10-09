namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Bus
{
    public class Response
    {
        public Busitinerary busitinerary { get; set; }
        public bool viewcreditnote { get; set; }
        public bool savependingbooking { get; set; }
        public bool isonbehalfbooking { get; set; }
        public bool IsPriceChanged { get; set; }
        public bool IsTimeChanged { get; set; }
        public int ResponseStatus { get; set; }
        public string TraceId { get; set; }
    }
}