namespace Alpha.CloneAccount.Api.Requests
{
    using MediatR;

    public class ValidateMaxNewMemberQuery : IRequest<Models.Result>
    {
        public ValidateMaxNewMemberQuery(int agentId, string language)
        {
            AgentId = agentId;
            Language = language;
        }

        public int AgentId { get; }

        public string Language { get; }

        public bool IsValid()
            => AgentId > 0;
    }
}