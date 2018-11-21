namespace Alpha.CloneAccount.Api.Controllers
{
    using System.Web.Http;
    using MediatR;

    [RoutePrefix("api/{controller}")]
    public abstract class BaseApiController : ApiController
    {
        protected readonly IMediator mediator;

        protected BaseApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }
    }
}