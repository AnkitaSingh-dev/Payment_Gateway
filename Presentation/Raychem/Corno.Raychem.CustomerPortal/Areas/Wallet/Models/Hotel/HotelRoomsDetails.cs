using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Hotel
{
    public class HotelRoomsDetails
    {
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public bool RequireAllPaxDetails { get; set; }
        public int RoomIndex { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public string RatePlanCode { get; set; }
       
        public Price price { get; set; }
        public List<HotelPassenger> hotelPassenger { get; set; }
    }
}