namespace Alpha.CloneAccount.Services
{
    using System.Threading.Tasks;
    using Models;
    using Params;

    public interface ThirdPartyProductService
    {
        void SyncMasterOrAgentSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);

        Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);
    }

    public class ThirdPartyProductServiceImpl : ThirdPartyProductService
    {
        private readonly NG2Service ng2Service;
        private readonly DCSService dcsService;
        private readonly KenoService kenoService;

        public ThirdPartyProductServiceImpl(
            NG2Service ng2Service,
            DCSService dcsService,
            KenoService kenoService)
        {
            this.ng2Service = ng2Service;
            this.dcsService = dcsService;
            this.kenoService = kenoService;
        }

        public void SyncMasterOrAgentSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
        }

        public async Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            await dcsService.InsertMemberSettings(
                createdCustomer.CustId,
                sourceCustomer.CustId,
                actor.CustId,
                param.ActorSubAccId,
                param.IP);

            kenoService.UpdateMemberSettings(
                createdCustomer.CustId,
                sourceCustomer.CustId,
                param);

            ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
        }
    }
}
