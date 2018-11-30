namespace Alpha.CloneAccount.Api.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using MediatR;
    using Models;
    using Requests;

    public class CustomerController : BaseApiController
    {
        public CustomerController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Route("api/customer/{custId}/{custLevel}/pt")]
        public async Task<CustomerPTInfo> GetPT(int custId, byte custLevel, string site)
        {
            var message = new CustomerPTInfoQuery(custId, custLevel, site);

            return message.IsValid()
              ? await mediator.Send(message)
              : new CustomerPTInfo { CustId = custId };
        }

        [HttpPost]
        [Route("api/customer/copy")]
        public async Task<CopyCustomerResult> Copy([FromBody]CopyCustomerCommand command)
            => command?.IsValid() == true
            ? await mediator.Send(command)
            : new CopyCustomerResult(0, null, null, null, 400, "Bad Request.");
    }
}