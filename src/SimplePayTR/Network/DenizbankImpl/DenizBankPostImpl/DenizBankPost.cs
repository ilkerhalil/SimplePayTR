using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network.DenizbankImpl.DenizBankPostImpl
{
    public class DenizBankPost:Post
    {
        private DenizBankPost()
        {
            
        }

        public static DenizBankPost CreatePost(BaseRequest request)
        {
            var prepare = new DenizBankPost()
            {
                Request = request,
                
                ContentType = "text/xml",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = ""
            };
            return prepare;
        }

    }
}
