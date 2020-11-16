namespace Alpha.CloneAccount.Criteria
{
    using Alpha.CloneAccount.Configurations;
    using Fanex.Data.Repository;

    public class GetCustomerInfoCriteria : CriteriaBase
    {
        public GetCustomerInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public byte SettingStatus => AppSettings.SettingStatus;

        public override string GetSettingKey()
            => "Age_CustomerSelUserInfo";

        public override bool IsValid()
            => CustId > 0;
    }
}
