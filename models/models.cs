using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mpesa.Models
{
  
        public class MpesaResponse
        {
            public string MerchantRequestID { get; set; }
            public string CheckoutRequestID { get; set; }
            public string ResponseCode { get; set; }
            public string ResultDesc { get; set; }
            public string ResponseDescription { get; set; }
            public string ResultCode { get; set; }
            public string message { get; set; }
        }
        public class MpesaRequest
        {
            public string BusinessShortCode { get; set; }
            public string Password { get; set; }
            public string Timestamp { get; set; }
            public string TransactionType { get; set; }
            public string Amount { get; set; }
            public string PartyA { get; set; }
            public string PartyB { get; set; }
            public string PhoneNumber { get; set; }
            public string CallBackURL { get; set; }
            public string AccountReference { get; set; }
            public string TransactionDesc { get; set; }
            public string PostUrl { get; set; }
			public string BusinessShortCode { get; set; }
            public string Amount { get; set; }

        }

    }

