using Corno.Data.Base;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class HotelCity : MasterModel
    {
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }

    public class HotelResponse {
        public bookResult BookResult { get; set; }
        public string TraceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ConfirmationNumber { get; set; }
        public string BookingId { get; set; }
    }

    public class bookResult
    {
        public errorBody Error { get; set; }
    }
    public class errorBody
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
