namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class RollbackBalanceCustomerCommandTests
    {
        [Fact]
        public void GetSettingKey_isCreditBalance_ReturnsCreditKey()
        {
            // Arrange
            var criteria = new RollbackBalanceCustomerCommand(8, isCreditBalance: true);
            var expectedResult = "SiteDB_Dot_BalanceCreditRemove";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetSettingKey_isDebitBalance_ReturnsDebitKey()
        {
            // Arrange
            var criteria = new RollbackBalanceCustomerCommand(8, isCreditBalance: false);
            var expectedResult = "SiteDB_Dot_BalanceDebitRemove";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
