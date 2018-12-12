namespace Alpha.CloneAccount.Api.Tests.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Handlers;
    using Api.Requests;
    using Constants;
    using Enumerations;
    using Models;
    using NSubstitute;
    using Params;
    using Services;
    using Xunit;

    [Trait("Category", "Handlers")]
    public class CustomerHandlerTests
    {
        private readonly CustomerService customerService;
        private readonly CustomerHandler customerHandler;

        public CustomerHandlerTests()
        {
            customerService = Substitute.For<CustomerService>();
            customerHandler = new CustomerHandler(customerService);
        }

        [Fact]
        public async Task HandleValidateMaxNewMember_Always_CallsValidateMaxNewMember()
        {
            // Arrange
            var request = new ValidateMaxNewMemberQuery(8, "en-US");

            // Act
            await customerHandler.Handle(request, default(CancellationToken));

            // Assert
            await customerService
                .Received()
                .ValidateMaxNewMember(request.AgentId, request.Language);
        }

        [Fact]
        public async Task HandleCustomerPTInfo_Always_CallsGetPTInfo()
        {
            // Arrange
            var request = new CustomerPTInfoQuery(8, 4);

            // Act
            await customerHandler.Handle(request, default(CancellationToken));

            // Assert
            await customerService
                .Received()
                .GetSMAPTInfo(Enumeration.FromValue<UserLevel>(request.CustLevel), request.CustId);
        }

        [Fact]
        public async Task HandleCopyCustomer_Always_CallsCopyCustomer()
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 3,
                UserName = "agent",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            await customerHandler.Handle(command, default(CancellationToken));

            // Assert
            await customerService
                .Received()
                .CopyCustomer(Arg.Any<CopyCustomerParam>());
        }
    }
}
