
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class RequestModel
    {
        public string UserName { get; set; }
        public string DeviceId { get; set; }
        public string EndUserIp { get; set; }
        public string Service { get; set; }
        public string PaymentMode { get; set; }
        public string Operator { get; set; }
        public double Amount { get; set; }
        public float? OperatorComissionName { get; set; }

        public string Number { get; set; }

        public string PaymentTransactionId { get; set; }

        public string Pnr { get; set; }
        public string BookingId { get; set; }

        public string Account { get; set; }
        public string Authenticator3 { get; set; }
        public string Authenticator4 { get; set; }        

        public string OneTimePassword { get; set; }
        public string OneTimePasswordRefCode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Pin { get; set; }
        public string Gender { get; set; }
        public string MothersMaidenName { get; set; }
        public string State { get; set; }

        public string RoutingType { get; set; }
        public string TransRefId { get; set; }

        public string BeneficiaryAccount { get; set; }
        public string BeneficiaryMobile { get; set; }
        public string BeneficiaryNickName { get; set; }
        public string BeneficiaryIfsc { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryCode { get; set; }

        public string TermId { get; set; }
        public string Type { get; set; }
        public string AmountAll { get; set; }
        public string Comment { get; set; }

        public string PlanOffer { get; set; }

        public string Request { get; set; }
        public string Response { get; set; }

        // Gift Card
        public string CategoryId { get; set; }
        public string ProductId { get; set; }
        public string BillingEmail { get; set; }
        public string BeneficiaryFirstName { get; set; }
        public string BeneficiaryLastName { get; set; }
        public string Email { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string GiftMessage { get; set; }
        public string Theme { get; set; }
        public string ProductType { get; set; }
        public string AgentTransId { get; set; }

        // Appnit only
        public string Mid { get; set; }
        public string PgTxnId { get; set; }
        public string OrderId { get; set; }

        // Not sent from client. Only filled in the code.
        public string CyberPlatTransId { get; set; }
        public string OperatorTransId { get; set; }
        public double? Commission { get; set; }
        public double? OpeningBalance { get; set; }
        public double? Credit { get; set; }
        public double? Debit { get; set; }
        public double? ClosingBalance { get; set; }

        public string TransactionId { get; set; }
        public string ToUserName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string AppKey { get; set; }
        public string MposUserName { get; set; }

        public string SessionId { get; set; }
        public string Agent { get; set; }
        public string AgentId { get; set; }

        public string BenBankName { get; set; }
        //public string BenMobile { get; set; }
        public string BenId { get; set; }
        public string KeyKycStatus { get; set; }
        public string Rcode { get; set; }
        public string RequestFor { get; set; }

        public string AadhaarNo { get; set; }
        public string Pan { get; set; }
        public string DOB { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public string Expiry { get; set; }

        public string CardId { get; set; }
        public string CardNumber { get; set; }

        public string FCMToken { get; set; }
        public string CardType { get; set; }
        public string kitNo { get; set; }
        public string RemId { get; set; }
        public string doc1 { get; set; }
        public string doc2 { get; set; }       
    }

    public class ResponseModel
    {
        public bool Status { get; set; }
        public string Result { get; set; }
        public double? WalletBalance { get; set; }
        public string TransactionId { get; set; }
        public string AddionalInfo { get; set; }
        public string DataHtml { get; set; }
        public object Response { get; set; }

        public UserViewModel UserInfo { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string SessionId { get; set; }
        public double? ConvinienceCharges { get; set; }
        public string Otp { get; set; }

        public bool IsKYCSubmit { get; set; }
        public bool IsPanSubmit { get; set; }
        public bool IsAdhaarSubmit { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string AgentId { get; set; }

        public string Otp_Ref_Number { get; set; }
        public string AadhaarNo { get; set; }

        public string ResponseCode { get; set; }
        public string KitNo { get; set; }
        public bool IsCardUser { get; set; }
        public CardDetails CardDetails { get; set; }
        public bool IsVCardBlock { get; set; }
        public result CardList { get; set; }
        public string CardAmount { get; set; }
        public List<beneficiaryList> BeneficiaryList { get; set; }

        public string FCMToken { get; set; }
        public ElectricityDetails ElectricityDetails { get; set; }
        public dynamic PrepaidPlans { get; set; }

        public string KYCStatus { get; set; }
        public string CardStatus { get; set; }

        public txnReflection TxnReflection { get; set; }
        public mmResponse mmResponse { get; set; }

        public double? CardBalance { get; set; }
    }

    public class MposResponseModel
    {
        public int Amount { get; set; }
        public int? AmountAdditional { get; set; }
        public int? AmountCashBack { get; set; }
        public int? AmountOriginal { get; set; }
        public string AuthCode { get; set; }
        public string BatchNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerReceiptUrl { get; set; }
        public string DeviceSerial { get; set; }
        public string ExternalRefNumber { get; set; }
        public string FormattedPan { get; set; }
        public string InvoiceNumber { get; set; }
        public string Mid { get; set; }
        public string PayerName { get; set; }
        public string PaymentCardBrand { get; set; }
        public string PaymentCardType { get; set; }
        public string PaymentMode { get; set; }
        public string PgInvoiceNumber { get; set; }
        public int? PostingDate { get; set; }
        public string RrNumber { get; set; }
        public string SettlementStatus { get; set; }
        public string Stan { get; set; }
        public string Status { get; set; }
        public string Tid { get; set; }
        public string TxnId { get; set; }
        public string TxnType { get; set; }
        public string UserAgreement { get; set; }
        public string UserName { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
    }

    public class walletResponse
    {
        public bool Status { get; set; }
        public string Result { get; set; }
        public double WalletBalance { get; set; }
    }
}