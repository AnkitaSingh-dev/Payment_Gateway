

using Corno.Services.Encryption.Interfaces;
using Jose;
using System.Collections.Generic;
using System.Text;
using System;

/// <summary>
/// Use the below code to generate symmetric Secret Key
///     var hmac = new HMACSHA256();
///     var key = Convert.ToBase64String(hmac.Key);
/// </summary> 

namespace Corno.Services.Encryption
{
    public class JWTService : IJWTService
    {
        // Define const Key this should be private secret key  stored in some safe place
        //private const string key = "e5e5926f41bdf290fc4104f4261c6f1d8b2fd5274b6b65a63cd996fdd1e94099"; //Production Key
        private const string key = "ddd2b16fc6f44cd72a08ea98cece807c4eec0d242a20061fa91346101ff302dd"; //Test Key

        private byte[] secretKey = Encoding.UTF8.GetBytes(key);

        public string GenerateToken(string request)
        {
            var payload = new Dictionary<string, object>()
                     {
                        { "payload", request },
                     };

            return JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
        }

        public string DecryptToken(string response)
        {
            return JWT.Decode(response, secretKey, JwsAlgorithm.HS256).Replace("\"payload\":\"{","").Replace("}\"}","}").Replace("\\",""); //\"payload\": \"{
        }
    }
}