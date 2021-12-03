namespace Alpha.CloneAccount.Utils
{
    public static class LanguageUtils
    {
        public static string ToLanguageKey(string language)
        {
            var lowerLanguage = string.IsNullOrWhiteSpace(language)
                ? "en-us"
                : language.ToLowerInvariant();

            switch (lowerLanguage)
            {
                case "en-us": return "E";
                case "ja-jp": return "J";
                case "zh-tw": return "CT"; // Traditional
                case "zh-cn": return "CS"; // Simplified
                case "th-th": return "TH";
                case "ko-kr": return "KO";
                case "vi-vn": return "VI";
                default: return "E";
            }
        }
    }
}
