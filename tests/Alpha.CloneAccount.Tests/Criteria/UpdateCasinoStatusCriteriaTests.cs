namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class UpdateCasinoStatusCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new UpdateCasinoStatusCriteria(
                33120519,
                false,
                true,
                true,
                SystemIdentity.Chatbot,
                string.Empty,
                "E",
                33120484,
                0);
            var expectedResult = "Acc_UpdCSStatus_Cust";

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
            var criteria = new UpdateCasinoStatusCriteria(
                custId,
                false,
                true,
                true,
                SystemIdentity.Chatbot,
                string.Empty,
                "E",
                33120484,
                0);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(byte.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_SystemId_ReturnsExpectedResult(byte systemId, bool expectedResult)
        {
            var criteria = new UpdateCasinoStatusCriteria(
                33120519,
                false,
                true,
                true,
                systemId,
                string.Empty,
                "E",
                33120484,
                0);

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
            var criteria = new UpdateCasinoStatusCriteria(
                33120519,
                false,
                true,
                true,
                SystemIdentity.Chatbot,
                string.Empty,
                "E",
                actorId,
                0);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, true)]
        [InlineData(int.MaxValue, true)]
        public void IsValid_SubAccId_ReturnsExpectedResult(int actorSubAccId, bool expectedResult)
        {
            var criteria = new UpdateCasinoStatusCriteria(
                33120519,
                false,
                true,
                true,
                SystemIdentity.Chatbot,
                string.Empty,
                "E",
                33120484,
                actorSubAccId);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
