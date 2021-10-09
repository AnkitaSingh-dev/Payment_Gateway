using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Constants
{
    public static class PrePaidConstants
    {
        public const string AggregatorId = "AGGR001011";  //Prod  AGGR001008
        //public const string AggregatorId = "AGGR002557";//Test
        public const string CountryCode = "INR";
        public const string Country = "India";
        public const string CardURL = "https://wallet.4everpayapi.co.in/agWalletAPI/v2/agg"; // Production URL
        //public const string CardURL = "http://114.143.100.102:81/agWalletAPI/v1/agg"; // Test URL
        //public const string CardURL = "http://114.143.100.102:91/agWalletAPI/v2/agg";
        public const string OperatingSystem = "MOBILE";
        public const string IdentifierType = "mobile";
        public const string ChangePinMode = "M";

        //User type constants
        public const int TypeCustomer = 1;
        public const string TypeAgent = "2";
        public const int LoginStatus = 0;

        //Request type constants
        public const string RequestTypeUser = "USR";
        public const string RequestTypeTxn = "WTW";
        public const string RequestTypeOnboarding = "ONBO";
        public const string RequestTypePPC = "PPC";

        //Others
        public const string ImageFormat = "image/jpg";
        public const string Aadhaar = "aadhaar";
        public const string Pan = "pan";

        //request sub type constants
        public const string SubTypeRegister = "REG";
        public const string SubTypeMobileOTP = "MOTP";
        public const string SubTypeAadhaarReg = "RAR";
        public const string SubTypeAadhaarVerify = "AOTP";
        public const string SubTypeProfileFetch = "PRF";
        public const string SubTypeBankKYC = "BWKS";
        public const string SubTypeBlockUnblock = "BUCW";
        public const string SubTypeFullKYC = "SFK";
        public const string SubTypeReflectTxn = "PTR";
        public const string SubTypeCardReg = "SCRR";
        public const string SubTypeCardAccept = "SAR";
        public const string SubTypeSetPin = "RPIN"; 
        public const string SubTypeLockUnlockBlock = "SULB";
        public const string SubTypeCardReplace = "SCR";
        public const string SubTypeCardList = "GCL";
        public const string SubTypeUpdate = "SCU";
        public const string SubTypeAddBene = "ADDBE";
        public const string SubTypeFetchBene = "FBENE";
        public const string SubTypeGetDetails = "SCC";
        public const string SubTypeUpdateAadhaar = "UAN";
        public const string SubTypeCardActive = "CAR";
        public const string SubTypeTxnStatus = "STS";
        public const string SubTypeAPC = "ACPC";
        public const string SubTypePSP = "SPCP";
        public const string SubTypeFVC = "FVC";
        public const string SubTypeBVC = "BVC";
        public const string SubTypePCLUB = "CLUB";
        public const string SubTypeSubmitFullKYC = "MMKYC";
        public const string SubTypeGetPin = "SGP";
        public const string SubTypeAadhaarRegx = "DARA";
        public const string SubTypePanRegx = "RUP";
        public const string SubTypeCPB = "CPB";
        public const string SubTypeTSC = "TSC";
        public const string SubTypeTH = "TH";
        public const string SubTypeUCL = "UCL";
        public const string SubTypeUCD = "UCD";
        public const string SubTypeGWB = "GWB";
        public const string subTypeGUWCT = "GUWCT";
        
        //Card status
        public const string Lock = "L";
        public const string Unlock = "UL";
        public const string Block = "BL";

        //KYC Status
        public const string UnderReview = "Under Review";
        public const string Accepted = "Accepted";
        public const string SubmitKYC = "Submit KYC";
        public const string Rejected = "Rejected";
    }
}