using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class MobileWareApiService : IMobileWareApiService
    {
        #region -- Constructors --

        public MobileWareApiService()
        {
        }
        #endregion

        #region -- Data Members --

        #endregion

        #region -- Methods --
        public PanResponse PANValidateURL(KYCModel requestModel)
        {
            var result = new PanResponse();
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(TboConstants.PanVerify);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Headers.Add("Access-Token", FieldConstants.AccessToken);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {

                        tenantCode = FieldConstants.TenantCode,
                        userName = FieldConstants.PanUserName,
                        pan = requestModel.PanNumber,
                        metaData = new
                        {
                            udc = requestModel.DeviceId,
                            pip = TboConstants.EndUserIp.Replace("\"",""),
                            lot = FieldConstants.P_GConst,
                            lov = FieldConstants.PinCode
                        },
                        dt = new Random().Next(0, 1000000000).ToString("D9")
                });

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = new JavaScriptSerializer().Deserialize<PanResponse>(streamReader.ReadToEnd());
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        public AccountResponse AccountValidateURL(KYCModel req, string hash, ApplicationUser user)
        {
            var result = new AccountResponse();
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(TboConstants.AccountVerify);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", FieldConstants.Autho);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        customerId = user.UserName,
                        udf1 = req.AccountNumber,
                        udf2 = req.IFSC,
                        recipientType = FieldConstants.MerchantId,
                        clientRefId = new Random().Next(0, 1000000000).ToString("D9"),
                        initiatorId = FieldConstants.MerchantId,
                        currency = FieldConstants.Currency,
                        hashKey = hash,
                        channel = "1",
                        agentCode = user.UserName
                    });

                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = new JavaScriptSerializer().Deserialize<AccountResponse>(streamReader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
        }
        #endregion
    }
}