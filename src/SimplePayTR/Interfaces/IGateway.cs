using System.Collections.Generic;
using System.Collections.Specialized;
using SimplePayTR.Model;

namespace SimplePayTR.Interfaces
{
    public interface IGateway
    {
        Result Pay(BaseRequest request);

        Result Refund(BaseRequest request);

        Result Pay3D(BaseRequest request, NameValueCollection collection);

        bool Check3D(NameValueCollection formCollection, Dictionary<string, object> accounts);

        List<NetworkConfigurationModel> GetNetworkConfiguration();
    }
}