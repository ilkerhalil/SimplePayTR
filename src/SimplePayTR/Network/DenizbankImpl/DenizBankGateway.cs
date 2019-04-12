using System.Collections.Generic;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network.DenizbankImpl
{
    public class DenizBankGateway : BaseGateway
    {
        private readonly IHttpClient _httpClient;

        public DenizBankGateway(IHttpClient httpClient)
        :base(httpClient)
        {
            _httpClient = httpClient;
        }

        public override Result Pay(BaseRequest request)
        {
            var denizBankPost = DenizBankPostImpl.DenizBankPost.CreatePost(request);
            denizBankPost.ConfigName = !request.Is3D ? "GB_Pay.xml" : "DB_Pay_3D.cshtml";
            var response =  _httpClient.Post(denizBankPost);
            return Handler(response);
        }

        public override Result Refund(BaseRequest request)
        {
            var denizBankPost = DenizBankPostImpl.DenizBankPost.CreatePost(request);
            denizBankPost.ConfigName = "DB_Refound.xml";
            var response=  _httpClient.Post(denizBankPost);
            return Handler(response);
        }

        protected override Result Handler(IHttpClientResponse serverResponse)
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

        public override List<NetworkConfigurationModel> GetNetworkConfiguration()
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

        public override Result Pay3D(BaseRequest request, System.Collections.Specialized.NameValueCollection collection)
        {
            var result = new Result(false, "")
            {
                Error = collection["ErrorMessage"].ToString(),
                ErrorCode = "XXX000",
                Status = collection["3DStatus"].StartsWith("-") == false
            };
            if (!result.Status)
            {
                return result;
            }
            result.ReferenceNumber = collection["HostRefNum"];
            result.ProvisionNumber = collection["AuthCode"];
            result.Status = true;

            return result;
        }

        public override bool Check3D(System.Collections.Specialized.NameValueCollection formCollection, Dictionary<string, object> accounts)
        {
            var strMdStatus = formCollection.Get("mdstatus");
            return (strMdStatus == "1" | strMdStatus == "2" | strMdStatus == "3" | strMdStatus == "4");
        }
    }
}