namespace Alpha.CloneAccount.Tests.Criteria
{
    using Alpha.CloneAccount.Criteria;
    using Xunit;

    public class CasinoAPILogCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new CasinoAPILogCriteria(
                55218371,
                55218381,
                4147,
                "UpdateCustGameTypeStakeLevel",
                "<DocumentElement></DocumentElement>",
                "<DocumentElement />",
                0);
            var expectedResult = "WriteLog_UserLog_CasinoAPI";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Constructor_Always_ReturnsCorrectProperties()
        {
            // Arrange & Act
            var criteria = new CasinoAPILogCriteria(
                55218371,
                55218381,
                4147,
                "UpdateCustGameTypeStakeLevel",
                "<DocumentElement></DocumentElement>",
                "<DocumentElement />",
                0);

            // Assert
            Assert.True(
                55218371 == criteria.UserId
                && 55218381 == criteria.CustId
                && 4147 == criteria.ActionId
                && "UpdateCustGameTypeStakeLevel" == criteria.FuncName
                && "<DocumentElement></DocumentElement>" == criteria.TrackingValue
                && "<DocumentElement />" == criteria.ReturnValue
                && 0 == criteria.ReturnCode);
        }
    }
}
