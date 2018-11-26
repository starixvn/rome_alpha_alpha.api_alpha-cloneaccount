namespace Alpha.CloneAccount.Models
{
    using Enumerations;

    public class CustomerInfo
    {
        public CustomerInfo()
        {
        }

        public CustomerInfo(
            int custId,
            string userPwd,
            string userName,
            string userName2,
            string firstName,
            string lastName,
            string phone,
            string mobilePhone,
            string fax,
            int currency,
            bool closed,
            bool suspend,
            int superId,
            int masterId,
            int agentId,
            byte roleId,
            bool syncCasino,
            bool disableCasino,
            bool hideNG2,
            bool syncedNG2,
            bool disableNG2,
            bool disableGoldDeluxe)
        {
            CustId = custId;
            UserPwd = userPwd;
            UserName = userName;
            UserName2 = userName2;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            MobilePhone = mobilePhone;
            Fax = fax;
            Currency = currency;
            Closed = closed;
            Suspend = suspend;
            SuperId = superId;
            MasterId = masterId;
            AgentId = agentId;
            RoleId = roleId;
            SyncCasino = syncCasino;
            DisableCasino = disableCasino;
            HideNG2 = hideNG2;
            SyncedNG2 = syncedNG2;
            DisableNG2 = disableNG2;
            DisableGoldDeluxe = disableGoldDeluxe;
        }

        public int CustId { get; }

        public string UserPwd { get; }

        public string UserName { get; }

        public string UserName2 { get; }

        public string FirstName { get;  }

        public string LastName { get; }

        public string Phone { get; }

        public string MobilePhone { get; }

        public string Fax { get; }

        public int Currency { get; }

        public bool Closed { get; }

        public bool Suspend { get; }

        public int SuperId { get; }

        public int MasterId { get; }

        public int AgentId { get; }

        public byte RoleId { get; }

        public UserLevel CustLevel
            => Enumeration.FromValue<UserLevel>(RoleId);

        public bool SyncCasino { get; }

        public bool DisableCasino { get; }

        public bool HideNG2 { get; }

        public bool SyncedNG2 { get; }

        public bool DisableNG2 { get; }

        public bool DisableGoldDeluxe { get; }
    }
}
