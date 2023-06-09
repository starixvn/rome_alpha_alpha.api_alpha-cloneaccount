namespace Alpha.CloneAccount.Tests.Services
{
    using System.Threading.Tasks;
    using CloneAccount.Criteria;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Services")]
    public class BalanceServiceTests
    {
        private readonly IDynamicRepository repository;
        private readonly IBalanceService balanceService;

        public BalanceServiceTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            balanceService = new BalanceService(repository);
        }

        [Fact]
        public async Task InitBalanceCustomer_Always_CallsInsertCreditAndDebit()
        {
            // Arrange
            var agentId = 33120484;

            // Act
            await balanceService.InitBalanceCustomer(agentId);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Is<InitBalanceCustomerCommand>(i => i.IsCreditBalance == true));

            await repository
                .Received()
                .ExecuteAsync(Arg.Is<InitBalanceCustomerCommand>(i => i.IsCreditBalance == false));
        }

        [Fact]
        public async Task InitBalanceCustomer_InsertError_CallsRollback()
        {
            // Arrange
            var agentId = 33120484;
            repository.When(x=>x.ExecuteAsync(Arg.Any<InitBalanceCustomerCommand>())).Do(x=>
                {
                    ((InitBalanceCustomerCommand)x[0]).ErrCode = 1;
                });

            // Act
            await balanceService.InitBalanceCustomer(agentId);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Is<RollbackBalanceCustomerCommand>(i => i.IsCreditBalance == true));

            await repository
                .Received()
                .ExecuteAsync(Arg.Is<RollbackBalanceCustomerCommand>(i => i.IsCreditBalance == false));
        }
    }
}