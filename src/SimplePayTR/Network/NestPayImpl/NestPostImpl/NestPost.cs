using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR.Network.NestPayImpl.NestPostImpl
{
    public class NestPost:Post
    {
        public static NestPost CreatePost(BaseRequest request)
        {
            return new NestPost
            {
                Request = request,
                ContentType = "application/x-www-form-urlencoded",
                RequestFormat = HttpClientRequestFormat.Xml,
                PreTag = "DATA="
            };
        }
    }
}
