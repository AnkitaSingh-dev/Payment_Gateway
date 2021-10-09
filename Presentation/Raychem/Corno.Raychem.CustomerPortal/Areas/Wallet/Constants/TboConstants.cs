namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Constants
{
    public static class TboConstants
    {
        // Test Credentials
        public const string TestClientId = "\"ApiIntegrationNew\"";
        public const string TestUserName = "\"ever4\"";
        public const string TestPassword = "\"ever4@123\"";
        public const string TestMemberId = "826";
        public const string TestAgencyId = "886";

        // Live Credentials
        public const string LiveClientId = "\"tboprod\"";
        public const string LiveUserName = "\"BOME329\"";
        public const string LivePassword = "\"liveapi@@4321@\"";//"\"travel@@329@\""; "Jugad4ever@"
        public const string EndUserIp = "\"104.211.179.239\"";//"\"43.252.195.170\"";   
        public const string LiveMemberId = "53844";
        public const string LiveAgencyId = "47834";

        //public const string TokenId = "02641cfb-e29c-4282-80d5-8336ab56fd2e";
        //public const string TraceId = "55f5455a-d9aa-4d45-8627-f960897a5181";
        //public const string PNR = "QCWGGP";
        //public const string BookingId = "29455338";
        //public const string TripIndicator = "1";


        // Shared Service
        public const string SharedServiceUrl = "https://api.travelboutiqueonline.com/SharedAPI/SharedData.svc/rest/";

        // Airline
        public const string TestAirlineBaseUrl = "http://api.tektravels.com/SharedServices/SharedData.svc/rest";
        public const string TestAirlineBaseUrl1 = "http://api.tektravels.com/BookingEngineService_Air/AirService.svc/rest";

        public const string LiveAirlineBaseUrl = "https://tboapi.travelboutiqueonline.com/SharedAPI/SharedData.svc/rest";
        public const string LiveAirlineBaseUrl1 = "https://tboapi.travelboutiqueonline.com/AirAPI_V10/AirService.svc/rest";

        public const string TBOAirlineSearchUrl = "https://tboapi.travelboutiqueonline.com/AirAPI_V10/AirService.svc/rest";
        public const string TBOAirlineBookUrl = "https://booking.travelboutiqueonline.com/AirAPI_V10/AirService.svc/rest";

        // Hotel
        public const string TestHotelAuthenticateUrl = "http://api.tektravels.com/SharedServices/SharedData.svc/";
        public const string TestHotelBaseUrl = "http://api.tektravels.com/BookingEngineService_Hotel/hotelservice.svc/";
        public const string TestHotelStaticDataUrl = "http://api.tektravels.com/SharedServices/StaticData.svc/";
        
        public const string LiveHotelBaseUrl = "https://api.travelboutiqueonline.com/HotelAPI_V10/HotelService.svc/";
        public const string LiveHotelStaticDataUrl = "https://api.travelboutiqueonline.com/SharedAPI/StaticData.svc/";

        public const string TBOHotelSearchUrl = "https://api.travelboutiqueonline.com/HotelAPI_V10/HotelService.svc/";
        public const string TBOHotelBookUrl = "https://booking.travelboutiqueonline.com/HotelAPI_V10/HotelService.svc/";

        //Pan Verification
        public const string PanVerify = "http://pan.mobilewaretech.com:2080/SWB-web/api/users/1.0/validatePan";

        //Account Verification
        public const string AccountVerify = "http://mum.mobilewaretech.com:28085/remTransXT/api/1.0/transact/RecipeintAccountEnquiry";

        //Society API
        public const string SSVUrl = "http://www.ssvcredit.com/api/CheckMemberIdAndSocietyId/CheckMemberIdValidOrNot?Memberid=";
        public const string SSCUrl = "http://www.sscredit.org/api/CheckMemberIdAndSocietyId/CheckMemberIdValidOrNot?Memberid=";
        public const string LUCCUrl = "http://www.akedu.org/api/CheckMemberIdAndSocietyId/CheckMemberIdValidOrNot?Memberid=";
        public const string HUMUrl = "http://www.humanapps.co.in/api/CheckMemberIdAndSocietyId/CheckMemberIdValidOrNot?Memberid=";

    }
}