namespace Alpha.CloneAccount.Services
{
    using Criteria;
    using Fanex.Data.Repository;
    using System.Threading.Tasks;

    public interface IBalanceService
    {
        Task InitBalanceCustomer(int custId);
    }

    public class BalanceService : IBalanceService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BalanceService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task InitBalanceCustomer(int custId)
        {
            var commandCredit = new InitBalanceCustomerCommand(custId, isCreditBalance: true);
            var taskCredit = dynamicRepository.ExecuteAsync(commandCredit);

            var commandDebit = new InitBalanceCustomerCommand(custId, isCreditBalance: false);
            var taskdebit = dynamicRepository.ExecuteAsync(commandDebit);

            await Task.WhenAll(taskCredit, taskdebit);
            if (commandDebit.ErrCode != 0 || commandCredit.ErrCode != 0)
            {
                var commandRollbackDebit = new RollbackBalanceCustomerCommand(custId, isCreditBalance: false);
                var taskRollbackDebit = dynamicRepository.ExecuteAsync(commandRollbackDebit);
                
                var commandRollbackCredit = new RollbackBalanceCustomerCommand(custId, isCreditBalance: true);
                var taskRollbackCredit = dynamicRepository.ExecuteAsync(commandRollbackCredit);

                await Task.WhenAll(taskRollbackDebit, taskRollbackCredit);
            }
        }

    }
}