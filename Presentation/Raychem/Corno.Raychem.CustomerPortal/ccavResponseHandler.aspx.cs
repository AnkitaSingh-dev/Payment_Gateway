using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Specialized;
using CCA.Util;
using log4net;

public partial class CCAVResponseHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        var logger = LogManager.GetLogger("Kotak_PG");
        //string workingKey = "3557769DAEF31DE81DD079180CF70BE4"; //put in the 32bit alpha numeric key in the quotes provided here
        string workingKey = "69632435AEC5279071255F6A66F52D28"; //Production Key
            CCACrypto ccaCrypto = new CCACrypto();
            logger.Info(Request.Form["encResp"]);
            string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"],workingKey);
            NameValueCollection Params = new NameValueCollection();
            string[] segments = encResponse.Split('&');
            foreach (string seg in segments)
            {
                string[] parts = seg.Split('=');
                if (parts.Length > 0)
                {
                    string Key = parts[0].Trim();
                    string Value = parts[1].Trim();
                    Params.Add(Key, Value);
                }
            }
            
            for (int i = 0; i < Params.Count; i++)
            {
                Response.Write(Params.Keys[i] + " = " + Params[i] + "<br>");
            }
           
         }
    }
