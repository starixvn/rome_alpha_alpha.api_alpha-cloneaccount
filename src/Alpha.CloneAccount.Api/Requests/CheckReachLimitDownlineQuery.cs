namespace Alpha.CloneAccount.Api.Requests
{
    using MediatR;
    using Models;

    public class CheckReachLimitDownlineQuery : IRequest<Result>
    {
        public int CustId { get; set; }

        public byte CustLevel { get; set; }

        public string Username { get; set; }

        public string Language { get; set; }
    }
}