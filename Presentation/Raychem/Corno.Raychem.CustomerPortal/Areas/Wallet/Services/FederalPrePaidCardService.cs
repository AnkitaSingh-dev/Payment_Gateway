using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Encryption;
using Corno.Services.Encryption.Interfaces;
using Newtonsoft.Json;
using NuGet.Modules;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class FederalPrePaidCardService : WalletBaseService, IFederalPrePaidCardService
    {
        #region -- Constructors --
        public FederalPrePaidCardService(IJWTService JWTService, ILogService logService,
            IWalletBaseService walletBaseService, IdentityManager identityManager, IJugadUserSafexUserService safexServices) : base(identityManager)
        {
            _identityManager = identityManager;
            _logService = logService;
            _JWTService = JWTService;
            _walletBaseService = walletBaseService;
            _safexServices = safexServices;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IJWTService _JWTService;
        private readonly ILogService _logService;
        private readonly IWalletBaseService _walletBaseService;
        private readonly IJugadUserSafexUserService _safexServices;
        #endregion

        #region -- Methods --


        public string GetToken(string request)
        {
            return _JWTService.GenerateToken(request);
        }

        public CardResponse DecryptResponse(string responsePayload, string encyType)
        {
            if (encyType == "AES")
            {
                var aesServices = (IMyCryptoClass)Bootstrapper.GetService(typeof(MyCryptoClass));
                responsePayload = aesServices.decrypt(responsePayload);
                return JsonConvert.DeserializeObject<CardResponse>((responsePayload),
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore
                   }); ;
            }
            else
                return JsonConvert.DeserializeObject<CardResponse>(@_JWTService.DecryptToken(responsePayload));
        }

        public ResponseModel UserRegistration(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);
                Logger.LogHandler.LogInfo("User Validation Successfull", Logger.LogHandler.LogType.Notify);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid()
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeOnboarding,
                        requestSubType = PrePaidConstants.SubTypeRegister,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    walletMastBean = new walletMastBean
                    {
                        mobileno = user.UserName,
                        emailid = user.Email,
                        name = user.FirstName,
                        lastName = user.LastName,
                        usertype = PrePaidConstants.TypeCustomer,
                        loginStatus = PrePaidConstants.LoginStatus,
                        docType = PrePaidConstants.Aadhaar,
                        docNo = viewModel.AadhaarNo
                    }

                };
                Logger.LogHandler.LogInfo(new JavaScriptSerializer().Serialize(requestObject), Logger.LogHandler.LogType.Notify);
                var response = ClientCallWithJWT(requestObject);
                Logger.LogHandler.LogInfo(new JavaScriptSerializer().Serialize(response), Logger.LogHandler.LogType.Notify);
                var responseModel = DecryptResponse(response.payload, response.encType);
                Logger.LogHandler.LogInfo(new JavaScriptSerializer().Serialize(responseModel), Logger.LogHandler.LogType.Notify);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                Logger.LogHandler.LogInfo(status, Logger.LogHandler.LogType.Notify);
                responseCode = responseModel.response.code;
                Logger.LogHandler.LogInfo(responseCode, Logger.LogHandler.LogType.Notify);

                result = new JavaScriptSerializer().Serialize(responseModel);
                Logger.LogHandler.LogInfo(result, Logger.LogHandler.LogType.Notify);

                if (responseCode != "UR000" && responseCode !="UR002" && responseCode != "UR007")
                {
                    bStatus = false;
                    throw new Exception(status);
                }

                if (responseCode == "UR000")
                {
                    Logger.LogHandler.LogInfo("2", Logger.LogHandler.LogType.Notify);
                    //Update Customer Id
                    user.CustId = responseModel.walletMastBean.id;
                    user.CardAmount = 0;

                    _identityManager.UpdateUser(user);
                    Logger.LogHandler.LogInfo("5 " + user.CustId, Logger.LogHandler.LogType.Notify);
                }

                Logger.LogHandler.LogInfo("6", Logger.LogHandler.LogType.Notify);
                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active);
                Logger.LogHandler.LogInfo(new JavaScriptSerializer().Serialize(exist), Logger.LogHandler.LogType.Notify);
                if (exist.Count() == 0)
                {
                    Logger.LogHandler.LogInfo("new record", Logger.LogHandler.LogType.Notify);
                    var newUser = new JugadUserSafexUserRT
                    {
                        UserName = user.UserName,
                        CustId = user.CustId,
                        Status = StatusConstants.Active,
                        PStatus = StatusConstants.Freezed
                    };
                    _safexServices.Add(newUser);
                    Logger.LogHandler.LogInfo(user.UserName + " " + user.CustId + StatusConstants.Active + " "+ StatusConstants.Freezed, Logger.LogHandler.LogType.Notify);
                    Logger.LogHandler.LogInfo("Save request", Logger.LogHandler.LogType.Notify);
                    _safexServices.Save();
                    Logger.LogHandler.LogInfo("Save Successfull", Logger.LogHandler.LogType.Notify);
                }
                Logger.LogHandler.LogInfo("Complete", Logger.LogHandler.LogType.Notify);
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode
            };
        }

        public ResponseModel SCRegistration(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var isCardUser = false;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid()
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypePPC,
                        requestSubType = PrePaidConstants.SubTypeCardReg,
                        tranCode = 0,
                        txnAmt = 0.0,
                        token = GenerateToken(user.CustId)
                    },
                    smartCardModel = new smartCardModel
                    {
                        userId = user.CustId,
                        preferredName = viewModel.FirstName,
                        cardType = viewModel.CardType,
                        kitNo = viewModel.kitNo,
                        gender = viewModel.Gender,
                        type = "Instant",
                        dateofBirth = viewModel.DOB
                    }
                };

                var response = ClientCallWithJWT(requestObject, user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "SC0143" && responseCode != "SC0150" && responseCode != "0000")
                {
                    bStatus = false;
                    throw new Exception(status);
                }

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                if (exist != null)
                {
                    exist.IsCardUser = true;
                    exist.VCardId = viewModel.CardType == "VC" ? responseModel.smartCardModel.reqId : exist.VCardId;
                    exist.PCardId = viewModel.CardType == "PC" ? responseModel.smartCardModel.reqId : exist.PCardId;
                    exist.PKitNo = viewModel.CardType == "PC" ? viewModel.kitNo : exist.PKitNo;
                    exist.VStatus = viewModel.CardType == "VC" ? StatusConstants.Unlock : exist.VStatus;
                    exist.PStatus = viewModel.CardType == "PC" ? StatusConstants.Unlock : exist.PStatus;
                    _safexServices.Update(exist);
                    _safexServices.Save();
                }

                if(viewModel.CardType == "PC") {
                    var prePaidKit = new PrePaidCardKitNo
                    {
                        KitNo = viewModel.kitNo,
                        Username = user.UserName
                    };
                    var _prepaidKitServices = (IPrePaidCardKitService)Bootstrapper.GetService(typeof(PrePaidCardKitService));
                    _prepaidKitServices.Add(prePaidKit);
                    _prepaidKitServices.Save();
                }
                isCardUser = exist.IsCardUser;
            }

            catch (Exception excception)
            {
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                IsCardUser = isCardUser
            };
        }

        public ResponseModel UserCardData(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = "0000";
            var cardDetails = new CardDetails();
            var isVCardBlock = false;
            var cardStatus = string.Empty;
            var kycStatus = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                cardStatus = exist.VStatus;

                if (viewModel.CardType == "PC")
                {
                    cardStatus = exist.PStatus;
                    kycStatus = string.IsNullOrEmpty(user.KYCStatus) ? PrePaidConstants.SubmitKYC : user.KYCStatus;
                }
                else if (viewModel.CardType == "VC" && cardStatus == StatusConstants.Unlock)
                {
                    // create request model
                    var requestObject = new CardRequest
                    {
                        header = new header
                        {
                            operatingSystem = PrePaidConstants.OperatingSystem,
                            sessionId = new Guid(),
                            version = "1.1.1"
                        },
                        transaction = new transaction
                        {
                            requestType = PrePaidConstants.RequestTypePPC,
                            requestSubType = PrePaidConstants.SubTypeUCD,
                            channel = PrePaidConstants.OperatingSystem,
                            id = viewModel.CardType == "VC" ? exist.VCardId : exist.PCardId,
                            token = GenerateToken(user.CustId)
                        }
                    };

                    var response = ClientCallWithJWT(requestObject, user.CustId);

                    var responseModel = DecryptResponse(response.payload, response.encType);

                    _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                    status = responseModel.response.description;
                    responseCode = responseModel.response.code;

                    result = new JavaScriptSerializer().Serialize(responseModel);

                    if (responseCode != "0000")
                    {
                        throw new Exception(responseModel.transaction.responseMsg);
                    }

                    cardDetails = new CardDetails()
                    {
                        Name = user.FirstName + " " + user.LastName,
                        CardNumber = DecryptAES(responseModel.smartCardModel.prePaidCardNumber),
                        Expiry_Date = DecryptAES(responseModel.smartCardModel.expiry),
                        CVV = DecryptAES(responseModel.smartCardModel.cvv)
                    };        
                }
                status = "Card is Blocked";
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                CardDetails = cardDetails,
                CardStatus = cardStatus,
                KYCStatus = kycStatus
            };
        }

        public ResponseModel CardLUB(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var cardStatus = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypePPC,
                        requestSubType = PrePaidConstants.SubTypeLockUnlockBlock,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    smartCardModel= new smartCardModel
                    {
                        reqId = viewModel.CardType == "VC" ? exist.VCardId : exist.PCardId,
                        flag = viewModel.Type,
                        reason = viewModel.RequestFor
                    }
                };

                var response = ClientCallWithJWT(requestObject,user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "SC0146" && responseCode != "SC0147" && responseCode != "SC0148")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                exist.VStatus = viewModel.CardType == "VC" ? viewModel.Type : exist.VStatus;
                exist.PStatus = viewModel.CardType == "PC" ? viewModel.Type : exist.PStatus;
                exist.IsVCardBlock = viewModel.Type == StatusConstants.Block ? true : false;
                _safexServices.Update(exist);
                _safexServices.Save();

                cardStatus = viewModel.CardType == "VC" ? exist.VStatus : exist.PStatus;
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                CardStatus = cardStatus
            };
        }

        public ResponseModel ResetPin(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                
                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid(),
                        version = "1.0.0"
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypePPC,
                        requestSubType = PrePaidConstants.SubTypeSetPin
                    },
                    smartCardModel = new smartCardModel
                    {
                        reqId = viewModel.CardType == "VC" ? exist.VCardId : exist.PCardId,
                        changePinMode = viewModel.Pin
                    }
                };

                var response = ClientCallWithJWT(requestObject,user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "0000")
                {
                    throw new Exception(status);
                }
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode
            };
        }

        public ResponseModel GetWalletBalance(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            double? walletBalance = 0.0;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid()
                    },
                    //userInfo = new userInfo
                    //{
                    //    name = user.FirstName + " " + user.LastName,
                    //    mobileNo = user.UserName,
                    //    emailId = user.Email,
                    //    aadharNo = viewModel.AadhaarNo,
                    //    // type = PrePaidConstants.TypeCustomer,
                    //    aggregatorId = PrePaidConstants.AggregatorId,
                    //    createdBy = PrePaidConstants.AggregatorId
                    //},
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeOnboarding,
                        requestSubType = PrePaidConstants.SubTypeGWB,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    walletMastBean = new walletMastBean
                    {
                        usertype = 0,
                        loginStatus = 0,
                        userId = user.UserName
                    }
                };

                var response = ClientCallWithJWT(requestObject,user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "L0019")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                walletBalance = responseModel.walletMastBean.bankBalance == null ? responseModel.walletMastBean.finalBalance : responseModel.walletMastBean.bankBalance;
                _identityManager.UpdateCard(user.Id, walletBalance);
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                CardAmount = walletBalance.ToString()
            };
        }

        public ResponseModel ShowUserProfile(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var otpRefCode = string.Empty;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        name = user.FirstName + " " + user.LastName,
                        mobileNo = user.UserName,
                        emailId = user.Email,
                        aadharNo = viewModel.AadhaarNo,
                       // type = PrePaidConstants.TypeCustomer,
                        aggregatorId = PrePaidConstants.AggregatorId,
                        createdBy = PrePaidConstants.AggregatorId
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeTxn,
                        requestSubType = PrePaidConstants.SubTypeRegister,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;
                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "00" || responseCode != "M02" || responseCode != "1010")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                //Update Customer Id
                user.CustId = responseModel.userInfo.id;
                _identityManager.UpdateUser(user);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active);
                if (exist.Count() == 0)
                {
                    _safexServices.Add(new JugadUserSafexUserRT
                    {
                        UserName = user.UserName,
                        //CustId = user.CustId,
                        Status = StatusConstants.Active
                    });
                    _safexServices.Save();
                }

                otpRefCode = responseModel.transaction.otp_ref_number;
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                Otp_Ref_Number = otpRefCode
            };
        }

        public ResponseModel UserCardList(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid(),
                        userAgent = "WalletIntegration"
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypePPC,
                        requestSubType = PrePaidConstants.SubTypeUCL,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0,
                        userId = user.CustId
                    }
                };

                var response = ClientCallWithJWT(requestObject, user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "0000")
                {
                    throw new Exception(status);
                }
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode
            };
        }

        public ResponseModel ReflectTransaction(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var type = string.Empty;
            var tranType = string.Empty;
            var tranSubType = string.Empty;
            var responseCode = string.Empty;
            var fromExist = false; var toExist = false;

            try
            {
                if (viewModel.Amount <= 0)
                {
                    return new ResponseModel()
                    {
                        Status = true,
                        Result = "Success Transaction"
                    };
                }

                viewModel.UserName = Encrypt(viewModel.UserName);

                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);
                var exist = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                if (exist == null && viewModel.Service != ServiceConstants.BalanceTransfer)
                {
                    return new ResponseModel()
                    {
                        Status = true,
                        Result = "Success Transaction"
                    };
                }

                if (exist != null)
                {
                    if (!exist.IsCardUser)
                    {
                        return new ResponseModel()
                        {
                            Status = true,
                            Result = "Success Transaction"
                        };
                    }
                }

                //if (viewModel.Service == ServiceConstants.Airline || viewModel.Service == ServiceConstants.Bus || viewModel.Service == ServiceConstants.Hotel)
                //{
                //    merchantId = "MC0002";
                //}
                //else if (viewModel.Operator == ServiceConstants.JugadAdmin || viewModel.Operator == ServiceConstants.JugadWallet || viewModel.Service == ServiceConstants.BalanceTransfer)
                //{
                //    merchantId = "MC0004";
                //}
                //else
                //{
                //    merchantId = "MC0001";
                //}

                switch (viewModel.Service)
                {
                    case ServiceConstants.Credit:
                        tranType = "A";
                        tranSubType = "A";
                        break;
                    case ServiceConstants.Refund:
                        tranType = "R";
                        tranSubType = "R";
                        break;
                    case ServiceConstants.BalanceTransfer:
                        //if (exist == null)
                        //{
                        //    return new ResponseModel()
                        //    {
                        //        Status = true,
                        //        Result = "Success Transaction"
                        //    };
                        //}
                        if (viewModel.Operator == ServiceConstants.JugadWallet)
                        {
                            tranType = "A";
                            tranSubType = "A";
                        }
                        else
                        {
                            var from = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                            var to = _safexServices.Get(x => x.UserName == viewModel.Number).FirstOrDefault();

                            if (from != null)
                                fromExist = from.IsCardUser;

                            if (to != null) ;
                            toExist = to.IsCardUser;

                            if (fromExist && toExist)
                            {
                                tranType = "W";
                                tranSubType = "W";
                            }
                            else if (fromExist && !toExist)
                            {
                                throw new Exception("Reciever is not yes bank verified user");
                            }
                            else if (!fromExist && toExist)
                            {
                                tranType = "W";
                                tranSubType = "A";
                                user = _walletBaseService.GetUser(Encrypt(viewModel.Number), _identityManager);
                            }
                            else
                            {
                                return new ResponseModel()
                                {
                                    Status = true,
                                    Result = "Success Transaction"
                                };
                            }
                        }
                        break;
                    default:
                        tranType = "S";
                        tranSubType = "S";
                        break;
                }

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeTxn,
                        requestSubType = PrePaidConstants.SubTypeReflectTxn,
                        token = GenerateToken(user.CustId)
                    },
                    txnReflection = new txnReflection
                    {
                        amount = viewModel.Amount,
                        partnerTranId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId,
                        partnerRefundId = tranType == "R" ? viewModel.OperatorTransId : string.Empty,
                        tranType = tranType,
                        tranSubType = tranSubType,
                        beneficiaryId = (tranType == "W" && tranSubType == "W") ? viewModel.BeneficiaryCode : viewModel.Number
                    }
                };

                var response = ClientCallWithJWT(requestObject, user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                status = responseModel.response.description;

                if (responseModel.response.code != "0000")
                {
                    throw new Exception(status);
                }

                var safexTransaction = new SafexWalletTransaction
                {
                    UserId = responseModel.txnReflection.userId,
                    Amount = responseModel.txnReflection.amount,
                    SwitchRefId = responseModel.txnReflection.partnerTranId,
                    SafexRefId = responseModel.txnReflection.id,
                    WalletTransactionId = responseModel.txnReflection.walletTranId,
                    Status = StatusConstants.Active,
                    UserName = user.UserName
                };

                var _safexWalletTransactionService = (ISafexWalletTransactionService)Bootstrapper.GetService(typeof(SafexWalletTransactionService));
                _safexWalletTransactionService.AddSafexWalletTransaction(safexTransaction);

                viewModel.UserName = user.UserName;
            }
            catch (Exception excception)
            {

                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                ResponseCode = responseCode
            };
        }

        public ResponseModel SubmitFullKYC(RequestModel viewModel)
        {
            var status = "Uploaded Successfully";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            string kycStatus = string.Empty;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);
                Logger.LogHandler.LogInfo("1", Logger.LogHandler.LogType.Notify);
                kycStatus = string.IsNullOrEmpty(user.KYCStatus) ? PrePaidConstants.SubmitKYC : user.KYCStatus;
                Logger.LogHandler.LogInfo("2", Logger.LogHandler.LogType.Notify);
                var aadhaarFront = new idProofImages
                {
                    data = viewModel.doc1,
                    content_type = PrePaidConstants.ImageFormat,
                    filename = user.UserName + "_doc1.jpeg"
                };
                var aadhaarBack = new idProofImages
                {
                    data = viewModel.doc2,
                    content_type = PrePaidConstants.ImageFormat,
                    filename = user.UserName + "_doc2.jpeg"
                };

                Logger.LogHandler.LogInfo("3", Logger.LogHandler.LogType.Notify);
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid(),
                        version = "1.0.0"
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeOnboarding,
                        requestSubType = PrePaidConstants.SubTypeSubmitFullKYC,
                        token = GenerateToken(user.CustId)
                    },
                    kycMastBean = new kycMastBean
                    {
                        UserId = user.CustId,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        city = viewModel.City,
                        pincode = viewModel.Pin,
                        addprofdesc = PrePaidConstants.Aadhaar,
                        idprofdesc = PrePaidConstants.Pan,
                        title = viewModel.Title,
                        idNumber = viewModel.Pan,
                        gender = viewModel.Gender.ToUpper() =="M" ? "male":"female",
                        address = viewModel.Address,
                        address2 = viewModel.Address2,
                        state = viewModel.State,
                        nationality ="indian",
                        idProofImages = new idProofImages[] { aadhaarFront, aadhaarBack } //, pan 
                    }
                };
                Logger.LogHandler.LogInfo("4", Logger.LogHandler.LogType.Notify);
                var response = ClientCallWithJWT(requestObject, user.CustId);
                Logger.LogHandler.LogInfo("5", Logger.LogHandler.LogType.Notify);
                var responseModel = DecryptResponse(response.payload, response.encType);
                Logger.LogHandler.LogInfo("6", Logger.LogHandler.LogType.Notify);
                var serializer = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, serializer.Serialize(requestObject), serializer.Serialize(responseModel));
                Logger.LogHandler.LogInfo("7", Logger.LogHandler.LogType.Notify);
                status = responseModel.response.description;
                responseCode = responseModel.response.code;
                Logger.LogHandler.LogInfo("8", Logger.LogHandler.LogType.Notify);
                result = new JavaScriptSerializer().Serialize(responseModel);
                Logger.LogHandler.LogInfo("9", Logger.LogHandler.LogType.Notify);
                if (responseCode != "SK000")
                {
                    throw new Exception(status);
                }
                Logger.LogHandler.LogInfo("10", Logger.LogHandler.LogType.Notify);
                user.KYCStatus = PrePaidConstants.UnderReview;
                user.CustId = user.CustId;
                _identityManager.UpdateUser(user);
                kycStatus = string.IsNullOrEmpty(user.KYCStatus) ? PrePaidConstants.SubmitKYC : user.KYCStatus;

                var kycDetails = new UserKYCModel
                {
                    UserName = user.UserName,
                    DeviceId = user.DeviceId,
                    Status = StatusConstants.Active
                };

                var _kycService = (IUserKYCService)Bootstrapper.GetService(typeof(UserKYCService));
                _kycService.Add(kycDetails);
                _kycService.Save();

                Logger.LogHandler.LogInfo("11", Logger.LogHandler.LogType.Notify);
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                ResponseCode = responseCode,
                KYCStatus = kycStatus
            };
        }

        public ResponseModel FetchCardTransactions(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var transactionList = new mmResponse();

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem,
                        sessionId = new Guid(),
                        userAgent = "WalletIntegration",
                        version = "1.0.0",
                        date = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss tt"),
                        requestId = ""
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypePPC,
                        requestSubType = PrePaidConstants.subTypeGUWCT,
                        agId = PrePaidConstants.AggregatorId,
                        pageNo = viewModel.Number,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    smartCardModel = new smartCardModel
                    {
                        reqId = viewModel.CardType == "VC" ? exist.VCardId : exist.PCardId
                    }
                };

                var response = ClientCallWithJWT(requestObject, user.CustId);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.response.description;
                responseCode = responseModel.response.code;

                result = new JavaScriptSerializer().Serialize(responseModel);

                if (responseCode != "0000")
                {
                    throw new Exception(status);
                }

                transactionList = responseModel.mmResponse;

                foreach(var transaction in transactionList.transactions)
                {
                    transaction.amount = transaction.amount.Replace(",", string.Empty);
                }
            }

            catch (Exception excception)
            {
                bStatus = false;
                status = excception.Message;
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = result,
                mmResponse = transactionList,
                ResponseCode = responseCode
            };
        }

        public string Encrypt(string number)
        { return _walletBaseService.Encrypt(number, FieldConstants.LoginIv); }

        private cardRequestModel ClientCallWithJWT(CardRequest requestObject, string userId = null)
        {
            var serializer = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
            //Get JWT token
            //var payload = GetToken(new JavaScriptSerializer().Serialize(requestObject));
            //var type = "JWT";

            //   if (requestObject.transaction.requestType == PrePaidConstants.RequestTypeTxn)
            //  {
            var aesServices = (IMyCryptoClass)Bootstrapper.GetService(typeof(MyCryptoClass));
                // payload = aesServices.Encrypt(payload, FieldConstants.CardKey, FieldConstants.CardIV);
                var payload = aesServices.encrypt(serializer.Serialize(requestObject));
                var type = "AES";
                var uId = userId == null ? string.Empty : userId; 
           // }

            var postParameters = serializer.Serialize(new cardRequestModel
            {
                agId = PrePaidConstants.AggregatorId,
                meId = "",
                payload = payload,
                encType = type,
                uId = uId
            });

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(PrePaidConstants.CardURL);
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", postParameters, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, serializer.Serialize(postParameters), serializer.Serialize(response.Content));
            return JsonConvert.DeserializeObject<cardRequestModel>(response.Content);
        }

        private string DecryptAES(string source)
        {
            var aesServices = (IMyCryptoClass)Bootstrapper.GetService(typeof(MyCryptoClass));
            return aesServices.decrypt(source);
        }

        private string GenerateToken(string userId)
        {
            var client = new RestClient("https://wallet.4everpayapi.co.in/agWalletAPI/v2/token");
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n\"uId\": \""+ userId +"\"\r\n} ", ParameterType.RequestBody);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IRestResponse response = client.Execute(request);
            _logService.GenerateLog("Card", PrePaidConstants.CardURL, "Token generation", userId, new JavaScriptSerializer().Serialize(response.Content));
            var responseModel = JsonConvert.DeserializeObject<cardRequestModel>(response.Content);
            var decryptResponse = DecryptResponse(responseModel.payload, "AES");
            _logService.GenerateLog("Card", PrePaidConstants.CardURL, "Token generation", userId, new JavaScriptSerializer().Serialize(decryptResponse));
            return decryptResponse.transaction.token;
        }

        private string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
            }
        }

        #endregion
    }
}