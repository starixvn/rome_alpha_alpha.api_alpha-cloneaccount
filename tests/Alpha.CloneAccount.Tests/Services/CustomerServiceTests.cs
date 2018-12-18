namespace Alpha.CloneAccount.Tests.Services
{
    using System.Threading.Tasks;
    using CloneAccount.Criteria;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Services")]
    public class CustomerServiceTests
    {
        private readonly IDynamicRepository repository;
        private readonly PasswordService passwordService;
        private readonly SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
        private readonly ThirdPartyProductService thirdPartyProductService;
        private readonly CustomerService customerService;

        public CustomerServiceTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            passwordService = Substitute.For<PasswordService>();
            copyMemberSpParamsBuilder = Substitute.For<SportbooksCopyMemberSpParamsBuilder>();
            thirdPartyProductService = Substitute.For<ThirdPartyProductService>();
            customerService = new CustomerServiceImpl(repository, passwordService, copyMemberSpParamsBuilder, thirdPartyProductService);
        }

        [Fact]
        public async Task ValidateMaxNewMember_ValidCriteria_CallsExecuteAsync()
        {
            // Arrange
            var agentId = 33120484;
            var language = "en-US";

            // Act
            await customerService.ValidateMaxNewMember(agentId, language);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<ValidateMaxNewMemberCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_ValidSuperCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;

            // Act
            await customerService.GetSMAPTInfo(UserLevel.Super, custId);

            // Assert
            await repository
                .Received()
                .GetAsync<CustomerPTInfo>(Arg.Any<GetSMAPTInfoCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_ValidMasterCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;

            // Act
            await customerService.GetSMAPTInfo(UserLevel.Master, custId);

            // Assert
            await repository
                .Received()
                .GetAsync<CustomerPTInfo>(Arg.Any<GetSMAPTInfoCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_ValidAgentCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;

            // Act
            await customerService.GetSMAPTInfo(UserLevel.Agent, custId);

            // Assert
            await repository
                .Received()
                .GetAsync<CustomerPTInfo>(Arg.Any<GetSMAPTInfoCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_InvalidCriteria_DoNothing()
        {
            // Arrange
            var custId = 0;
            var custLevel = UserLevel.Agent;

            // Act
            await customerService.GetSMAPTInfo(custLevel, custId);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetSMAPTInfoCriteria(custLevel, custId));
        }

        [Fact]
        public async Task GetPTInfo_InvalidCustId_ReturnsNull()
        {
            // Arrange
            var custId = 8;
            var custLevel = UserLevel.Super;
            CustomerPTInfo expectedResult = null;
            repository
                .GetAsync<CustomerPTInfo>(Arg.Any<ICriteria>())
                .Returns(expectedResult);

            // Act
            var actualResult = await customerService.GetSMAPTInfo(custLevel, custId);

            // Assert
            Assert.Null(actualResult);
        }

        [Fact]
        public async Task CopyCustomer_ValidMaster_CallsExecuteAsync()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000000,
                0,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);
            passwordService
                .Generate(8, true, true, true)
                .Returns("1234aaAA");

            // Act
            await customerService.CopyCustomer(param);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<CopyMasterCommand>());
        }

        [Fact]
        public async Task CopyCustomer_ValidAgent_CallsExecuteAsync()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(3),
                "agent",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);
            passwordService
                .Generate(8, true, true, true)
                .Returns("1234aaAA");

            // Act
            await customerService.CopyCustomer(param);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<CopyAgentCommand>());
        }
    }
}