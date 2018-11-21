namespace Alpha.CloneAccount.Utils
{
    public static class LanguageUtils
    {
        public static string ToCasinoLanguage(string language)
        {
            switch (language)
            {
                case "en-US": return language + "_En";
                case "ja-JP": return language + "_Jp";
                case "zh-TW": return language + "_Ct"; // Traditional
                case "zh-CN": return language + "_Cs"; // Simplified
                case "th-TH": return language + "_Th";
                case "ko-KR": return language + "_Ko";
                default: return language + "_En";
            }
        }

        public static string ToLanguageKey(string language)
        {
            switch (language)
            {
                case "en-US": return "E";
                case "ja-JP": return "J";
                case "zh-TW": return "CT"; // Traditional
                case "zh-CN": return "CS"; // Simplified
                case "th-TH": return "TH";
                case "ko-KR": return "KO";
                case "vi-VN": return "VI";
                default: return "E";
            }
        }
    }
}
