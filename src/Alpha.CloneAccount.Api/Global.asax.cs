namespace Alpha.CloneAccount.Api
{
    using System.Web.Http;
    using App_Start;
    using Configurations;
    using Fanex.Data;
    using Fanex.Data.CrossCutting.WrappingException;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            DependencyConfig.Init();
            AppSettings.Verify();

            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(resourcePath: AppSettings.StoredProcedurePath, enableWatching: true)
                .Use((settingKey, commandTimeout) => new WrappingExceptionObjectDb(settingKey, commandTimeout))
                .Run();

            var loggingConfiguration = new LoggingConfiguration()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            ExceptionHandler.UseConfig(loggingConfiguration);
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    if (Request.IsLocal)
        //    {
        //        return;
        //    }

        //    Exception ex = Server.GetLastError();

        //    ExceptionHandler.Handle(ex);
        //}
    }
}
