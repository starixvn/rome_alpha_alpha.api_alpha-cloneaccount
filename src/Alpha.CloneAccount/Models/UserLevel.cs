namespace Alpha.CloneAccount.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Enumerations;

    [Serializable]
    public class UserLevel : Enumeration
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Reviewed!")]
        public static readonly UserLevel Super = new UserLevel(4, "Super");

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Reviewed!")]
        public static readonly UserLevel Master = new UserLevel(3, "Master");

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Reviewed!")]
        public static readonly UserLevel Agent = new UserLevel(2, "Agent");

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Reviewed!")]
        public static readonly UserLevel Member = new UserLevel(1, "Member");

        public UserLevel()
        {
        }

        private UserLevel(byte value, string displayName)
            : base(value, displayName)
        {
        }

        public bool IsAgentLevel() => this == Agent;

        public bool IsNotAgentLevel() => !IsAgentLevel();

        public bool IsMasterLevel() => this == Master;

        public bool IsNotMasterLevel() => !IsMasterLevel();

        public bool IsSuperLevel() => this == Super;

        public bool IsNotSuperLevel() => !IsSuperLevel();

        public bool IsMemberLevel() => this == Member;

        public bool IsNotMemberLevel() => !IsMemberLevel();
    }
}
