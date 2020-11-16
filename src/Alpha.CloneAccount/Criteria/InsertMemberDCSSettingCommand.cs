namespace Alpha.CloneAccount.Criteria
{
    using Alpha.CloneAccount.Configurations;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMemberDCSSettingCommand : NonQueryCommand
    {
        public InsertMemberDCSSettingCommand(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            string ip)
        {
            CustId = custId;
            SourceCustId = sourceCustId;
            ActorId = actorId;
            ActorSubAccId = actorSubAccId;
            IP = ip;
        }

        public int CustId { get; }

        [SpParam(SqlParamName = "ExistingCustId")]
        public int SourceCustId { get; }

        [SpParam(SqlParamName = "UserId")]
        public int ActorId { get; }

        [SpParam(SqlParamName = "SubAccId")]
        public int ActorSubAccId { get; }

        public string IP { get; }

        public byte SettingStatus => AppSettings.SettingStatus;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Acc_CustSetting_DCSSettingCopyInsertMember";

        public override bool IsValid()
            => CustId > 0
            && SourceCustId > 0
            && ActorId > 0
            && ActorSubAccId >= 0;
    }
}
