namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class GetAgentPTCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new GetAgentPTCriteria(33120484);
            var expectedResult = "Acc_CustomerSelAgentInfo";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120484, true)]
        public void IsValid_CustId_ReturnsExpectedResult(int custId, bool expectedResult)
        {
            var criteria = new GetAgentPTCriteria(custId);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
