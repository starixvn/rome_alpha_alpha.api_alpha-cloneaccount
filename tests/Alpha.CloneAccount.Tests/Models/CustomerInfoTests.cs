namespace Alpha.CloneAccount.Tests.Models
{
    using CloneAccount.Models;
    using Xunit;

    [Trait("Category", "Models")]
    public class CustomerInfoTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var actualResult = new CustomerInfo(
                33120484,
                "1234aa",
                "agent",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120480,
                0,
                0,
                3,
                true,
                false,
                false,
                true,
                false,
                false);

            // Assert
            Assert.True(
                actualResult.CustId == 33120484
                && actualResult.UserPwd == "1234aa"
                && actualResult.UserName == "agent"
                && actualResult.UserName2 == string.Empty
                && actualResult.FirstName == string.Empty
                && actualResult.LastName == string.Empty
                && actualResult.Phone == string.Empty
                && actualResult.MobilePhone == string.Empty
                && actualResult.Fax == string.Empty
                && actualResult.Currency == 21
                && !actualResult.Closed
                && !actualResult.Suspend
                && actualResult.SuperId == 33120480
                && actualResult.MasterId == 0
                && actualResult.AgentId == 0
                && actualResult.RoleId == 3
                && actualResult.SyncCasino
                && !actualResult.DisableCasino
                && !actualResult.HideNG2
                && actualResult.SyncedNG2
                && !actualResult.DisableNG2
                && !actualResult.DisableGoldDeluxe
                && actualResult.CustLevel.IsMasterLevel());
        }
    }
}
