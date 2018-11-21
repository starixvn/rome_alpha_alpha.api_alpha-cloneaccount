namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data.Repository;

    public class GetMemberSportbooksSettingCriteria : CriteriaBase
    {
        public GetMemberSportbooksSettingCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
            => "Acc_CustomerSelEditCustomer";

        public override bool IsValid()
            => CustId > 0;
    }
}
