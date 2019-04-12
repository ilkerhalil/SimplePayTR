namespace SimplePayTR.Model
{
    public class Result
    {
        public bool Status { get; set; }

        public string ResultContent { get; set; }

        public string RequestContent { get; set; }

        public string ProvisionNumber { get; set; }

        public string ReferenceNumber { get; set; }

        public string ProcessId { get; set; }

        public string Error { get; set; }

        public string ErrorCode { get; set; }

        public Result() { }

        public Result(bool status, string error)
        {
            Error = error;
            Status = status;
        }

        public BaseRequest RequestData { get; set; }

    }

   
}
