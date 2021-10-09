using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Corno.Services.Http.Interfaces;
using Newtonsoft.Json;

namespace Corno.Services.Http
{
    public class HttpService : IHttpService
    {
        #region -- Methods --

        public virtual string Get(string uri)
        {
            try
            {
                //var webClient = new System.Net.WebClient();Hide admin menu items based on permissions
                //webClient.DownloadString(URI);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var req = (HttpWebRequest) WebRequest.Create(uri);
                req.UserAgent = ".NET Framework Client";
                req.ContentType = "application/x-www-form-urlencoded";
                var resp = (HttpWebResponse) req.GetResponse();
                var stream = resp.GetResponseStream();
                var httpWebResponse = resp as HttpWebResponse;
                if (stream == null) return null;

                var sr = new System.IO.StreamReader(stream);
                return sr.ReadToEnd().Trim();
            }
            catch (WebException exception)
            {
                var httpWebResponse = exception.Response as HttpWebResponse;
                if (httpWebResponse == null) return null;
                switch (httpWebResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return httpWebResponse.StatusCode.ToString();
                    //case HttpStatusCode.NotFound:
                    //    return "404:URL not found :" + uri;
                    //case HttpStatusCode.BadRequest:
                    //    return "400:Bad Request";
                    default:
                        throw;
                }
            }
        }

        public object JsonPost(string requestData, string url)
        {
            object jsonObject;
            try
            {
                var data = Encoding.UTF8.GetBytes(requestData);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip");

                var dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                var webResponse = request.GetResponse();
                var stream = webResponse.GetResponseStream();
                if (stream == null) return string.Empty;

                using (var streamReader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress)))
                {
                    var responseText = streamReader.ReadToEnd();
                    jsonObject = JsonConvert.DeserializeObject(responseText);
                }
            }
            catch (WebException webEx)
            {
                //get the response stream
                var response = webEx.Response;
                var stream = response.GetResponseStream();
                jsonObject = stream == null ? string.Empty : new StreamReader(stream).ReadToEnd();
            }

            return jsonObject;
        }
        #endregion
    }
}