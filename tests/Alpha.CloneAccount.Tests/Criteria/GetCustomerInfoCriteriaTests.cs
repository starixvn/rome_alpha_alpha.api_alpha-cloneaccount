namespace Alpha.CloneAccount.Tests.Criteria
{
    using System.Collections.Specialized;
    using Alpha.CloneAccount.Configurations;
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class GetCustomerInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new GetCustomerInfoCriteria(33120484);
            var expectedResult = "Age_CustomerSelUserInfo";

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
            // Arrange
            var criteria = new GetCustomerInfoCriteria(custId);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("0", 0)]
        [InlineData("abc", 0)]
        public void SettingStatus_Always_AssignsCorrectValue(string settingStatus, byte expectedResult)
        {
            // Arrange && Act
            AppSettings.AppSettingValues = new NameValueCollection();
            AppSettings.AppSettingValues.Add("SettingStatus", settingStatus);
            var criteria = new GetCustomerInfoCriteria(33120484);

            // Assert
            Assert.Equal(expectedResult, criteria.SettingStatus);
        }
    }
}
