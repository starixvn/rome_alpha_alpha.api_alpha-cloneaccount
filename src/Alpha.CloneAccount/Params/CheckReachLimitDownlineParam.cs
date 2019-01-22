namespace Alpha.CloneAccount.Params
{
    using Utils;

    public class CheckReachLimitDownlineParam
    {
        public CheckReachLimitDownlineParam(
            int custId,
            byte custLevel,
            string username,
            string language)
        {
            CustId = custId;
            CustLevel = custLevel;
            Username = username;
            Language = language;
            LanguageKey = LanguageUtils.ToLanguageKey(Language);
        }

        public int CustId { get; }

        public byte CustLevel { get; }

        public string Username { get; }

        public string Language { get; }

        public string LanguageKey { get; }
    }
}