namespace SimplePayTR.Model
{
    public class RequestPos {

        public RequestPos()
        {
            Installment = 0;
        }

        public string CardNumber { get; set; }

        public string ExpireDate { get; set; }

        public string CVV2 { get; set; }

        public string Hash { get; set; }

        public string FullName { get; set; }

        public string EMail { get; set; }

        public string Ip { get; set; }

        public decimal Total { get; set; }

        public decimal Comission { get; set; }

        public int Installment { get; set; }

        /// <summary>
        /// TransactionId
        /// </summary>
        public string ProcessId { get; set; }

        public string UserId { get; set; }

        public string Extra { get; set; }

        public int BankId { get; set; }

        public string SpecialField1 { get; set; }

        public string SpecialField2 { get; set; }
    }
}