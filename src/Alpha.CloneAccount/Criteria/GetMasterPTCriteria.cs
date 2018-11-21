namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data.Repository;

    public class GetMasterPTCriteria : CriteriaBase
    {
        public GetMasterPTCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "Acc_CustomerSelMAgentInfo";

        public override bool IsValid()
            => CustId > 0;
    }
}
