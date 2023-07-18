namespace Alpha.CloneAccount.Services
{
    using System.Threading.Tasks;
    using Models;
    using Params;

    public interface ThirdPartyProductService
    {
        Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);
    }

    public class ThirdPartyProductServiceImpl : ThirdPartyProductService
    {
        private readonly DCSService dcsService;

        public ThirdPartyProductServiceImpl(
            DCSService dcsService)
        {
            this.dcsService = dcsService;
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
        }
    }
}
