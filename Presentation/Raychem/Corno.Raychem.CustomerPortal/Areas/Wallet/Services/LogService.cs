using System;
using System.IO;
using System.Net.Http;
using System.Web;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Newtonsoft.Json;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class LogService : ILogService
    {
        #region -- Methods --
        //public void GenerateLog(string tboType, string url, string requestName, HttpRequestMessage request, HttpResponseMessage response)
        public void GenerateLog(string service, string url, string requestName, string request, string response, string username = "")
        {
            try
            {
                //dynamic parsedRequest = JsonConvert.DeserializeObject(request.Content.ReadAsStringAsync().Result);
                var actualRequest = request;
                var actualResponse = response;
                try
                {
                    dynamic parsedRequest = JsonConvert.DeserializeObject(request);
                    actualRequest = parsedRequest["request"].ToString();

                    //dynamic parsedResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                    dynamic parsedResponse = JsonConvert.DeserializeObject(response);
                    actualResponse = parsedResponse["response"].ToString();
                }
                catch
                {
                    // Ignore
                }
                

                WriteLog(service, requestName, actualRequest, actualResponse, username);
                //LogHandler.LogInfo("Seperate Log : " + requestName, LogHandler.LogType.General);
                WriteLogInSingleFile(service, url, actualRequest, actualResponse, username);
                //LogHandler.LogInfo("Single File Log : " + requestName, LogHandler.LogType.General);
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }
        }

        private static string GetTodaysFile(string serviceType, string logType)
        {
            var fileName = $"{serviceType}_{logType}_{DateTime.Now:dd-MM-yyyy_hh-mm-ss-tt}.txt";

            var directoryPath = HttpContext.Current.Server.MapPath("~/ServiceLogs/");
            var exists = Directory.Exists(directoryPath);
            if (!exists)
                Directory.CreateDirectory(directoryPath);
            return directoryPath + fileName;
        }

        private static string GetTodaysSingleFile(string serviceType)
        {
            var fileName = $"{DateTime.Now:dd-MM-yyyy}.txt";

            var directoryPath = HttpContext.Current.Server.MapPath("~/ServiceLogs/" + serviceType + "/");
            var exists = Directory.Exists(directoryPath);
            if (!exists)
                Directory.CreateDirectory(directoryPath);
            return directoryPath + fileName;
        }

        private static void WriteLog(string service, string requestName, string request, string response, string username)
        {
            var filePath = GetTodaysFile(service, requestName);

            using (var file = File.Open(filePath, FileMode.OpenOrCreate))
            {
                file.Seek(0, SeekOrigin.End);
                using (var stream = new StreamWriter(file))
                {
                    stream.WriteLine("Initiated by : " + username + "\r\n");
                    stream.WriteLine(request + "\r\n");
                    stream.WriteLine(response + "\r\n");
                }
            }
        }

        private static void WriteLogInSingleFile(string service, string url, string request, string response, string username)
        {
            var filePath = GetTodaysSingleFile(service);

            using (var file = File.Open(filePath, FileMode.OpenOrCreate))
            {
                file.Seek(0, SeekOrigin.End);
                using (var stream = new StreamWriter(file))
                {
                    stream.WriteLine("URL: " + url + "\r\n");
                    stream.WriteLine("Initiated by : " + username + "\r\n");
                    stream.WriteLine(request + "\r\n");
                    stream.WriteLine(response + "\r\n");
                    stream.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
                }
            }
        }
        #endregion
    }
}