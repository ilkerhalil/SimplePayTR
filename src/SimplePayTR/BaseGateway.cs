using System.Collections.Generic;
using System.Collections.Specialized;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR
{
    public abstract class BaseGateway : IGateway
    {
        private readonly IHttpClient _httpClient;

        protected BaseGateway(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        public abstract Result Pay(BaseRequest request);

        public abstract Result Refund(BaseRequest request);

        public abstract Result Pay3D(BaseRequest request, NameValueCollection collection);

        public abstract bool Check3D(NameValueCollection formCollection, Dictionary<string, object> accounts);

        public abstract List<NetworkConfigurationModel> GetNetworkConfiguration();

        protected abstract Result Handler(IHttpClientResponse serverResponse);

    }
}