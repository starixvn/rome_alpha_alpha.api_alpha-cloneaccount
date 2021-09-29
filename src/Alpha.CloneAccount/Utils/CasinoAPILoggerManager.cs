namespace Alpha.CloneAccount.Utils
{
    using Alpha.CloneAccount.Services;
    using CasinoLibrary.SBAgent;
    using Sunplus.Libraries.APILogger;

    public static class CasinoAPILoggerManager
    {
        public static void Register()
        {
            var logger = new CasinoAPILogger();
            var manager = new APILogManager(logger);

            manager.Register(
                typeof(Customer),
                "AutoInsert*|Update*|Insert*|Move*|Reset*|FullSettlementResetMaxTransfer|Rollback*");
        }
    }
}
