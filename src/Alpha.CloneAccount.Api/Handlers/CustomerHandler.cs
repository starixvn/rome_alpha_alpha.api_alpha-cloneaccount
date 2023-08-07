namespace Alpha.CloneAccount.Api.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Enumerations;
    using MediatR;
    using Models;
    using Params;
    using Requests;
    using Services;

    public class CustomerHandler :
        IRequestHandler<ValidateMaxNewMemberQuery, Result>,
        IRequestHandler<CustomerPTInfoQuery, CustomerPTInfo>,
        IRequestHandler<CopyCustomerCommand, CopyCustomerResult>,
        IRequestHandler<CheckReachLimitDownlineQuery, Result>,
        IRequestHandler<CheckLimitNewDownlineQuery, Result>
    {
        private readonly ICustomerService customerService;

        public CustomerHandler(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public async Task<Result> Handle(ValidateMaxNewMemberQuery request, CancellationToken cancellationToken)
            => await customerService.ValidateMaxNewMember(request.AgentId, request.Language);

        public async Task<CustomerPTInfo> Handle(CustomerPTInfoQuery request, CancellationToken cancellationToken)
            => await customerService.GetSMAPTInfo(
                Enumeration.FromValue<UserLevel>(request.CustLevel),
                request.CustId);

        public async Task<CopyCustomerResult> Handle(CopyCustomerCommand request, CancellationToken cancellationToken)
            => await customerService.CopyCustomer(new CopyCustomerParam(
                request.ActorId,
                request.ActorSubAccId,
                request.ActorSubAccName,
                Enumeration.FromValue<UserLevel>(request.ActorUserLevel),
                request.UserName,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Phone,
                request.MobilePhone,
                request.Fax,
                request.Credit,
                request.MaxCredit,
                request.SourceCustId,
                request.SourceUsername,
                request.Language,
                request.IP,
                request.SystemId,
                request.IsQrCode));

        public async Task<Result> Handle(CheckReachLimitDownlineQuery request, CancellationToken cancellationToken)
            => await customerService.CheckReachLimitDownline(
                new CheckReachLimitDownlineParam(
                    request.CustId,
                    request.CustLevel,
                    request.Username,
                    request.Language));

        public async Task<Result> Handle(CheckLimitNewDownlineQuery request, CancellationToken cancellationToken)
        {
            var result = new Result(0, string.Empty);
            if (request.CustLevel == 2)
            {
                result = await Handle(new ValidateMaxNewMemberQuery(request.CustId, request.Language), cancellationToken);
            }

            if (result.IsSuccess)
            {
                result = await Handle(new CheckReachLimitDownlineQuery
                { CustId = request.CustId, CustLevel = request.CustLevel, Language = request.Language, Username = request.ActorUsername },
                cancellationToken);
            }

            if (result.IsSuccess)
            {
                result = await customerService.CheckLimitNewDownline(
                    new CheckLimitNewDownlineParam(
                        request.CustId,
                        Enumeration.FromValue<UserLevel>(request.CustLevel),
                        request.Language));
            }

            return result;
        }
    }
}