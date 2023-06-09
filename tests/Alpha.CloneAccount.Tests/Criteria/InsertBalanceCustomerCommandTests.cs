namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class InsertBalanceCustomerCommandTests
    {
        [Fact]
        public void GetSettingKey_isCreditBalance_ReturnsCreditKey()
        {
            // Arrange
            var criteria = new InitBalanceCustomerCommand(8, isCreditBalance: true);
            var expectedResult = "SiteDB_Dot_BalanceCreditInitialize";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetSettingKey_isDebitBalance_ReturnsDebitKey()
        {
            // Arrange
            var criteria = new InitBalanceCustomerCommand(8, isCreditBalance: false);
            var expectedResult = "SiteDB_Dot_BalanceDebitInitialize";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
