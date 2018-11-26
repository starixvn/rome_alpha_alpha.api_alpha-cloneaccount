namespace Alpha.CloneAccount.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateCasinoStatusCriteria : NonQueryCommand
    {
        public UpdateCasinoStatusCriteria()
        {
        }

        public UpdateCasinoStatusCriteria(
            int custId,
            bool disabledCS,
            bool syncCS,
            bool enableDownline,
            byte systemId,
            string ip,
            string language,
            int actorId,
            int subAccId)
        {
            CustId = custId;
            DisabledCS = disabledCS;
            SyncCS = syncCS;
            EnableDownline = enableDownline;
            SystemId = systemId;
            IP = ip;
            Language = language;
            ActorId = actorId;
            SubAccId = subAccId;
        }

        public int CustId { get; }

        public bool DisabledCS { get; }

        public bool SyncCS { get; }

        public bool EnableDownline { get; }

        public byte SystemId { get; }

        [SpParam(SqlParamName = "UserId")]
        public int ActorId { get; }

        public int SubAccId { get; }

        public string IP { get; }

        public string Language { get; }

        public bool Status { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Acc_UpdCSStatus_Cust";

        public override bool IsValid()
            => CustId > 0 && SystemId > 0 && ActorId > 0 && SubAccId >= 0;
    }
}
