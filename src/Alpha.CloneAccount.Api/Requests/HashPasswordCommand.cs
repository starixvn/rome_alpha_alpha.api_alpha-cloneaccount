using MediatR;

namespace Alpha.CloneAccount.Api.Requests
{
    public class HashPasswordCommand
    {
        public string Password { get; set; }

        public bool IsValid()
            => !string.IsNullOrWhiteSpace(Password);
    }
}