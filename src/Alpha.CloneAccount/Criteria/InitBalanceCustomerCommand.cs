namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class InitBalanceCustomerCommand : NonQueryCommand
    {
        public InitBalanceCustomerCommand(int custId, bool isCreditBalance)
        {
            CustId = custId;
            IsCreditBalance = isCreditBalance;
        }

        public int CustId { get; }

        public int ErrCode { get; set; }

        [SpParam(Ignored = true)]
        private bool IsCreditBalance { get; }

        public override string GetSettingKey()
            => IsCreditBalance ? "SiteDB_Dot_BalanceCreditInitialize" : "SiteDB_Dot_BalanceDebitInitialize";

        public override bool IsValid()
            => CustId > 0;
    }
}
