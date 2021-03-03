namespace Alpha.CloneAccount.Api.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using MediatR;
    using Models;
    using Requests;

    [RoutePrefix("api/customer")]
    public class CustomerController : BaseApiController
    {
        public CustomerController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Route("{agentId}/ValidateMaxNewMember")]
        public async Task<Result> ValidateMaxNewMember(int agentId, string language)
        {
            var message = new ValidateMaxNewMemberQuery(agentId, language);

            return message.IsValid()
              ? await mediator.Send(message)
              : new Result(400, "Bad Request.");
        }

        [HttpGet]
        [Route("{custId}/{custLevel}/pt")]
        public async Task<CustomerPTInfo> GetPT(int custId, byte custLevel)
        {
            var message = new CustomerPTInfoQuery(custId, custLevel);

            return message.IsValid()
              ? await mediator.Send(message)
              : new CustomerPTInfo { CustId = custId };
        }

        [HttpPost]
        [Route("copy")]
        public async Task<CopyCustomerResult> Copy([FromBody]CopyCustomerCommand command)
            => command?.IsValid() == true
            ? await mediator.Send(command)
            : new CopyCustomerResult(0, null, null, 0, null, 0, 0, 400, "Bad Request.");

        [HttpPost]
        [Route("CheckReachLimitDownline")]
        public async Task<Result> CheckReachLimitDownline([FromBody]CheckReachLimitDownlineQuery request)
            => await mediator.Send(request);

        [HttpPost]
        [Route("CheckLimitNewDownline")]
        public async Task<Result> CheckLimitNewDownline([FromBody]CheckLimitNewDownlineQuery request)
            => await mediator.Send(request);
    }
}