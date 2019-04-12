namespace SimplePayTR.Interfaces
{
    public class HttpClientResponse : IHttpClientResponse
    {
        public HttpClientResponse(string content, int statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
        public string Content { get; set; }
        public int StatusCode { get; }
    }
}