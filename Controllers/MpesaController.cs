using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mpesa.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MpesaSTKPush.Models;
using MpesaSTKPushAPI;

namespace API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MpesaController : Controller
    {
       // private Bl bl;
        public MpesaController()
        {
			instantiate your db connection here
           // bl = new Bl(Util.GetDbConnString());
        }
       //name the methods and models as pefer your project or as you prefer
        [HttpPost("MnauriRequest")]
        public MpesaResponse MnauriRequest(MpesaRequest req)
        {
            MpesaResponse resp = new MpesaResponse();
            try
            {
                if (req == null)
                {
                    return new MpesaResponse
                    {
                        message = "Request cannt be empty!!!!!!!!!!!"
                    };
                }
                string data = "";

                resp = makeRequest(req);
                //string data =  new StreamReader(HttpContext.Request.Body);// Request.Content.ReadAsStringAsync();
                using (StreamReader stream = new StreamReader(HttpContext.Request.Body))
                {
                    data = stream.ReadToEnd();
                    // body = "param=somevalue&param2=someothervalue"
                }
                Util.LogError("MpesaResponse", new Exception(data), false);
                if (!string.IsNullOrEmpty(data))
                {
                    Util.LogError("MpesaResponse", new Exception(data));
                }
                if (resp.ResponseCode == "0")
                {
                    resp.message = "wahala!!!";
                }

                return resp;
            }
            catch (Exception ex)
            {
                Util.LogError("MpesaResponse", ex);
            }
            return resp;
        }

        

        [HttpPost("LipaOnline")]
        public MpesaResponse Mpesa()
        {
            try
            {
                string data = "";
                //string data =  new StreamReader(HttpContext.Request.Body);// Request.Content.ReadAsStringAsync();
                using (StreamReader stream = new StreamReader(HttpContext.Request.Body))
                {
                    data = stream.ReadToEnd();
                    // body = "param=somevalue&param2=someothervalue"
                }
                Util.LogError("MpesaResponse", new Exception(data), false);
                if (!string.IsNullOrEmpty(data))
                {
                    Util.LogError("MpesaResponse", new Exception(data));
                }
                return new MpesaResponse();
            }
            catch (Exception ex)
            {
                Util.LogError("MpesaResponse", ex);
            }
            return new MpesaResponse();
        }

        public MpesaResponse makeRequest(MpesaRequest req)
        {
            //this url,consumer key , secret key should be fetched from a db for easier management 
            string authUrl = "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";
            string consumerkey = "9HUN5NOlioAZdjhEofAyFD0nr4j69xQv";
            string consumersecret = "LK2iIjCN3a4G4a6P";
            String timeStamp = "20181101101948";// format to generate DateTime.Now.ToString("yyyyMMddHHmmss"); 
                                                
            string Password = "";
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(consumerkey + consumersecret);
            byte[] isoBytes = Encoding.Convert(Encoding.UTF8, iso, utfBytes);
            string d = Convert.ToBase64String(isoBytes);
            string authToken =  GetMPesaAuthToken(authUrl,consumerkey,consumersecret);
            if (string.IsNullOrEmpty(authToken))
                return new MpesaResponse
                {
                    ResultCode= "1",
                    message = "Failed to get auth token!"
                };
            //password 
            string pass = req.BusinessShortCode + "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919" + "20181101101948";
            byte[] passutfBytes = utf8.GetBytes(pass);
            byte[] passisoBytes = Encoding.Convert(Encoding.UTF8, iso, passutfBytes);
             Password = Convert.ToBase64String(passisoBytes);

            MpesaResponse resp = new MpesaResponse();

            //----- Post main transaction -- add record to db --ones posted update txn status -- 
            MpesaRequest postModel = new MpesaRequest
            {
                PostUrl = "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest",
                BusinessShortCode = req.BusinessShortCode,
                Password = Password,
                Timestamp = timeStamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = "310",
                PartyA = "254707318620",
                PartyB = req.BusinessShortCode,
                PhoneNumber = "254707318620",
                CallBackURL = "http://8ba878d9.ngrok.io/Mpesa/api/v1/Mpesa/LipaOnline",
                AccountReference = "MN2018",
                TransactionDesc = "this is a test"
            };
            var jsonData = JsonConvert.SerializeObject(postModel);
            //=============================================
            Util.LogError("PostMnauri mpesa request:Data ", new Exception(jsonData), false);
            //===============================================
            var postResp = DoPost(jsonData, authToken, postModel.PostUrl);

            //=============================================
            Util.LogError("PostMnauri mpesa request >> ", new Exception(postResp), false);
            //===============================================


            if (postResp.StartsWith("Error!"))
            {
                return new MpesaResponse
                {
                    ResponseCode = "140",
                    ResponseDescription = postResp
                };
            }
            else
            {
                var respData = JsonConvert.DeserializeObject<MpesaResponse>(postResp);

                return respData;
            }

        }
        //this method gets an auth token from mpesa
        public string GetMPesaAuthToken(string url, string consumerKey, string consumerSecret)
        {
            string authToken = "";

            //---- Create headers
            string pass = consumerKey + ":" + consumerSecret;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(pass);
            byte[] isoBytes = Encoding.Convert(Encoding.UTF8, iso, utfBytes);
            string tkn = Convert.ToBase64String(isoBytes);
            headers.Add("Authorization", "Basic " + tkn);

            HttpClient httpClient = new HttpClient(url, HttpClient.RequestType.Get, headers);

            Exception error;
            var results = httpClient.SendRequest("", out error);
            if (!string.IsNullOrEmpty(results))
            {
                if (results != "Error!")
                {
                    dynamic data = JObject.Parse(results);
                    authToken = data.access_token;
                }
            }

            return authToken;
        }
        //post request to mpesa for processing
        private string DoPost(string jsonData, string authHeader, string url)
        {
            //---- Create headers
            Dictionary<string, string> headers = new Dictionary<string, string>();

            //---- Create token
            headers.Add("Authorization", "Bearer " + authHeader);

            HttpClient httpClient = new HttpClient(url, HttpClient.RequestType.Post, headers);

            Exception ex;
            var results = httpClient.SendRequest(jsonData, out ex);

            if (string.IsNullOrEmpty(results))
                throw ex;

            return results;
        }
    }
}