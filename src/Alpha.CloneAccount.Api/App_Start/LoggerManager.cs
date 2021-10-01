namespace Alpha.CloneAccount.Api.App_Start
{
    using System.Collections.Generic;
    using Alpha.CloneAccount.Configurations;
    using Alpha.CloneAccount.Utils;
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.RabbitMQ;

    public static class LoggerManager
    {
        public static void Register()
        {
            LogManager
                .SetDefaultLogCategory(AppSettings.LoggingDefaultCategory)
                .Use(new RabbitMQLogging(AppSettings.RabbitMQConnectionStrings));

            ExceptionHandler
                // To use custom config
                .UseConfig(new LoggingConfiguration()
                // To ignore exceptions
                //.IgnoreException<DivideByZeroException>()
                // To hide sensitive information
                .HideSensitiveInfo(
                    new List<string>
                    {
                        "(?:password=)(?<paramValue>[^&|^\\s]+)",
                        "(\"password\":)(?<paramValue>[^,|^\r]+)"
                    })
                // To Enable Log Http Header
                .GetHttpHeaderInfo()
                // To Enable Log Http Session
                /*.GetHttpSessionInfo()*/)
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            CasinoAPILoggerManager.Register();
        }
    }
}