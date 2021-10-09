using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class PrePaidCardModel
    {
        public class header
        {
            public string ipAddress { get; set; }
            public string userAgent { get; set; }
            public string operatingSystem { get; set; }
            public Guid sessionId { get; set; }
            public string version { get; set; }
            public string date { get; set; }
            public string requestId { get; set; }
        }

        public class userInfo
        {
            public string name { get; set; }
            public string mobileNo { get; set; }
            public string type { get; set; }
            public string createdBy { get; set; }
            public string aggregatorId { get; set; }
            public string distributorId { get; set; }
            public string agentId { get; set; }
            public string subAgentId { get; set; }
            public string aadharNo { get; set; }
            public string emailId { get; set; }
            public string id { get; set; }
            public string walletId { get; set; }
            public double? balance { get; set; }
        }

        public class transaction
        {
            public string requestType { get; set; }
            public string requestSubType { get; set; }
            public string channel { get; set; }
            public string agId { get; set; }
            public string meId { get; set; }
            public int? tranCode { get; set; }
            public string responseMsg { get; set; }
            public string responseCode { get; set; }
            public string countryCode { get; set; }
            public string otp { get; set; }
            public string otp_ref_number { get; set; }
            public string token { get; set; }
            public string txnId { get; set; }
            public string otpTxnId { get; set; }
            public double? txnAmt { get; set; }
            public string userId { get; set; }
            public string id { get; set; }
            public string pageNo { get; set; }
        }

        public class response
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class rbiRequest
        {
            public string p1 { get; set; }
            public string p2 { get; set; }
            public string p3 { get; set; }
            public string p4 { get; set; }
            public string p5 { get; set; }
            public string p6 { get; set; }
            public string p7 { get; set; }
            public string p8 { get; set; }
            public string aggId { get; set; }
            public idProofImages id_proof_image_1 { get; set; }
            public idProofImages id_proof_image_2 { get; set; }
            public idProofImages addr_proof_image_1 { get; set; }
            public idProofImages addr_proof_image_2 { get; set; }
            public string action_name { get; set; }
            public string kt { get; set; }
            public string id { get; set; }
            public string transaction_id { get; set; }
            public double transaction_amount { get; set; }
        }

        public class rbiResponse
        {
            public string status_code { get; set; }
            public string kyc_status { get; set; }
            public string wallet_status { get; set; }
            public string id { get; set; }
            public string bank_status { get; set; }
            public string message { get; set; }
            public string walletCode { get; set; }
            public string code { get; set; }
            public string url { get; set; }
            public double? balance { get; set; }
            public double? load_balance { get; set; }
            public double? spend_balance { get; set; }
            public dynamic transactions { get; set; }
        }

        public class idProofImages
        {
            public string data { get; set; }
            public string content_type { get; set; }
            public string filename { get; set; }
        }

        public class FullKYC
        {
            public header header { get; set; }
            public userInfo userInfo { get; set; }
            public transaction transaction { get; set; }
            public rbiRequest rbiRequest { get; set; }
            public response response { get; set; }
        }

        public class CardRequest
        {
            public header header { get; set; }
            public userInfo userInfo { get; set; }
            public transaction transaction { get; set; }
            public txnReflection txnReflection { get; set; }
            public dynamic recharge { get; set; }
            public Array[] passbook { get; set; }
            public Array[] merchantCustPassbook { get; set; }
            public dynamic transactionStatus { get; set; }
            public Array[] smartCardUserList { get; set; }
            public dynamic beneficiaryInfo { get; set; }
            public Array[] beneficiaryList { get; set; }
            public dynamic rechargeData { get; set; }
            public rbiRequest rbiRequest { get; set; }
            public smartCardModel smartCardModel { get; set; }
            public walletMastBean walletMastBean { get; set; }
            public kycMastBean kycMastBean { get; set; }
        }

        public class CardResponse
        {
            public header header { get; set; }
            public userInfo userInfo { get; set; }
            public transaction transaction { get; set; }
            public response response { get; set; }
            public dynamic recharge { get; set; }
            public Array[] passbook { get; set; }
            public Array[] merchantCustPassbook { get; set; }
            public dynamic transactionStatus { get; set; }
            public Array[] smartCardUserList { get; set; }
            public beneficiaryInfo beneficiaryInfo { get; set; }
            public List<beneficiaryList> beneficiaryList { get; set; }
            public dynamic rechargeData { get; set; }
            public rbiRequest rbiRequest { get; set; }
            public rbiResponse rbiResponse { get; set; }
            public txnReflection txnReflection { get; set; }
            public yesPayVirtualCardResponse yesPayVirtualCardResponse { get; set; }
            public cardTransactionReflection cardTransactionReflection { get; set; }
            public smartCardModel smartCardModel { get; set; }
            public walletMastBean walletMastBean { get; set; }
            public mmResponse mmResponse { get; set; }
        }

        [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
        public class cardRequestModel
        {
            public string agId { get; set; }
            public string meId { get; set; }
            public string payload { get; set; }
            public string encType { get; set; }
            public string uId { get; set; }
        }

        public class txnReflection
        {
            public string userId { get; set; }
            public double? amount { get; set; }
            public string partnerTranId { get; set; }
            public string partnerRefundId { get; set; }
            public string tranType { get; set; }
            public string tranSubType { get; set; }
            public string ip { get; set; }
            public string id { get; set; }
            public string aggId { get; set; }
            public string walletTranId { get; set; }
            public string walletRefundId { get; set; }
            public string tranStatus { get; set; }
            public string createdBy { get; set; }
            public string creationDate { get; set; }
            public string respCode { get; set; }
            public string respMsg { get; set; }
            public string finalBal { get; set; }
            public string agent { get; set; }
            public string beneficiaryId { get; set; }
        }

        public class result
        {
            public dynamic cardList { get; set; }
            public dynamic cardStatusList { get; set; }
            public dynamic expiryDateList { get; set; }
            public dynamic kitList { get; set; }
            public dynamic cardTypeList { get; set; }
            public dynamic networkTypeList { get; set; }
        }

        public class beneficiaryInfo
        {
            public string userId { get; set; }
            public string mobileNo { get; set; }
            public string beneficiaryName { get; set; }
            public string identifierType { get; set; }
            public string identifier { get; set; }
            public string maxMonthlyAllowedLimit { get; set; }
        }

        public class beneficiaryList
        {
            public string Id { get; set; }
            public string beneficiaryId { get; set; }
            public string beneficiaryName { get; set; }
            public string beneficiaryType { get; set; }
            public string identifier { get; set; }
            public string maxMonthlyAllowedLimit { get; set; }
        }

        public class cardTransactionReflection
        {
            public string partnerRefId { get; set; }
            public string aggId { get; set; }
            public string userId { get; set; }
            public string safexRefId { get; set; }
            public string switchRefId { get; set; }
            public double amount { get; set; }
            public string safexTxnType { get; set; }
            public string switchMCC { get; set; }
            public string switchMerchId { get; set; }
            public string switchMerchName { get; set; }
            public string switchChannel { get; set; }
            public string kitNo { get; set; }
            public string network { get; set; }
            public string switchTerminalId { get; set; }
            public string switchTraceNo { get; set; }
            public string switchInstitutionCode { get; set; }
            public string cardCurrencyCode { get; set; }
            public string txnCurrencyCode { get; set; }
            public string txnCurrency { get; set; }
            public string posTxnType { get; set; }
            public string respStatus { get; set; }
            public string respMsg { get; set; }
        }

        public class yesPayVirtualCardResponse
        {
            public string code { get; set; }
            public string status_code { get; set; }
            public string card_number { get; set; }
            public string expiry_month { get; set; }
            public string expiry_year { get; set; }
            public string message { get; set; }
            public string walletCode { get; set; }
            public string walletDesc { get; set; }
            public string cvv { get; set; }
        }

        public class smartCardModel
        {
            public string pincode { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string title { get; set; }
            public string gender { get; set; }
            public string userId { get; set; }
            public string idNumber { get; set; }
            public string address { get; set; }
            public string address2 { get; set; }
            public idProofImages[] IdProofImages { get; set; }
            public string reqId { get; set; }
            public string changePinMode { get; set; }
            public string preferredName { get; set; }
            public string cardType { get; set; }
            public string dateofBirth { get; set; }
            public string proxyNumber { get; set; }
            public string flag { get; set; }
            public string kitNo { get; set; }
            public string type { get; set; }
            public string prePaidCardNumber { get; set; }
            public string cvv { get; set; }
            public string expiry { get; set; }
            public string reason { get; set; }
        }

        public class walletMastBean
        {
            public string mobileno { get; set; }
            public string emailid { get; set; }
            public string name { get; set; }
            public string lastName { get; set; }
            public int usertype { get; set; }
            public int loginStatus { get; set; }
            public string docType { get; set; }
            public string docNo { get; set; }
            public string id { get; set; }
            public string userId { get; set; }
            public double? finalBalance { get; set; }
            public double? bankBalance { get; set; }
        }

        public class kycMastBean
        {
            public string UserId { get; set; }
            //public string userName { get; set; }
            public string city { get; set; }
            public string pincode { get; set; }
            public string addprofdesc { get; set; }
            public string idprofdesc { get; set; }
            public string title { get; set; }
            public string idNumber { get; set; }
            public string gender { get; set; }
            public string address { get; set; }
            public string address2 { get; set; }
            public string state { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string nationality { get; set; }
            public idProofImages[] idProofImages { get; set; }
        }

        public class mmResponse
        {
            public transactions[] transactions { get; set; }
        }

        public class transactions
        {
            public string amount { get; set; }
            public string type { get; set; }
            public string indicator { get; set; }
            public string date { get; set; }
        }
    }
}