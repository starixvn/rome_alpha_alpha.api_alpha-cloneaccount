namespace Alpha.CloneAccount.Api.Controllers
{
    using System.Web.Http;
    using Filters;
    using MediatR;

    [RoutePrefix("api/{controller}")]
    [HmacAuthorizationFilter]
    public class BaseApiController : ApiController
    {
        protected readonly IMediator mediator;

        protected BaseApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }
    }
}