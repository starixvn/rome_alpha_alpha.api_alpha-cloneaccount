namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class InsertMemberDCSSettingCommandTests
    {
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
    }
}
