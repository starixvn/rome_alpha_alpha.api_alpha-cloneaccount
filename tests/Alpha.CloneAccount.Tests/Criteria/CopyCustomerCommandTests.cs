namespace Alpha.CloneAccount.Tests.Criteria
{
    using System;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class CopyCustomerCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ThrowsNotImplementedException()
        {
            // Arrange
            var criteria = new CopyCustomerCommandForTesting(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                8,
                108,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("     ", false)]
        [InlineData("agent", true)]
        public void IsValid_UserName_ReturnsExpectedResult(string userName, bool expectedResult)
        {
            var criteria = new CopyCustomerCommandForTesting(
                userName,
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                8,
                108,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("     ", false)]
        [InlineData("1234aaAA", true)]
        public void IsValid_Password_ReturnsExpectedResult(string password, bool expectedResult)
        {
            var criteria = new CopyCustomerCommandForTesting(
                "member",
                password,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                8,
                108,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(-100, false)]
        [InlineData(0, true)]
        [InlineData(1000000, true)]
        public void IsValid_Credit_ReturnsExpectedResult(decimal credit, bool expectedResult)
        {
            var criteria = new CopyCustomerCommandForTesting(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                credit,
                8,
                108,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_Creator_ReturnsExpectedResult(int creator, bool expectedResult)
        {
            var criteria = new CopyCustomerCommandForTesting(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                creator,
                108,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120485, true)]
        public void IsValid_ExistingCustId_ReturnsExpectedResult(int sourceCustId, bool expectedResult)
        {
            var criteria = new CopyCustomerCommandForTesting(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                8,
                sourceCustId,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

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
            var criteria = new CopyCustomerCommandForTesting(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                8,
                33120485,
                0,
                "en-US",
                string.Empty,
                systemId);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
