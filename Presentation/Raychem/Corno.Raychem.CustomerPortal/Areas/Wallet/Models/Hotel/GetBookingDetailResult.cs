using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Hotel
{
    public class GetBookingDetailResult
    {
        public bool VoucherStatus { get; set; }
        public int ResponseStatus { get; set; }
        public string TraceId { get; set; }
        public int Status { get; set; }
        public string HotelBookingStatus { get; set; }
        public string ConfirmationNo { get; set; }
        public string BookingRefNo { get; set; }
        public int BookingId { get; set; }
        public bool IsPriceChanged { get; set; }
        public string IsCancellationPolicyChanged { get; set; }
        public string HotelPolicyDetail { get; set; }
        public string InvoiceCreatedOn { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string HotelConfirmationNo { get; set; }
        public string HotelName { get; set; }
        public int StarRating { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string City { get; set; }       
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string LastCancellationDate { get; set; }
        public string NoOfRooms { get; set; }
        public string BookingDate { get; set; }
        public bool IsDomestic { get; set; }
        public int AgentReferenceNo { get; set; }
        //public string LastCancellationDate { get; set; }
        //public string NoOfRooms { get; set; }
        //public string BookingDate { get; set; }

        public Error error { get; set; }
        public List<HotelRoomsDetails> hotelRoomsDetails { get; set; }
    }
}