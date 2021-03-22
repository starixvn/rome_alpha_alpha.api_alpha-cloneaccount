namespace Alpha.CloneAccount.Tests.Utils
{
    using CloneAccount.Utils;
    using Xunit;

    [Trait("Category", "Utils")]
    public class LanguageUtilsTests
    {
        [Theory]
        [InlineData(null, "_En")]
        [InlineData("", "_En")]
        [InlineData("   ", "_En")]
        [InlineData("en-US", "_En")]
        [InlineData("ja-JP", "_Jp")]
        [InlineData("zh-TW", "_Ct")]
        [InlineData("zh-CN", "_Cs")]
        [InlineData("th-TH", "_Th")]
        [InlineData("ko-KR", "_Ko")]
        [InlineData("abc", "_En")]
        public void ToCasinoLanguage_Always_ReturnsCorrectCasinoLanguage(string language, string casinoLanguage)
        {
            // Arrange & Act
            var actualResult = LanguageUtils.ToCasinoLanguage(language);

            // Assert
            Assert.Equal(language + casinoLanguage, actualResult);
        }

        [Theory]
        [InlineData(null, "E")]
        [InlineData("", "E")]
        [InlineData("   ", "E")]
        [InlineData("en-US", "E")]
        [InlineData("ja-JP", "J")]
        [InlineData("zh-TW", "CT")]
        [InlineData("zh-CN", "CS")]
        [InlineData("th-TH", "TH")]
        [InlineData("ko-KR", "KO")]
        [InlineData("vi-VN", "VI")]
        [InlineData("abc", "E")]
        public void ToLanguageKey_Always_ReturnsCorrectToLanguageKey(string language, string languageKey)
        {
            // Arrange & Act
            var actualResult = LanguageUtils.ToLanguageKey(language);

            // Assert
            Assert.Equal(languageKey, actualResult);
        }
    }
}
