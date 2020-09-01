namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Threading.Tasks;
    using Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public interface DCSService
    {
        Task InsertMemberSettings(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            string ip);
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
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            string ip)
        {
            try
            {
                await repository.ExecuteAsync(
                    new InsertMemberDCSSettingCommand(
                        custId,
                        sourceCustId,
                        actorId,
                        actorSubAccId,
                        ip));
            }
            catch (AggregateException ex)
            {
                logger.Warn(ex.ToString());
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }
    }
}
