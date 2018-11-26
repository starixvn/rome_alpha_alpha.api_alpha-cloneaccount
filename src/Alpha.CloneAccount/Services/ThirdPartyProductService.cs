namespace Alpha.CloneAccount.Services
{
    using System.Threading.Tasks;
    using Models;
    using Params;

    public interface ThirdPartyProductService
    {
        Task SyncMasterOrAgentSettings(
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
        private readonly CasinoService casinoService;
        private readonly NG2Service ng2Service;
        private readonly DCSService dcsService;
        private readonly GoldDeluxeService goldDeluxeService;
        private readonly KenoService kenoService;

        public ThirdPartyProductServiceImpl(
            CasinoService casinoService,
            NG2Service ng2Service,
            DCSService dcsService,
            GoldDeluxeService goldDeluxeService,
            KenoService kenoService)
        {
            this.casinoService = casinoService;
            this.ng2Service = ng2Service;
            this.dcsService = dcsService;
            this.goldDeluxeService = goldDeluxeService;
            this.kenoService = kenoService;
        }

        public async Task SyncMasterOrAgentSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            await casinoService.SyncMasterOrAgentSettings(createdCustomer, sourceCustomer, actor, param);

            ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer, actor, param);
        }

        public async Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            await dcsService.InsertMemberSettings(createdCustomer, sourceCustomer, actor, param);

            goldDeluxeService.UpdateMemberSettings(createdCustomer, sourceCustomer, actor, param);

            await casinoService.SyncMemberSettings(createdCustomer, sourceCustomer, actor, param, true);

            kenoService.UpdateMemberSettings(createdCustomer, sourceCustomer, actor, param);

            ng2Service.SyncCustomerSettings(createdCustomer, sourceCustomer, actor, param);
        }
    }
}
