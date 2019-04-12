using System;
using System.Collections.Generic;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network
{
    internal class VakıfBankGateway : IGateway
    {
        private readonly IHttpClient _httpClient;

        public VakıfBankGateway(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Result Pay(BaseRequest request)
        {

            var prepare = new Post
            {
                Request = request,
                ConfigName = request.Is3D ? "VPOS_Pay_3D.cshtml" : "",
                ContentType = "vpos724v3",
                RequestFormat =  HttpClientRequestFormat.Xml,
                IsQueryParameter = true,
                PreTag = "",
                Method = HttpClientMethod.Get
            };

            var expiredate = request.Pos.ExpireDate.Substring(2, 2) + request.Pos.ExpireDate.Substring(0, 2);
            var total = (request.Pos.Total * 100).ToString("N0").PadLeft(12, '0');
 

            if (request.Is3D)
            {
                var response= _httpClient.Post(prepare);
                return Handler(response);
            }
            else
            {
                prepare.Parameters.Add("kullanici", request.Accounts["kullanici"]);
                prepare.Parameters.Add("sifre", request.Accounts["sifre"]);
                prepare.Parameters.Add("uyeno", request.Accounts["uyeno"]);
                prepare.Parameters.Add("posno", request.Accounts["posno"]);
                prepare.Parameters.Add("islem", "PRO");

                prepare.Parameters.Add("kkno", request.Pos.CardNumber);
                prepare.Parameters.Add("gectar", expiredate);
                prepare.Parameters.Add("cvc", request.Pos.CVV2);
                prepare.Parameters.Add("tutar", total);

                prepare.Parameters.Add("provno", "000000");
                prepare.Parameters.Add("taksits", (request.Pos.Installment == 1 || request.Pos.Installment == 0) ? "00" : request.Pos.Installment.ToString().PadLeft(2, '0'));
                prepare.Parameters.Add("islemyeri", "I");
                prepare.Parameters.Add("uyeref", request.Pos.ProcessId);
                prepare.Parameters.Add("vbref", 0);
                prepare.Parameters.Add("khip", request.Pos.Ip);
                prepare.Parameters.Add("xcip", request.Accounts["xcip"]);
                var response= _httpClient.Post(prepare);
                return Handler(response);
            }

        }

        Result Handler(IHttpClientResponse serverResponse)
        {
            var result = new Result {ResultContent = serverResponse.Content};
            var hostResponse = ModelHelper.GetInlineContent(result.ResultContent, "Kod");

            if (hostResponse == "00")
            {
                result.Status = true;
                result.ProvisionNumber = ModelHelper.GetInlineContent(result.ResultContent, "Mesaj").Clean(true, false, true, "", true);
                result.ReferenceNumber = ModelHelper.GetInlineContent(result.ResultContent, "VBRef");
                result.ProcessId = ModelHelper.GetInlineContent(result.ResultContent, "UyeRef");
            }
            else
            {
                result.Error = ModelHelper.GetInlineContent(result.ResultContent, "Mesaj");
                result.ErrorCode = ModelHelper.GetInlineContent(result.ResultContent, "Kod");
            }

            return result;
        }


        public Result Refund(BaseRequest request)
        {
            var prepare = new Post
            {
                Request = request,
                ConfigName = "EST_Refound.xml",
                ContentType = "application/x-www-form-urlencoded",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = "DATA="
            };
            var response= _httpClient.Post(prepare);
            return Handler(response);

        }

        public List<NetworkConfigurationModel> GetNetworkConfiguration()
        {
            var model = new List<NetworkConfigurationModel>
            {
                new NetworkConfigurationModel
                {
                    Key = "kullanici", Value = "", Description = "kullanici", NetworkType = NetworkType.Vakifbank
                },
                new NetworkConfigurationModel
                {
                    Key = "sifre", Value = "", Description = "sifre", NetworkType = NetworkType.Vakifbank
                },
                new NetworkConfigurationModel
                {
                    Key = "uyeno", Value = "", Description = "uyeno", NetworkType = NetworkType.Vakifbank
                },
                new NetworkConfigurationModel
                {
                    Key = "posno", Value = "", Description = "posno", NetworkType = NetworkType.Vakifbank
                },
                new NetworkConfigurationModel
                {
                    Key = "xcip", Value = "", Description = "xcip", NetworkType = NetworkType.Vakifbank
                }
            };

            return model;
        }

        public Result Pay3D(BaseRequest request, System.Collections.Specialized.NameValueCollection collection)
        {
            throw new NotImplementedException();
        }

        public bool Check3D(System.Collections.Specialized.NameValueCollection formCollection, Dictionary<string, object> accounts)
        {
            var memberNumber = accounts["uyeno"].ToString().Trim();
            var memberRequestNumber = formCollection["uyeno"].Trim();
            var status = formCollection["status"].Trim();

            return (memberNumber == memberRequestNumber && status == "Y");

        }

    }
}
