using SimplePayTR.Model;

namespace SimplePayTR.Interfaces
{
    public interface IHttpClient
    {
        //Result Get(Post post, Func<IRestResponse, Result> handler);
        //Result Post(Post post, Func<IRestResponse, Result> handler);


        IHttpClientResponse Get<T>(T post) where T : Post;
        IHttpClientResponse Post<T>(T post) where T : Post;
    }
}