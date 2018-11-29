namespace Alpha.CloneAccount.Tests.Configurations
{
    using System.Collections.Specialized;
    using System.Configuration;
    using CloneAccount.Configurations;
    using Xunit;

    [Trait("Category", "Configurations")]
    public class AppSettingsTests
    {
        [Fact]
        public void Verify_MissingStoredProcedurePath_ThrowsConfigurationErrorsException()
        {
            // Arrange
            AppSettings.AppSettingValues = new NameValueCollection();

            // Act & Assert
            Assert.Throws<ConfigurationErrorsException>(() => AppSettings.Verify());
        }

        [Fact]
        public void Verify_StoredProcedurePath_ReturnsCorrectStoredProcedurePath()
        {
            // Arrange
            AppSettings.AppSettingValues = new NameValueCollection();
            AppSettings.AppSettingValues.Add("StoredProcedurePath", "App_Data/SPNames");

            // Act
            AppSettings.Verify();

            // Assert
            Assert.NotEmpty(AppSettings.StoredProcedurePath);
        }

        [Fact]
        public void RemoteRefURL_Always_ReturnsCorrectConfig()
        {
            // Arrange
            AppSettings.AppSettingValues = new NameValueCollection();
            AppSettings.AppSettingValues.Add("RemoteRefURL", "http://ha.nexdev.net:7258/Main/");
            var expectedRemoteRefURL = "http://ha.nexdev.net:7258/Main/";

            // Act
            var actualRemoteRefURL = AppSettings.RemoteRefURL;

            // Assert
            Assert.Equal(expectedRemoteRefURL, actualRemoteRefURL);
        }

        [Fact]
        public void ApiKey_Always_ReturnsCorrectConfig()
        {
            // Arrange
            AppSettings.AppSettingValues = new NameValueCollection();
            AppSettings.AppSettingValues.Add("ApiKey", "X4GZm/KXKv4BINyJWgj+l7UkWH3SOOo5zf1VEdcSaw0=");
            var expectedApiKey = "X4GZm/KXKv4BINyJWgj+l7UkWH3SOOo5zf1VEdcSaw0=";

            // Act
            var actualApiKey = AppSettings.ApiKey;

            // Assert
            Assert.Equal(expectedApiKey, actualApiKey);
        }
    }
}
