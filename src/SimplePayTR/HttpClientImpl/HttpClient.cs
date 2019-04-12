using System;
using System.Net;
using System.Text;
using RestSharp;
using SimplePayTR.Helper;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.HttpClientImpl
{
    public class HttpClient : IHttpClient
    {
        public HttpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }
        //public Result Get(Post post, Func<IRestResponse, Result> handler)
        //{
        //    var spr = new StringBuilder();
        //    spr.Append(post.Request.Url);

        //    foreach (var item in post.Parameters)
        //    {
        //        spr.AppendFormat("{0}={1}&", item.Key, item.Value);
        //    }

        //    var baseUrl = spr.ToString();
        //    baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

        //    var client = new RestClient(baseUrl);
        //    var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Xml };

        //    request.Parameters.Clear();

        //    var serverResponse = client.Execute(request);

        //    var result = serverResponse.StatusCode == HttpStatusCode.OK ? handler(serverResponse) : new Result { Status = false, Error = "[TIMEOUT]", RequestContent = serverResponse.Content };

        //    result.RequestData = post.Request;

        //    return result;
        //}

        //public Result Post(Post post, Func<IRestResponse, Result> handler)
        //{
        //    var result = new Result();
        //    var content = post.Parameters.Count == 0 ? ModelHelper.ReadEmbedXml(post.ConfigName) : "";
        //    var postData = post.Parameters.Count == 0 ? ModelHelper.CreatePosXml(post.Request, content).Trim() : "";


        //    if (!post.Request.Is3D)
        //    {
                
        //        var client = new RestClient(post.Request.Url);
        //        var request = new RestRequest(post.Method);

        //        if (post.Request.Accounts.ContainsKey("Method"))
        //        {
        //            request.Resource = post.Request.Accounts["Method"].ToString();
        //        }

        //        request.RequestFormat = post.RequestFormat;

        //        if (!post.IsQueryParameter)
        //        {
        //            request.AddParameter(post.ContentType, post.PreTag + postData, RestSharp.ParameterType.RequestBody);
        //        }
        //        else
        //        {
        //            if (post.Parameters.Count == 0)
        //            {
        //                request.AddParameter(post.PreTag, postData);
        //            }
        //            else
        //            {
        //                request.Resource = post.ContentType;

        //                foreach (var item in post.Parameters)
        //                {
        //                    request.AddParameter(item.Key, item.Value, ParameterType.UrlSegment);
        //                }
        //            }
        //        }


        //        client.Timeout = Convert.ToInt32(20000);

        //        var serverResponse = client.Execute(request);

        //        if (serverResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            result = handler(serverResponse);
        //        }
        //        else
        //        {
        //            result = new Result { Status = false, Error = "[TIMEOUT]", RequestContent = serverResponse.Content };
        //        }
        //    }
        //    else
        //    {
        //        result.Status = true;
        //        result.ResultContent = postData;
        //    }

        //    result.RequestData = post.Request;

        //    return result;
        //}

        public IHttpClientResponse Get<T>(T post) where T : Post
        {
            var spr = new StringBuilder();
            spr.Append(post.Request.Url);

            foreach (var item in post.Parameters)
            {
                spr.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            var baseUrl = spr.ToString();
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            var client = new RestClient(baseUrl);
            var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Xml };

            request.Parameters.Clear();

            var serverResponse = client.Execute(request);
            return new HttpClientResponse(serverResponse.Content, (int)serverResponse.StatusCode);

        }

        public IHttpClientResponse Post<T>(T post) where T : Post
        {
            var content = post.Parameters.Count == 0 ? ModelHelper.ReadEmbedXml(post.ConfigName) : "";
            var postData = post.Parameters.Count == 0 ? ModelHelper.CreatePosXml(post.Request, content).Trim() : "";

            var httpMethod = ConvertHttpMethod(post.Method);

            var client = new RestClient(post.Request.Url);
            var request = new RestRequest(httpMethod);

            if (post.Request.Accounts.ContainsKey("Method"))
            {
                request.Resource = post.Request.Accounts["Method"].ToString();
            }
            
            request.RequestFormat = ConvertDataFormat(post.RequestFormat);

            if (!post.IsQueryParameter)
            {
                request.AddParameter(post.ContentType, post.PreTag + postData, RestSharp.ParameterType.RequestBody);
            }
            else
            {
                if (post.Parameters.Count == 0)
                {
                    request.AddParameter(post.PreTag, postData);
                }
                else
                {
                    request.Resource = post.ContentType;

                    foreach (var item in post.Parameters)
                    {
                        request.AddParameter(item.Key, item.Value, ParameterType.UrlSegment);
                    }
                }
            }
            client.Timeout = Convert.ToInt32(20000);
            var serverResponse = client.Execute(request);
            return new HttpClientResponse(serverResponse.Content, (int)serverResponse.StatusCode);
        }

        static Method ConvertHttpMethod(HttpClientMethod method)
        {
            switch (method)
            {
                case HttpClientMethod.Get:
                    return Method.GET;
                case HttpClientMethod.Post:
                    return Method.POST;
                case HttpClientMethod.Options:
                    return Method.OPTIONS;
                case HttpClientMethod.Head:
                    return Method.HEAD;
                    
                case HttpClientMethod.Put:
                    return Method.PUT;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        static DataFormat ConvertDataFormat(HttpClientRequestFormat requestFormat)
        {
            switch (requestFormat)
            {
                case HttpClientRequestFormat.Xml:
                    return DataFormat.Xml;
                case HttpClientRequestFormat.Json:
                    return DataFormat.Json;
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestFormat), requestFormat, null);
            }
        }


    }
}
