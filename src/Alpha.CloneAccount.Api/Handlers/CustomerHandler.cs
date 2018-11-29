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
        IRequestHandler<CustomerPTInfoQuery, CustomerPTInfo>,
        IRequestHandler<CopyCustomerCommand, CopyCustomerResult>
    {
        private readonly CustomerService customerService;

        public CustomerHandler(CustomerService customerService)
        {
            this.customerService = customerService;
        }

        public async Task<CustomerPTInfo> Handle(CustomerPTInfoQuery request, CancellationToken cancellationToken)
            => await customerService.GetPTInfo(
                request.CustId,
                Enumeration.FromValue<UserLevel>(request.CustLevel),
                request.Site);

        public async Task<CopyCustomerResult> Handle(CopyCustomerCommand command, CancellationToken cancellationToken)
            => await customerService.CopyCustomer(new CopyCustomerParam(
                command.ActorId,
                command.ActorSubAccId,
                command.ActorSubAccName,
                Enumeration.FromValue<UserLevel>(command.ActorUserLevel),
                command.UserName,
                command.Password,
                command.FirstName,
                command.LastName,
                command.Phone,
                command.MobilePhone,
                command.Fax,
                command.Credit,
                command.MaxCredit,
                command.SourceCustId,
                command.Language,
                command.IP,
                command.SystemId));
    }
}