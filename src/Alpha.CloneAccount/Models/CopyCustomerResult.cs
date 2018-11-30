namespace Alpha.CloneAccount.Models
{
    public class CopyCustomerResult
    {
        public CopyCustomerResult(
            int custId,
            string custName,
            string password,
            int custLevel,
            string sourceUsername,
            decimal credit,
            decimal maxCredit,
            int code,
            string message)
        {
            CustId = custId;
            CustName = custName;
            Password = password;
            CustLevel = custLevel;
            SourceUsername = sourceUsername;
            Credit = credit;
            MaxCredit = maxCredit;
            Code = code;
            Message = message;
        }

        public int CustId { get; }

        public string CustName { get; }

        public string Password { get; }

        public int CustLevel { get; }

        public string SourceUsername { get; }

        public decimal Credit { get; }

        public decimal MaxCredit { get; }

        public int Code { get; }

        public string Message { get; }

        public bool IsSuccess => Code == 0;
    }
}