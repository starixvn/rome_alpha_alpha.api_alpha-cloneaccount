namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data.Repository;

    public class GetSuperPTCriteria : CriteriaBase
    {
        public GetSuperPTCriteria(int custId, string site)
        {
            CustId = custId;
            Site = site;
        }

        public int CustId { get; }

        public string Site { get; }

        public override string GetSettingKey()
            => "Acc_CustomerSelSAgentInfo";

        public override bool IsValid()
            => CustId > 0;
    }
}
