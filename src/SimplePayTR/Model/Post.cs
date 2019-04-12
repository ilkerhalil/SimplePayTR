using System.Collections.Generic;
using SimplePayTR.Interfaces;
using SimplePayTR.Model;

namespace SimplePayTR
{
    public class Post
    {
        public Post() {
            Method = HttpClientMethod.Post;
            Parameters = new Dictionary<string, object>();
        }

        public BaseRequest Request { get; set; }

        public HttpClientRequestFormat RequestFormat { get; set; }

        public string ContentType { get; set; }

        public string PreTag { get; set; }

        public string ConfigName { get; set; }

        public bool IsQueryParameter { get; set; }

        public HttpClientMethod Method { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

    }
}
