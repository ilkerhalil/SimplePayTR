using System.Collections.Generic;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network
{
    public class YapıKrediGateway:IGateway
    {
        private readonly IHttpClient _httpClient;

        public YapıKrediGateway(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Result Pay(BaseRequest request)
        {
            Result rs;
            var prepare = new Post
            {
                Request = request,
                ConfigName = !request.Is3D ? "YKB_Pay.xml" : "YKB_Pay_3D.xml",
                ContentType = "application/x-www-form-urlencoded",
                RequestFormat =  HttpClientRequestFormat.Xml,
                PreTag = "xmldata="
            };

            if (request.Is3D)
            {
                request.Is3D = false;

                var response= _httpClient.Post(prepare);
                rs =  Handler3D(response);
                if (rs.Status) {

                    prepare.Request = new BaseRequest
                    {
                        Accounts = request.Accounts,
                        SuccessUrl = request.SuccessUrl,
                        ErrorUrl = request.ErrorUrl,
                        Pos = new RequestPos {
                            FullName = rs.ProvisionNumber,
                            EMail = rs.ReferenceNumber,
                            Ip = rs.Error
                        },
                        Is3D=true
                    };
                    prepare.ConfigName = "YKB_Pay_3D.cshtml";
                    prepare.ContentType = "application/x-www-form-urlencoded";
                    prepare.RequestFormat = HttpClientRequestFormat.Xml;
                    prepare.PreTag = "xmldata=";
                    request.Is3D = true;
                    response= _httpClient.Post(prepare);
                    rs =  Handler3D(response);
                    rs.RequestData = request;
                }
            }
            else {
                var response= _httpClient.Post(prepare);
                rs =  Handler(response);

            }

            return rs;
        }

        public Result Refund(BaseRequest request)
        {
            var prepare = new Post
            {
                Request = request,
                ConfigName = "YKB_Refound.xml",
                ContentType = "text/xml",
                RequestFormat =  HttpClientRequestFormat.Xml,
                PreTag = ""
            };
            var response= _httpClient.Post(prepare);
            return  Handler(response);
        }

        Result Handler(IHttpClientResponse serverResponse)
        {
            var result = new Result();

            result.ResultContent = serverResponse.Content;

            var hostResponse = ModelHelper.GetInlineContent(result.ResultContent, "approved");
            if (hostResponse == "1")
            {
                result.Status = true;
                result.ProvisionNumber = ModelHelper.GetInlineContent(result.ResultContent, "authCode");
                result.ReferenceNumber = ModelHelper.GetInlineContent(result.ResultContent, "hostlogkey");
                result.ProcessId = ModelHelper.GetInlineContent(result.ResultContent, "hostlogkey");
            }
            else
            {
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "respText");
                result.ErrorCode = ModelHelper.GetInlineContent(result.ResultContent, "respCode");
            }

            return result;
        }

        Result Handler3D(IHttpClientResponse serverResponse)
        {
            var result = new Result {ResultContent = serverResponse.Content};


            var hostResponse = ModelHelper.GetInlineContent(result.ResultContent, "approved");
            if (hostResponse == "1")
            {
                result.Status = true;
                result.ProvisionNumber = ModelHelper.GetInlineContent(result.ResultContent, "data1");
                result.ReferenceNumber = ModelHelper.GetInlineContent(result.ResultContent, "data2");
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "sign");
            }
            else
            {
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "respText");
                result.ErrorCode = ModelHelper.GetInlineContent(result.ResultContent, "respCode");
            }

            return result;
        }

        public List<NetworkConfigurationModel> GetNetworkConfiguration()
        {
            var model = new List<NetworkConfigurationModel>
            {
                new NetworkConfigurationModel
                {
                    Key = "MId", Value = "", Description = "Mağaza No", NetworkType = NetworkType.YapiKredi
                },
                new NetworkConfigurationModel
                {
                    Key = "TId", Value = "", Description = "Terminal No", NetworkType = NetworkType.YapiKredi
                },
                new NetworkConfigurationModel
                {
                    Key = "Username", Value = "", Description = "Kullanıcı Adı", NetworkType = NetworkType.YapiKredi
                },
                new NetworkConfigurationModel
                {
                    Key = "Password", Value = "", Description = "Şifre", NetworkType = NetworkType.YapiKredi
                },
                new NetworkConfigurationModel
                {
                    Key = "PosnetId", Value = "", Description = "PosnetId", NetworkType = NetworkType.YapiKredi
                }
            };

            return model;
        }

        public Result Pay3D(BaseRequest request, System.Collections.Specialized.NameValueCollection collection)
        {
            
            for (var i = 0; i < collection.Count; i++)
            {
                request.Accounts.Add(collection.GetKey(i), collection[i]);
            }

            var prepare = new Post
            {
                Request = request,
                ConfigName = "YKB_Pay_3DEnd.xml",
                ContentType = "application/x-www-form-urlencoded",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = "xmldata="

            };
            var response= _httpClient.Post(prepare);
            return  Handler(response);
        }

        public bool Check3D(System.Collections.Specialized.NameValueCollection formCollection, Dictionary<string, object> accounts)
        {
            return true;
        }
    }
}
