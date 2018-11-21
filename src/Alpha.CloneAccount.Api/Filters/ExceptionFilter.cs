namespace Alpha.CloneAccount.Api.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http.Filters;
    using Fanex.Logging.Extensions.Web;

    public sealed class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                var exception = actionExecutedContext.Exception;
                var messageBuilder = new StringBuilder();

                messageBuilder.AppendFormat("Message: {0}", exception.Message).AppendLine();
                messageBuilder.AppendFormat("Type: {0}", exception.GetType().ToString()).AppendLine();
                messageBuilder.AppendFormat("Source: {0}", exception.Source).AppendLine();
                messageBuilder.AppendFormat("Url: {0}", actionExecutedContext.Request.RequestUri.OriginalString).AppendLine();

                ExceptionHandler.Handle(exception);

                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, messageBuilder.ToString());
            }

            base.OnException(actionExecutedContext);
        }
    }
}