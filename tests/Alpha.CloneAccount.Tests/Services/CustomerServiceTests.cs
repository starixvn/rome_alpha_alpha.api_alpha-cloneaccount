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
        public async Task GetPTInfo_ValidSuperCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;
            var custLevel = Enumeration.FromValue<UserLevel>(4);
            var site = "BET-ODDS";
            var expectedResult = new CustomerPTInfo(custId, custLevel.Value, "00", 1000000000);
            repository
                .GetAsync<CustomerPTInfo>(Arg.Any<ICriteria>())
                .Returns(expectedResult);

            // Act
            await customerService.GetPTInfo(custId, custLevel, site);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetSuperPTCriteria(custId, site));
        }

        [Fact]
        public async Task GetPTInfo_ValidMasterCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;
            var custLevel = Enumeration.FromValue<UserLevel>(3);
            var site = "BET-ODDS";

            // Act
            await customerService.GetPTInfo(custId, custLevel, site);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetMasterPTCriteria(custId));
        }

        [Fact]
        public async Task GetPTInfo_ValidAgentCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;
            var custLevel = Enumeration.FromValue<UserLevel>(2);
            var site = "BET-ODDS";

            // Act
            await customerService.GetPTInfo(custId, custLevel, site);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetAgentPTCriteria(custId));
        }

        [Fact]
        public async Task GetPTInfo_InvalidCriteria_DoNothing()
        {
            // Arrange
            var custId = 0;
            var custLevel = Enumeration.FromValue<UserLevel>(2);
            var site = "BET-ODDS";

            // Act
            await customerService.GetPTInfo(custId, custLevel, site);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetAgentPTCriteria(custId));
        }

        [Fact]
        public async Task GetPTInfo_InvalidCustId_ReturnsNull()
        {
            // Arrange
            var custId = 8;
            var custLevel = Enumeration.FromValue<UserLevel>(4);
            var site = "BET-ODDS";
            CustomerPTInfo expectedResult = null;
            repository
                .GetAsync<CustomerPTInfo>(Arg.Any<ICriteria>())
                .Returns(expectedResult);

            // Act
            var actualResult = await customerService.GetPTInfo(custId, custLevel, site);

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
                .Generate(8, true, true, true, true)
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
                .Generate(8, true, true, true, true)
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