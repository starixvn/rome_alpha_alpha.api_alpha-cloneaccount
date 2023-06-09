namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RollbackBalanceCustomerCommand : NonQueryCommand
    {
        public RollbackBalanceCustomerCommand(int custId, bool isCreditBalance)
        {
            CustId = custId;
            IsCreditBalance = isCreditBalance;
        }

        public int CustId { get; }

        [SpParam(Ignored = true)]
        internal bool IsCreditBalance { get; }

        public override string GetSettingKey()
            => IsCreditBalance ? "SiteDB_Dot_BalanceCreditRemove" : "SiteDB_Dot_BalanceDebitRemove";

        public override bool IsValid()
            => CustId > 0;
    }
}
