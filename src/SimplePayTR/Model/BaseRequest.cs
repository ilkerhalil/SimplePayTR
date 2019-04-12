using System.Collections.Generic;

namespace SimplePayTR.Model
{
    public class BaseRequest
    {
        public string Url { get; set; }

        public Dictionary<string, object> Accounts { get; set; }
        
        public RequestPos Pos { get; set; }

        public bool Is3D { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }

        public int Id { get; set; }

    }
}
