using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air
{
    public class Passenger
    {
        public int PaxId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PaxType { get; set; }
        public string DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpiry { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Fare Fare { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Nationality { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public bool IsLeadPax { get; set; }
        public object FFAirlineCode { get; set; }
        public string FFNumber { get; set; }
        public List<Baggage> Baggage { get; set; }
        public List<MealDynamic> MealDynamic { get; set; }
        public Ticket Ticket { get; set; }
        public List<SegmentAdditionalInfo> SegmentAdditionalInfo { get; set; }
    }
}