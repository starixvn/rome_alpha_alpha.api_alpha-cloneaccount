namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data.Repository;

    public class GetAgentPTCriteria : CriteriaBase
    {
        public GetAgentPTCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "Acc_CustomerSelAgentInfo";

        public override bool IsValid()
            => CustId > 0;
    }
}
