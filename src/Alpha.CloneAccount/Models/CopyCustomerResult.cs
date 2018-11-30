namespace Alpha.CloneAccount.Models
{
    public class CopyCustomerResult
    {
        public CopyCustomerResult(
            int custId,
            string custName,
            string password,
            string sourceUsername,
            int code,
            string message)
        {
            CustId = custId;
            CustName = custName;
            Password = password;
            SourceUsername = sourceUsername;
            Code = code;
            Message = message;
        }

        public int CustId { get; }

        public string CustName { get; }

        public string Password { get; }

        public string SourceUsername { get; }

        public int Code { get; }

        public string Message { get; }

        public bool IsSuccess => Code == 0;
    }
}