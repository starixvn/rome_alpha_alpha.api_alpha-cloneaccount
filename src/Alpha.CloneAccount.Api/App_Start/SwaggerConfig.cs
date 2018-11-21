using System.Web;
using System.Web.Http;
using Alpha.CloneAccount.Api.App_Start;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
namespace Alpha.CloneAccount.Api.App_Start
{
    public static class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c => c.SingleApiVersion("v1", "Alpha.CloneAccount.API"))
                .EnableSwaggerUi(c =>
                {
                    c.DocExpansion(DocExpansion.List);
                    c.DisableValidator();
                });
        }
    }
}