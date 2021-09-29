namespace Alpha.CloneAccount.Criteria
{
    using Alpha.CloneAccount.Constants;
    using Fanex.Data.Repository;

    public class CasinoAPILogCriteria : NonQueryCommand
    {
        public CasinoAPILogCriteria(
            int userId,
            int custId,
            int actionId,
            string functionName,
            string trackingValue,
            string returnValue,
            int returnCode)
        {
            SystemId = SystemIdentity.Alpha;
            UserId = userId;
            CustId = custId;
            ActionId = actionId;
            FuncName = functionName;
            TrackingValue = trackingValue;
            ReturnValue = returnValue;
            ReturnCode = returnCode;
        }

        public byte SystemId { get; }

        public int UserId { get; }

        public int CustId { get; }

        public int ActionId { get; }

        public string FuncName { get; }

        public string TrackingValue { get; }

        public string ReturnValue { get; }

        public int ReturnCode { get; }

        public override string GetSettingKey()
            => "WriteLog_UserLog_CasinoAPI";

        public override bool IsValid()
            => true;
    }
}
