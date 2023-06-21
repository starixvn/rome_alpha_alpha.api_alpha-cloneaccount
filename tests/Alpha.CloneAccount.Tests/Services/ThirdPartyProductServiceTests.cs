namespace Alpha.CloneAccount.Tests.Services
{
    using System.Threading.Tasks;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Services")]
    public class ThirdPartyProductServiceTests
    {
        private readonly DCSService dcsService;
        private readonly ThirdPartyProductService thirdPartyProductService;

        public ThirdPartyProductServiceTests()
        {
            dcsService = Substitute.For<DCSService>();
            thirdPartyProductService = new ThirdPartyProductServiceImpl(dcsService);
        }

        [Fact]
        public async Task SyncMemberSettings_Always_CallsSync3rdProductMemberSettings()
        {
            // Arrange
            var createdCustomer = new CustomerInfo(
                33120484,
                "1234aa",
                "master",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3
                );
            var sourceCustomer = new CustomerInfo(
                33120480,
                "1234aa",
                "sourcemaster",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3
                );
            var actor = new CustomerInfo(
                33120519,
                "1234aa",
                "super",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                0,
                0,
                0,
                4
                );
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
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
                SystemIdentity.Chatbot,
                false);

            // Act
            await thirdPartyProductService.SyncMemberSettings(createdCustomer, sourceCustomer, actor, param);

            // Assert
            await dcsService
                .Received()
                .InsertMemberSettings(
                    createdCustomer.CustId,
                    sourceCustomer.CustId,
                    actor.CustId,
                    param.ActorSubAccId,
                    param.IP);
        }
    }
}