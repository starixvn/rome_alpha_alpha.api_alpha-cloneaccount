namespace Alpha.CloneAccount.Params
{
    using Models;
    using Utils;

    public class CloneCustomerParam
    {
        public CloneCustomerParam(
            int actorId,
            int actorSubAccId,
            string actorSubAccName,
            UserLevel actorUserLevel,
            string userName,
            string password,
            string firstName,
            string lastName,
            string phone,
            string mobilePhone,
            string fax,
            decimal credit,
            decimal maxCredit,
            int sourceCustId,
            string language,
            string ip,
            byte systemId)
        {
            ActorId = actorId;
            ActorSubAccId = actorSubAccId;
            ActorSubAccName = actorSubAccName;
            ActorUserLevel = actorUserLevel;

            UserName = userName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            MobilePhone = mobilePhone;
            Fax = fax;
            Credit = credit;
            MaxCredit = maxCredit;
            SourceCustId = sourceCustId;

            Language = language;
            IP = ip;
            SystemId = systemId;
        }

        public int ActorId { get; }

        public int ActorSubAccId { get; }

        public string ActorSubAccName { get; }

        public UserLevel ActorUserLevel { get; }

        public string UserName { get; }

        public string Password { get; private set; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Phone { get; }

        public string MobilePhone { get; }

        public string Fax { get; }

        public decimal Credit { get; }

        public decimal MaxCredit { get; }

        public int SourceCustId { get; }

        public string Language { get; }

        public string LanguageKey
            => LanguageUtils.ToLanguageKey(Language);

        public string IP { get; }

        public byte SystemId { get; }

        public void SetPassword(string password)
        {
            Password = password;
        }
    }
}
