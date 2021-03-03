namespace Alpha.CloneAccount.Tests.Criteria
{
    using System.Collections.Specialized;
    using Alpha.CloneAccount.Configurations;
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class InsertMemberDCSSettingCommandTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectErrCodeAndErrMsg()
        {
            // Arrange & Act
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, 0, string.Empty)
            {
                ErrCode = 201,
                ErrMsg = "Master Max Credit must be between 0 and 24,875,698"
            };

            // Assert
            Assert.True(
                criteria.ErrCode == 201
                && criteria.ErrMsg == "Master Max Credit must be between 0 and 24,875,698");
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, 0, string.Empty);
            var expectedResult = "Acc_CustSetting_DCSSettingCopyInsertMember";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120519, true)]
        public void IsValid_CustId_ReturnsExpectedResult(int custId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(custId, 33120520, 33120484, 0, string.Empty);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120520, true)]
        public void IsValid_SourceCustId_ReturnsExpectedResult(int sourceCustId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, sourceCustId, 33120484, 0, string.Empty);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120484, true)]
        public void IsValid_ActorId_ReturnsExpectedResult(int actorId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, actorId, 0, string.Empty);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, true)]
        [InlineData(int.MaxValue, true)]
        public void IsValid_ActorSubAccId_ReturnsExpectedResult(int actorSubAccId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, actorSubAccId, string.Empty);

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
            // Arrange & Act
            AppSettings.AppSettingValues = new NameValueCollection();
            AppSettings.AppSettingValues.Add("SettingStatus", settingStatus);
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, 0, string.Empty);

            // Assert
            Assert.Equal(expectedResult, criteria.SettingStatus);
        }
    }
}
