namespace Alpha.CloneAccount.Services
{
    using System;
    using Fanex.Logging;
    using Fanex.Module.NG2.Entities;
    using Fanex.Module.NG2.Services;
    using Models;
    using Params;

    public interface NG2Service
    {
        Result SyncCustomerSettings(
            CustomerInfo createdCustomer,
            bool disableNG2,
            CustomerInfo actor,
            CopyCustomerParam param);
    }

    public class NG2ServiceImpl : NG2Service
    {
        private readonly ING2StatusService ng2StatusService;
        private readonly ILogger logger;

        public NG2ServiceImpl(ING2StatusService ng2StatusService, ILogger logger)
        {
            this.ng2StatusService = ng2StatusService;
            this.logger = logger;
        }

        public Result SyncCustomerSettings(
            CustomerInfo createdCustomer,
            bool disableNG2,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            if (!actor.HideNG2 && actor.SyncedNG2)
            {
                try
                {
                    return ng2StatusService.AutoInsertAccount(
                        createdCustomer.CustId,
                        createdCustomer.UserName,
                        GetRoleIdEnum(createdCustomer.RoleId),
                        actor.CustId,
                        disableNG2,
                        SiteId.Alpha,
                        actor.Currency,
                        actor.CustId,
                        param.SystemId,
                        param.Language,
                        param.ActorSubAccId);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message);
                }
            }

            return new Result();
        }

        private static RoleId GetRoleIdEnum(byte roleId)
        {
            switch (roleId)
            {
                case 1:
                    return RoleId.Member;

                case 2:
                    return RoleId.Agent;

                case 3:
                    return RoleId.Master;

                default:
                    return RoleId.Super;
            }
        }
    }
}
