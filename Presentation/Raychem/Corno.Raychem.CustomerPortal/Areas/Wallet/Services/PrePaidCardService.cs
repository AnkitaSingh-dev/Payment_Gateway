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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using static Corno.Raychem.CustomerPortal.Areas.Wallet.Models.PrePaidCardModel;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class PrePaidCardService : WalletBaseService, IPrePaidCardService
    {
        #region -- Constructors --
        public PrePaidCardService(IJWTService JWTService, ILogService logService,
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
                return JsonConvert.DeserializeObject<CardResponse>(@_JWTService.DecryptToken(responsePayload),
                   new JsonSerializerSettings
                   {
                       NullValueHandling = NullValueHandling.Ignore
                   }); ;
            }
            else
                return JsonConvert.DeserializeObject<CardResponse>(@_JWTService.DecryptToken(responsePayload));
        }

        public ResponseModel CreateUser(RequestModel viewModel)
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
                       // type = PrePaidConstants.TypeCustomer,
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

                if (responseModel.response.code != "0000")
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
                        CustId = user.CustId,
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

        public ResponseModel MobileOTPVerification(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var token = string.Empty; var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        mobileNo = user.UserName
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeMobileOTP,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        otp = viewModel.OneTimePassword,
                        otp_ref_number = viewModel.OneTimePasswordRefCode
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                result = new JavaScriptSerializer().Serialize(responseModel);
                token = responseModel.transaction.token;
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
                SessionId = token
            };
        }

        public ResponseModel AadhaarRegistration(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var txnId = string.Empty; var otptxnId = string.Empty;
            var responseCode = "E0001";
            var isCardUser = false;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

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
                        requestSubType = PrePaidConstants.SubTypeAadhaarRegx,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p2 = viewModel.FirstName,
                        p3 = viewModel.AadhaarNo,
                        p4 = viewModel.DOB,
                        p5 = viewModel.Gender,
                        p6 = viewModel.Address,
                        p7 = viewModel.Pin,
                        p8= viewModel.SessionId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;

                if (responseCode != "M09")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                if (exist != null)
                {
                    exist.IsCardUser = true;
                    _safexServices.Update(exist);
                }

                isCardUser = exist.IsCardUser;
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                IsCardUser = isCardUser
            };
        }

        public ResponseModel PanRegistration(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var txnId = string.Empty; var otptxnId = string.Empty;
            var responseCode = "E0001";
            var isCardUser = false;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                if (!Regex.Match(viewModel.Pan, @"[A-Z]{5}\d{4}[A-Z]{1}").Success)
                {
                    throw new Exception("Invalid PAN number");
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
                        requestSubType = PrePaidConstants.SubTypePanRegx,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p2 = viewModel.Pan,
                        p3 = user.FirstName + " " + user.LastName,
                        p4 = viewModel.Gender,
                        p5 = viewModel.DOB,
                        p6 = viewModel.Address,
                        p7 = viewModel.Pin,
                        p8 = viewModel.SessionId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;

                if (responseCode != "M09")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                if (exist != null)
                {
                    exist.IsCardUser = true;
                    _safexServices.Update(exist);
                }

                isCardUser = exist.IsCardUser;
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                IsCardUser = isCardUser
            };
        }

        //public ResponseModel VerifyAadhaarOTP(RequestModel viewModel)
        //{
        //    var status = "Transaction Successful.";
        //    var bStatus = true;
        //    var result = string.Empty;
        //    var txnId = string.Empty; var otptxnId = string.Empty;
        //    var responseCode = string.Empty;
        //    var isAadhaar = false;

        //    try
        //    {
        //        // Validate service model
        //        var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

        //        // create request model
        //        var requestObject = new CardRequest
        //        {
        //            header = new header
        //            {
        //                operatingSystem = PrePaidConstants.OperatingSystem
        //            },
        //            userInfo = new userInfo
        //            {
        //                mobileNo = user.UserName,
        //                aadharNo = viewModel.AadhaarNo
        //            },
        //            transaction = new transaction
        //            {
        //                requestType = PrePaidConstants.RequestTypeUser,
        //                requestSubType = PrePaidConstants.SubTypeAadhaarVerify,
        //                channel = PrePaidConstants.OperatingSystem,
        //                countryCode = PrePaidConstants.CountryCode,
        //                otp = viewModel.OneTimePassword,
        //                txnId = viewModel.TransactionId,
        //                otpTxnId = viewModel.OneTimePasswordRefCode
        //            }
        //        };

        //        var response = ClientCallWithJWT(requestObject);

        //        var responseModel = DecryptResponse(response.payload, response.encType);

        //        _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

        //        status = responseModel.transaction.responseMsg;
        //        responseCode = responseModel.transaction.responseCode;

        //        if (responseModel.response.code != "0000")
        //        {
        //            throw new Exception(responseModel.transaction.responseMsg);
        //        }

        //        viewModel.Service = ServiceConstants.Credit;
        //        viewModel.Amount = (double)user.Wallet;
        //        viewModel.CyberPlatTransId = "JW" + Guid.NewGuid().ToString();
        //        viewModel.UserName = user.UserName;

        //        var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
        //        if (exist != null)
        //        {
        //            exist.IsCardUser = true;
        //            _safexServices.Update(exist);
        //        }

        //        var reflectWallet = ReflectTransaction(viewModel);
        //        if (!reflectWallet.Status)
        //        {
        //            responseCode = reflectWallet.ResponseCode;
        //            throw new Exception(reflectWallet.Result);
        //        }
        //        //if (user.IsAdhaarSubmit)
        //        //{
        //        //    var KYCResponse = SubmitFullKYCRequest(viewModel);
        //        //    isAadhaar = KYCResponse.Status == false ? throw new Exception(KYCResponse.Result) : true;
        //        //}
        //        result = new JavaScriptSerializer().Serialize(responseModel);
        //    }
        //    catch (Exception excception)
        //    {
        //        bStatus = false;
        //        status = excception.Message;
        //    }

        //    return new ResponseModel()
        //    {
        //        Status = bStatus,
        //        Result = status,
        //        ResponseCode = responseCode,
        //        AddionalInfo = result,
        //        IsAdhaarSubmit = isAadhaar
        //    };
        //}

        public ResponseModel FetchCardUserProfile(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            double? cardAmount = 0.0;

            try
            {
                // Validate service model
                //var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        mobileNo = viewModel.UserName
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeProfileFetch,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        agId = PrePaidConstants.AggregatorId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                cardAmount = responseModel.rbiResponse.balance;
                status = responseModel.transaction.responseMsg;
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                CardAmount = cardAmount.ToString()
            };
        }

        public ResponseModel FetchBankWalletKYCStatus(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
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
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        id = user.CustId
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeBankKYC,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        agId = PrePaidConstants.AggregatorId,
                        tranCode = 0,
                        txnAmt = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.transaction.responseMsg;
                result = responseModel.ToString();
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

        public ResponseModel BlockUnblockCloseCardWallet(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
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
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        mobileNo = user.UserName,
                        id = user.CustId
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeBlockUnblock,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        agId = PrePaidConstants.AggregatorId,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        action_name = viewModel.Type == "1" ? "Block" : viewModel.Type == "2" ? "UnBlock" : "Close"
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.transaction.responseMsg;
                result = responseModel.ToString();
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

        public ResponseModel SubmitFullKYCRequest(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var isAadhaar = false;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);
                var aadhaarFront = new idProofImages();
                var aadhaarBack = new idProofImages();

                if (user.IsAdhaarSubmit)
                {

                    isAadhaar = true;

                    string subPath = WebConfigurationManager.AppSettings["KYCCardPath"] + "\\" + user.UserName + "\\" + user.UserName + "_AdharFront.jpg"; // your code goes here
                    bool exists = File.Exists(subPath); //System.IO.Directory.Exists(subPath);
                    if (!exists)
                    {
                        isAadhaar = false;
                        throw new Exception("Aadhaar Image doesnt exist in system");
                    }
                    else
                    {
                        Image myImg = Image.FromFile(subPath);
                        aadhaarFront.data = ImageToBase64(myImg, ImageFormat.Jpeg);
                        aadhaarFront.content_type = PrePaidConstants.ImageFormat;
                        aadhaarFront.filename = user.UserName + "_AdharFront.jpg";
                    }

                    subPath = WebConfigurationManager.AppSettings["KYCCardPath"] + "\\" + user.UserName + "\\" + user.UserName + "_AdharBack.jpg";
                    exists = File.Exists(subPath);
                    if (exists)
                    {
                        Image myImg = Image.FromFile(subPath);
                        aadhaarBack.data = ImageToBase64(myImg, ImageFormat.Jpeg);
                        aadhaarBack.content_type = PrePaidConstants.ImageFormat;
                        aadhaarBack.filename = user.UserName + "_AdharBack.jpg";
                    }
                }
                else
                    throw new Exception("Aadhaar not submitted");

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    userInfo = new userInfo
                    {
                        mobileNo = user.UserName,
                        id = user.CustId
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeFullKYC,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        agId = PrePaidConstants.AggregatorId,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p4 = PrePaidConstants.Aadhaar,
                        p5 = PrePaidConstants.Aadhaar,
                        id_proof_image_1 = aadhaarFront,
                        id_proof_image_2 = aadhaarBack,
                        addr_proof_image_1 = aadhaarFront,
                        addr_proof_image_2 = aadhaarBack,
                        kt = "AS"
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.transaction.responseMsg;
                result = responseModel.ToString();
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
                IsAdhaarSubmit = isAadhaar,
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
            var merchantId = string.Empty;
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

                if (viewModel.Service == ServiceConstants.Airline || viewModel.Service == ServiceConstants.Bus || viewModel.Service == ServiceConstants.Hotel)
                {
                    merchantId = "MC0002";
                }
                else if (viewModel.Operator == ServiceConstants.JugadAdmin || viewModel.Operator == ServiceConstants.JugadWallet || viewModel.Service == ServiceConstants.BalanceTransfer)
                {
                    merchantId = "MC0004";
                }
                else
                {
                    merchantId = "MC0001";
                }

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
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    txnReflection = new txnReflection
                    {
                        userId = user.CustId,
                        amount = viewModel.Amount,
                        partnerTranId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString(): viewModel.CyberPlatTransId,
                        partnerRefundId = tranType == "R" ? viewModel.OperatorTransId : string.Empty,
                        tranType = tranType,
                        tranSubType = tranSubType,
                        beneficiaryId = (tranType == "W" && tranSubType == "W") ? viewModel.BeneficiaryCode : merchantId,
                        ip = HttpContext.Current.Request.UserHostAddress,
                        aggId = PrePaidConstants.AggregatorId,
                        agent = "Chrome" //HttpContext.Current.Request.Browser.Browser
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.transaction.responseCode;
                if (responseModel.response.code != "1000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.transaction.responseMsg;
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

        public ResponseModel AddBeneficiary(RequestModel viewModel)
        {
            var status = "Added Beneficiary Successfully.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var safexUser = _safexServices.Get(x => x.CustId == user.CustId).FirstOrDefault();

                if (safexUser == null)
                {
                    responseCode = "E0001";
                    throw new Exception("Not a valid PPI wallet user");
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
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeAddBene,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        countryCode = PrePaidConstants.CountryCode,
                        txnAmt = 0.0
                    },
                    beneficiaryInfo = new beneficiaryInfo
                    {
                        userId = user.CustId,
                        mobileNo = user.UserName,
                        beneficiaryName = viewModel.BeneficiaryName,
                        identifierType = PrePaidConstants.IdentifierType,
                        identifier = viewModel.ToUserName,
                        maxMonthlyAllowedLimit = "0"

                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.transaction.responseCode;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.transaction.responseMsg;
                result = new JavaScriptSerializer().Serialize(responseModel);
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

        public ResponseModel FetchBeneficiary(RequestModel viewModel)
        {
            var status = "Added Beneficiary Successfully.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var beneficiaryList = new List<beneficiaryList>();

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var safexUser = _safexServices.Get(x => x.CustId == user.CustId).FirstOrDefault();

                if (safexUser == null)
                {
                    responseCode = "E0001";
                    throw new Exception("Not a valid PPI wallet user");
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
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeFetchBene,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        countryCode = PrePaidConstants.CountryCode,
                        txnAmt = 0.0
                    },
                    beneficiaryInfo = new beneficiaryInfo
                    {
                        userId = user.CustId,
                        identifierType = PrePaidConstants.IdentifierType,
                        maxMonthlyAllowedLimit = "0"
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                beneficiaryList = responseModel.beneficiaryList;
                status = responseModel.transaction.responseMsg;
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                BeneficiaryList = beneficiaryList
            };
        }

        public ResponseModel UpdateCustomerDetails(UserViewModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
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
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeAadhaarVerify,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        tranCode = 0,
                        txnAmt = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;

                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                result = new JavaScriptSerializer().Serialize(responseModel);
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
                ResponseCode = responseCode,
                AddionalInfo = result
            };
        }

        //public ResponseModel GetCardDetails(RequestModel viewModel)
        //{
        //    var status = "Transaction Successful.";
        //    var bStatus = true;
        //    var result = string.Empty;
        //    var responseCode = "1000";
        //    var cardDetails = new CardDetails();
        //    var cardStatus = string.Empty;

        //    try
        //    {
        //        // Validate service model
        //        var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

        //        var safexUser = viewModel.CardType == "P" ? _safexServices.Get(x => x.PCardId == user.PCardId).FirstOrDefault() :
        //            _safexServices.Get(x => x.VCardId == user.VCardId).FirstOrDefault();

        //        if (safexUser == null)
        //        {
        //            responseCode = "E0001";
        //            throw new Exception("No card issued currently");
        //        }
        //        var kitNo = viewModel.CardType == "P" ? safexUser.PKitNo : safexUser.VKitNo;

        //        var _prePaidCardService = (IPrePaidCardKitService)Bootstrapper.GetService(typeof(PrePaidCardKitService));
        //        cardStatus = _prePaidCardService.Get(x => x.KitNo == kitNo).FirstOrDefault().Status;

        //        if (viewModel.CardType == "V")
        //        { // create request model
        //            var requestObject = new CardRequest
        //            {
        //                header = new header
        //                {
        //                    operatingSystem = PrePaidConstants.OperatingSystem
        //                },
        //                transaction = new transaction
        //                {
        //                    requestType = PrePaidConstants.RequestTypeTxn,
        //                    requestSubType = PrePaidConstants.SubTypeGetDetails,
        //                    channel = PrePaidConstants.OperatingSystem,
        //                    countryCode = PrePaidConstants.CountryCode,
        //                    tranCode = 0,
        //                    txnAmt = 0.0
        //                }
        //            };

        //            var response = ClientCallWithJWT(requestObject);

        //            var responseModel = DecryptResponse(response.payload, response.encType);

        //            _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

        //            status = responseModel.transaction.responseMsg;
        //            responseCode = responseModel.transaction.responseCode;

        //            if (responseModel.response.code != "0000")
        //            {
        //                throw new Exception(responseModel.transaction.responseMsg);
        //            }

        //            result = new JavaScriptSerializer().Serialize(responseModel);
        //        }
        //    }
        //    catch (Exception excception)
        //    {
        //        bStatus = false;
        //        status = excception.Message;
        //    }

        //    return new ResponseModel()
        //    {
        //        Status = bStatus,
        //        Result = status,
        //        ResponseCode = responseCode,
        //        AddionalInfo = result,
        //        CardDetails = cardDetails,
        //        CardStatus = cardStatus
        //    };
        //}

        public ResponseModel UpdateAadhaar(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
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
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeTxn,
                        requestSubType = PrePaidConstants.SubTypeUpdateAadhaar,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        tranCode = 0,
                        txnAmt = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                status = responseModel.transaction.responseMsg;
                responseCode = responseModel.transaction.responseCode;

                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                ResponseCode = responseCode,
                AddionalInfo = result
            };
        }

        private cardRequestModel ClientCallWithJWT(CardRequest requestObject)
        {
            //Get JWT token
            var payload = GetToken(new JavaScriptSerializer().Serialize(requestObject));
            var type = "JWT";

            if (requestObject.transaction.requestType == PrePaidConstants.RequestTypeTxn)
            {
                var aesServices = (IMyCryptoClass)Bootstrapper.GetService(typeof(MyCryptoClass));

                //payload = aesServices.Encrypt(payload, FieldConstants.CardKey, FieldConstants.CardIV);

                payload = aesServices.encrypt(payload);
                type = "AES";
            }

            var postParameters = new JavaScriptSerializer().Serialize(new cardRequestModel
            {
                agId = PrePaidConstants.AggregatorId,
                meId = PrePaidConstants.AggregatorId,
                payload = payload,
                encType = type
            });

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(PrePaidConstants.CardURL);
            var request = new RestRequest(Method.POST);
             
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", postParameters, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<cardRequestModel>(response.Content);
        }

        public ResponseModel IsCardUser(RequestModel viewModel)
        {
            var bStatus = true;
            var responseCode = "1000";
            var status = "Transaction Successful";
            var isCardUser = false;
            try
            {
                isCardUser =  _safexServices.Get(x => x.UserName == viewModel.UserName).FirstOrDefault().IsCardUser;
                if (!isCardUser)
                {
                    responseCode = "1001";
                }
            }

            catch (Exception excception)
            {
                responseCode = "1001";
            }

            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                ResponseCode = responseCode,
                IsCardUser = isCardUser
            };
        }

        public bool CardTransactionStatus(int Id)
        {
            var bStatus = true;
            var responseCode = "1000";

            try
            {
                var safexTxn = (ISafexWalletTransactionService)Bootstrapper.GetService(typeof(SafexWalletTransactionService));

                var safexId = safexTxn.Get(x => x.Id == Id).FirstOrDefault().SafexRefId;

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeTxnStatus,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        tranCode = 0,
                        txnAmt = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;

                if (responseCode != "0000")
                {
                    throw new Exception(responseModel.response.description);
                }


                var txn = safexTxn.Get(x => x.SafexRefId == safexId).FirstOrDefault();
                safexTxn.Add(txn);
                safexTxn.Save();
            }
            catch (Exception excception)
            {
                bStatus = false;
            }

            return bStatus;
        }

        public string Encrypt(string number)
        { return _walletBaseService.Encrypt(number, FieldConstants.LoginIv); }

        public string Decrypt(string number)
        { return _walletBaseService.Decrypt(number, FieldConstants.LoginIv); }

        public ResponseModel AddPhyscicalCard(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var kitNo = string.Empty;
            var isCardUser = false;

            try
            {
                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                var _prePaidCardService = (IPrePaidCardKitService)Bootstrapper.GetService(typeof(PrePaidCardKitService));
                var kit = _prePaidCardService.Get(x => x.IsAllocated == false && x.Username == null).FirstOrDefault();
                kitNo = kit.KitNo;

                // create request model
                var requestObject = new CardRequest
                {
                    header = new header
                    {
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypeAPC,
                        channel = PrePaidConstants.OperatingSystem,
                        countryCode = PrePaidConstants.CountryCode,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p2 = kitNo,
                        p3 = kit.ValidationNumber,
                        p4 = "SafexPay",
                        p5 = "IND",
                        aggId = PrePaidConstants.AggregatorId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.rbiResponse.walletCode;
                if (responseCode != "1000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                kit.Username = user.UserName;
                kit.IsAllocated = true;
                kit.CreatedDate = DateTime.Now;
                kit.CreatedBy = user.UserName;
                kit.Status = PrePaidConstants.Unlock;
                _prePaidCardService.Update(kit);
                _prePaidCardService.Save();

                var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                safexUser.IsCardUser = true;
                safexUser.PKitNo = kitNo;
                _safexServices.Update(safexUser);
                _safexServices.Save();

                isCardUser = true;
                status = responseModel.rbiResponse.message;
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                KitNo = kitNo,
                IsCardUser = isCardUser
            };
        }

        public ResponseModel PhyscialCardLUB(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var statusCode = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault().PKitNo;

                if (safexUser == null)
                {
                    responseCode = "E0001";
                    throw new Exception("No active cards issued currently");
                }

                switch (viewModel.Type)
                {
                    case "1":
                        statusCode = PrePaidConstants.Unlock;
                        break;
                    case "2":
                        statusCode = PrePaidConstants.Lock;
                        break;
                    case "3":
                        statusCode = PrePaidConstants.Block;
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
                        requestType = PrePaidConstants.RequestTypeUser,
                        requestSubType = PrePaidConstants.SubTypePCLUB,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest{
                        p1=user.UserName,
                        p2= safexUser,
                        p3 = statusCode,
                        p4 = "Request from user",
                        aggId = PrePaidConstants.AggregatorId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.rbiResponse.walletCode;
                if (responseCode != "1000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                var kitNo = safexUser;
                var _prePaidCardService = (IPrePaidCardKitService)Bootstrapper.GetService(typeof(PrePaidCardKitService));
                var kit = _prePaidCardService.Get(x => x.KitNo == kitNo).FirstOrDefault();
                kit.Status = statusCode;
                kit.ModifiedDate = DateTime.Now;
                kit.ModifiedBy = user.UserName;
                _prePaidCardService.Update(kit);
                _prePaidCardService.Save();

                status = responseModel.rbiResponse.message;
                result = new JavaScriptSerializer().Serialize(responseModel);
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

        public ResponseModel PhysicalCardSetPin(RequestModel viewModel)
        {
            var status = "Pin Set Successfully.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var kitNo = string.Empty;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault().PKitNo;

                if (safexUser == null)
                {
                    responseCode = "E0001";
                    throw new Exception("No card issued currently");
                }

                // viewModel.Pin = Encrypt(viewModel.Pin);

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
                        requestSubType = PrePaidConstants.SubTypePSP,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p2 = safexUser,
                        aggId = PrePaidConstants.AggregatorId
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.rbiResponse.code;
                if (responseCode != "00")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                status = responseModel.rbiResponse.status_code;
                result = responseModel.rbiResponse.url;
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

        public ResponseModel FetchVirtualCard(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = "1000";
            var cardDetails = new CardDetails();
            var isVCardBlock = false;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager); 
                isVCardBlock = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault().IsVCardBlock;

                if (!isVCardBlock)
                {
                    var requestObject = new CardRequest
                    {
                        header = new header
                        {
                            operatingSystem = PrePaidConstants.OperatingSystem
                        },
                        transaction = new transaction
                        {
                            requestType = PrePaidConstants.RequestTypeTxn,
                            requestSubType = PrePaidConstants.SubTypeFVC,
                            channel = PrePaidConstants.OperatingSystem,
                            countryCode = PrePaidConstants.CountryCode,
                            tranCode = 0,
                            txnAmt = 0.0
                        },
                        rbiRequest = new rbiRequest
                        {
                            p1 = user.UserName,
                            aggId = PrePaidConstants.AggregatorId
                        }
                    };

                    var response = ClientCallWithJWT(requestObject);

                    var responseModel = DecryptResponse(response.payload, response.encType);

                    _logService.GenerateLog("Card", PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                    status = responseModel.yesPayVirtualCardResponse.message;
                    responseCode = responseModel.yesPayVirtualCardResponse.walletCode;

                    if (responseCode != "1000")
                    {
                        throw new Exception(responseModel.transaction.responseMsg);
                    }

                    cardDetails = new CardDetails()
                    {
                        Name = user.FirstName + " " + user.LastName,
                        CardNumber = responseModel.yesPayVirtualCardResponse.card_number,
                        Expiry_Date = responseModel.yesPayVirtualCardResponse.expiry_month + "/" + responseModel.yesPayVirtualCardResponse.expiry_year,
                        CVV = responseModel.yesPayVirtualCardResponse.cvv
                    };

                    result = new JavaScriptSerializer().Serialize(responseModel);
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
                ResponseCode = responseCode,
                AddionalInfo = result,
                CardDetails = cardDetails,
                IsVCardBlock = isVCardBlock
            };
        }

        public ResponseModel BlockVirtualCard(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = "1000";
            var isVCardBlock = false;

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

                var exist = _safexServices.Get(x => x.UserName == user.UserName && x.Status == StatusConstants.Active).FirstOrDefault();

                if (!exist.IsVCardBlock)
                {
                    // create request model
                    var requestObject = new CardRequest
                    {
                        header = new header
                        {
                            operatingSystem = PrePaidConstants.OperatingSystem
                        },
                        transaction = new transaction
                        {
                            requestType = PrePaidConstants.RequestTypeUser,
                            requestSubType = PrePaidConstants.SubTypeBVC,
                            channel = PrePaidConstants.OperatingSystem,
                            tranCode = 0,
                            txnAmt = 0.0
                        },
                        rbiRequest = new rbiRequest
                        {
                            p1 = user.UserName,
                            aggId = PrePaidConstants.AggregatorId
                        }
                    };

                    var response = ClientCallWithJWT(requestObject);

                    var responseModel = DecryptResponse(response.payload, response.encType);

                    _logService.GenerateLog(viewModel.Service, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                    responseCode = responseModel.rbiResponse.walletCode;

                    if (responseCode != "1000")
                    {
                        throw new Exception(responseModel.transaction.responseMsg);
                    }

                    exist.IsVCardBlock = true;
                    _safexServices.Update(exist);
                    _safexServices.Save();
                   
                    status = responseModel.rbiResponse.message;
                    result = new JavaScriptSerializer().Serialize(responseModel);
                }
                else
                {
                    exist.IsVCardBlock = false;
                    _safexServices.Update(exist);
                    _safexServices.Save();
                }

                isVCardBlock = exist.IsVCardBlock;
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
                IsVCardBlock = isVCardBlock
            };
        }

        public ResponseModel GetCheckPartnerBalance()
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;

            try
            {
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
                        requestSubType = PrePaidConstants.SubTypeCPB,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(ServiceConstants.Card, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.transaction.responseCode;
                status = responseModel.transaction.responseMsg;
                if (responseCode != "0")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                result = responseModel.rbiResponse.load_balance.ToString();
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
                AddionalInfo = result
            };
        }

        public ResponseModel ReQuery(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
            var responseCode = string.Empty;
            var txnReflection = new txnReflection();

            try
            {
                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);

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
                        requestSubType = PrePaidConstants.SubTypeTSC,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        id = user.CustId,
                        transaction_id = viewModel.TransactionId,
                        transaction_amount = viewModel.Amount
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(ServiceConstants.Card, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                status = responseModel.response.description;

                if (responseCode != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }

                txnReflection = responseModel.txnReflection; 
                result = new JavaScriptSerializer().Serialize(responseModel);
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
                TxnReflection = txnReflection
            };
        }

        public ResponseModel TransactionHistory(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var result = string.Empty;
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
                        operatingSystem = PrePaidConstants.OperatingSystem
                    },
                    transaction = new transaction
                    {
                        requestType = PrePaidConstants.RequestTypeTxn,
                        requestSubType = PrePaidConstants.SubTypeTH,
                        channel = PrePaidConstants.OperatingSystem,
                        tranCode = 0,
                        txnAmt = 0.0
                    },
                    rbiRequest = new rbiRequest
                    {
                        p1 = user.UserName,
                        p2 = viewModel.Number,
                        transaction_amount = 0.0
                    }
                };

                var response = ClientCallWithJWT(requestObject);

                var responseModel = DecryptResponse(response.payload, response.encType);

                _logService.GenerateLog(ServiceConstants.Card, PrePaidConstants.CardURL, requestObject.transaction.requestType + requestObject.transaction.requestSubType, new JavaScriptSerializer().Serialize(requestObject), new JavaScriptSerializer().Serialize(responseModel));

                responseCode = responseModel.response.code;
                status = responseModel.response.description;
                if (responseModel.response.code != "0000")
                {
                    throw new Exception(responseModel.transaction.responseMsg);
                }
                result = new JavaScriptSerializer().Serialize(responseModel);
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
            };
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