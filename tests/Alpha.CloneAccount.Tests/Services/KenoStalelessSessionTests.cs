namespace Alpha.CloneAccount.Tests.Services
{
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Xunit;

    [Trait("Category", "Services")]
    public class KenoStalelessSessionTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                5559,
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
                "127.0.0.1",
                SystemIdentity.Chatbot);

            // Act
            var actualResult = new KenoStalelessSession(param);

            // Assert
            Assert.True(
                actualResult.Ip == "127.0.0.1"
                && !actualResult.IsLocalTest
                && actualResult.Language == "en-US"
                && actualResult.SubAccId == 5559
                && actualResult.UserId == 33120484);
        }
    }
}
