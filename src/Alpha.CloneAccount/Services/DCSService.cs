namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Threading.Tasks;
    using Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Models;
    using Params;

    public interface DCSService
    {
        Task InsertMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);
    }

    public class DCSServiceImpl : DCSService
    {
        private readonly IDynamicRepository repository;
        private readonly ILogger logger;

        public DCSServiceImpl(
            IDynamicRepository repository,
            ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task InsertMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            try
            {
                await repository.ExecuteAsync(new InsertMemberDCSSettingCommand(
                    createdCustomer.CustId,
                    sourceCustomer.CustId,
                    actor.CustId,
                    param.ActorSubAccId,
                    param.IP));
            }
            catch (AggregateException ex)
            {
                logger.Warn(ex.Message);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
        }
    }
}
