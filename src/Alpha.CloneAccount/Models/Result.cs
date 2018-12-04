namespace Alpha.CloneAccount.Models
{
    public class Result
    {
        public Result(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }

        public bool IsSuccess => Code == 0;
    }
}
