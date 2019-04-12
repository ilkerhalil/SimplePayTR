using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network.NestPayImpl
{
    internal class NestPayGateway : BaseGateway
    {
        private readonly IHttpClient _httpClient;

        public NestPayGateway(IHttpClient httpClient)
        : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public override Result Pay(BaseRequest request)
        {
            var nestPost = NestPostImpl.NestPost.CreatePost(request);
            nestPost.ConfigName = "EST_Pay.xml";
            var response = _httpClient.Post(nestPost);
            return Handler(response);
        }

        protected override Result Handler(IHttpClientResponse serverResponse)
        {
            var result = new Result { ResultContent = serverResponse.Content };


            var hostResponse = ModelHelper.GetInlineContent(result.ResultContent, "Response");
            if (hostResponse == "Approved")
            {
                result.Status = true;
                result.ProvisionNumber = ModelHelper.GetInlineContent(result.ResultContent, "AuthCode");
                result.ReferenceNumber = ModelHelper.GetInlineContent(result.ResultContent, "HostRefNum");
                result.ProcessId = ModelHelper.GetInlineContent(result.ResultContent, "OrderId");
            }
            else
            {
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "ErrMsg");
                result.ErrorCode = ModelHelper.GetInlineContent(result.ResultContent, "ERRORCODE");
            }

            return result;
        }

        public override Result Refund(BaseRequest request)
        {
            var nestPost = NestPostImpl.NestPost.CreatePost(request);
            nestPost.ConfigName = "EST_Pay.xml";
            var response = _httpClient.Post(nestPost);
            return Handler(response);
        }

        public override List<NetworkConfigurationModel> GetNetworkConfiguration()
        {
            var model = new List<NetworkConfigurationModel>
            {
                new NetworkConfigurationModel
                {
                    Key = "ClientId", Value = "", Description = "Mağaza No", NetworkType = NetworkType.NestPay
                },
                new NetworkConfigurationModel
                {
                    Key = "Name", Value = "", Description = "Kullanıcı Adı", NetworkType = NetworkType.NestPay
                },
                new NetworkConfigurationModel
                {
                    Key = "Password", Value = "", Description = "Şifre", NetworkType = NetworkType.NestPay
                },
                new NetworkConfigurationModel
                {
                    Key = "StoreKey", Value = "", Description = "3D Şifresi", NetworkType = NetworkType.NestPay
                },
                new NetworkConfigurationModel
                {
                    Key = "Method",
                    Value = "cc5ApiServer",
                    Description = "Post Edilecek Metod",
                    NetworkType = NetworkType.NestPay
                }
            };
            return model;
        }

        public override Result Pay3D(BaseRequest request, System.Collections.Specialized.NameValueCollection collection)
        {
            var result = new Result(false, "İmza Doğrulanamadı!");
            if (!Check3D(collection, request.Accounts))
            {
                return result;
            }

            request.Accounts.Add("ProcessId", collection.Get("oid"));
            request.Accounts.Add("PayerTxnId", collection.Get("xid"));
            request.Accounts.Add("PayerSecurityLevel", collection.Get("eci"));
            request.Accounts.Add("PayerAuthenticationCode", collection.Get("cavv"));
            request.Accounts.Add("CardNumber", collection.Get("md"));

            var nestPost = NestPostImpl.NestPost.CreatePost(request);
            nestPost.ConfigName = "EST_Pay_3DEnd.xml";
            var response = _httpClient.Post(nestPost);
            return Handler(response);
        }

        public override bool Check3D(System.Collections.Specialized.NameValueCollection formCollection, Dictionary<string, object> accounts)
        {
            var hashparams = formCollection.Get("HASHPARAMS");
            var hashparamsval = formCollection.Get("HASHPARAMSVAL");

            string paramsval;
            paramsval = "";
            var index1 = 0;

            do
            {
                var index2 = hashparams.IndexOf(":", index1);
                var val = formCollection.Get(hashparams.Substring(index1, index2 - index1)) == null ? "" : formCollection.Get(hashparams.Substring(index1, index2 - index1));
                paramsval += val;
                index1 = index2 + 1;
            }
            while (index1 < hashparams.Length);

            string hashval;
            hashval = paramsval + accounts["StoreKey"];
            var hashparam = formCollection.Get("HASH");

            SHA1 sha = new SHA1CryptoServiceProvider();
            var hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
            var inputbytes = sha.ComputeHash(hashbytes);

            var hash = Convert.ToBase64String(inputbytes);

            if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam))
            {
                return false;
            }

            var mdstatus = formCollection.Get("mdStatus");
            var result = (mdstatus.Equals("1") || mdstatus.Equals("2") || mdstatus.Equals("3") || mdstatus.Equals("4"));
            return result;
        }
    }
}