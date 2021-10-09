namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string IssueDate { get; set; }
        public string ValidatingAirline { get; set; }
        public string Remarks { get; set; }
        public string ServiceFeeDisplayType { get; set; }
        public string Status { get; set; }
    }
}