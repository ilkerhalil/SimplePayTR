namespace SimplePayTR.Interfaces
{
    public interface IHttpClientResponse
    {
        string Content { get; }
        int StatusCode { get; }

    }
}