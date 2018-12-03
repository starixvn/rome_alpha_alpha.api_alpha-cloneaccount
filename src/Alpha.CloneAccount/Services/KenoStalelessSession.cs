namespace Alpha.CloneAccount.Services
{
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Services;
    using Params;

    public class KenoStalelessSession : IKenoSession
    {
        private readonly CopyCustomerParam copyCustomerParam;

        public KenoStalelessSession(CopyCustomerParam copyCustomerParam)
        {
            this.copyCustomerParam = copyCustomerParam;
        }

        public string Ip
            => copyCustomerParam.IP;

        public bool IsLocalTest => false;

        public string Language
            => copyCustomerParam.Language;

        public int SubAccId
            => copyCustomerParam.ActorSubAccId;

        public int UserId
            => copyCustomerParam.ActorId;
    }
}
