using System;
using System.Collections.Generic;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network
{
    public class GarantiBankGateway : IGateway
    {
        private readonly IHttpClient _httpClient;

        public GarantiBankGateway(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Result Pay(BaseRequest request)
        {
            var prepare = new Post
            {
                Request = request,
                ConfigName = !request.Is3D ? "GB_Pay.xml" : "GB_Pay_3D.cshtml",
                ContentType = "text/xml",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = ""
            };

            var response= _httpClient.Post(prepare);
            return Handler(response);
        }

        public Result Refund(BaseRequest request)
        {
            var prepare = new Post
            {
                Request = request,
                ConfigName = "GB_Refound.xml",
                ContentType = "text/xml",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = ""
            };

            var response= _httpClient.Post(prepare);
            return Handler(response);
        }

        private Result Handler(IHttpClientResponse serverResponse)
        {
            var result = new Result {ResultContent = serverResponse.Content};


            var hostResponse = ModelHelper.GetInlineContent(result.ResultContent, "Message");
            if (hostResponse == "Approved")
            {
                result.Status = true;
                result.ProvisionNumber = ModelHelper.GetInlineContent(result.ResultContent, "AuthCode");
                result.ReferenceNumber = ModelHelper.GetInlineContent(result.ResultContent, "RetrefNum");
                result.ProcessId = ModelHelper.GetInlineContent(result.ResultContent, "OrderID");
            }
            else
            {
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "ReasonMessage");
                if (string.IsNullOrEmpty(result.Error))
                {
                    result.Error = ModelHelper.GetInlineContent(result.ResultContent, "ErrorMsg");
                }
                result.ErrorCode = ModelHelper.GetInlineContent(result.ResultContent, "ReasonCode");
            }

            return result;
        }

        public List<NetworkConfigurationModel> GetNetworkConfiguration()
        {
            var model = new List<NetworkConfigurationModel>
            {
                new NetworkConfigurationModel
                {
                    Key = "MerchantId", Value = "", Description = "Mağaza No", NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "Id", Value = "", Description = "Terminal Id", NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "UserName",
                    Value = "",
                    Description = "İade Kullanıcı(PROVRFN) Id",
                    NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "Password", Value = "", Description = "Şifre", NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "PROVSalesId", Value = "", Description = "Satış Kullanıcı", NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "PROVPassword",
                    Value = "",
                    Description = "İade Kullanıcı Şifre(PROVRFN)",
                    NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "TerminalId",
                    Value = "",
                    Description = "3D - 8 Haneli Terminal No",
                    NetworkType = NetworkType.Garanti
                },
                new NetworkConfigurationModel
                {
                    Key = "StoreKey", Value = "", Description = "3D - Şifre", NetworkType = NetworkType.Garanti
                }
            };







            return model;
        }

        public Result Pay3D(BaseRequest request, System.Collections.Specialized.NameValueCollection collection)
        {
            var result = new Result(false, "") {Error = "XXX000", ErrorCode = "XXX000"};

            if (true)
            {
                var responseHashparams = collection.Get("hashparams");
                var responseHash = collection.Get("hash");
                var storekey = request.Accounts["StoreKey"].ToString();

                if (responseHashparams != null && !"".Equals(responseHashparams))
                {
                    var digestData = "";
                    char[] separator = { ':' };
                    var paramList = responseHashparams.Split(separator);

                    foreach (var param in paramList)
                    {
                        digestData += collection.Get(param) == null ? "" : collection.Get(param);
                    }

                    digestData += storekey;

                    System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                    var hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(digestData);
                    var inputbytes = sha.ComputeHash(hashbytes);
                    var hashCalculated = Convert.ToBase64String(inputbytes);

                    bool isValidHash;
                    if (responseHash.Equals(hashCalculated))
                    {
                        isValidHash = true;
                    }
                    else
                    {
                        isValidHash = false;
                        result = new Result(false, "") {Error = "0", ErrorCode = ""};
                    }

                    if (isValidHash)
                    {
                        result = new Result(true, "")
                        {
                            ReferenceNumber = collection.Get("hostrefnum"),
                            ProvisionNumber = collection.Get("authcode")
                        };
                    }
                }
                else
                {
                    result = new Result(false, "") {Error = "0", ErrorCode = ""};
                }
            }

            return result;
        }

        public bool Check3D(System.Collections.Specialized.NameValueCollection formCollection, Dictionary<string, object> accounts)
        {
            var strMdStatus = formCollection.Get("mdstatus");
            return (strMdStatus == "1" | strMdStatus == "2" | strMdStatus == "3" | strMdStatus == "4");
        }
    }
}