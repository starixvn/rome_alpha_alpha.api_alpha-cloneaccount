namespace Alpha.CloneAccount.Api.App_Start
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Http;
    using Controllers;
    using Fanex.CustomerSetting.Module.Casino.Services.Impl;
    using Fanex.CustomerSetting.Module.Casino.Services.Interface;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Services;
    using Fanex.Data.Repository;
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using Fanex.Logging;
    using Fanex.Module.NG2.Services;
    using MediatR;
    using Services;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using SimpleInjector.Lifestyles;

    public static class DependencyConfig
    {
        public static void Init()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterMediator(container);

            RegisterDataAccesses(container);
            RegisterServices(container);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void RegisterMediator(Container container)
        {
            var assemblies = GetAssemblies();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            //var notificationHandlerTypes = container.GetTypesToRegister(
            //    typeof(INotificationHandler<>),
            //    assemblies,
            //    new TypesToRegisterOptions
            //    {
            //        IncludeGenericTypeDefinitions = true,
            //        IncludeComposites = false,
            //    });
            //container.Collection.Register(typeof(INotificationHandler<>), notificationHandlerTypes);

            // Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(GenericPipelineBehavior<,>)
            });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(BaseApiController).GetTypeInfo().Assembly;
        }

        private static void RegisterDataAccesses(Container container)
        {
            container.RegisterSingleton<IDynamicRepository, DynamicRepository>();
        }

        private static void RegisterServices(Container container)
        {
            container.RegisterSingleton(() => Logger.Log);

            container.RegisterSingleton<CustomerService, CustomerServiceImpl>();
            container.RegisterSingleton<PasswordService, PasswordServiceImpl>();
            container.RegisterSingleton<SportbooksCopyMemberSpParamsBuilder, SportbooksCopyMemberSpParamsBuilderImpl>();
            container.RegisterSingleton<ThirdPartyProductService, ThirdPartyProductServiceImpl>();
            container.RegisterSingleton<DCSService, DCSServiceImpl>();
            container.Register<GoldDeluxeService>(
                () => new GoldDeluxeServiceImpl(new SettingService(), container.GetInstance<ILogger>()),
                Lifestyle.Singleton);
            container.Register<KenoService>(
                () => new KenoServiceImpl(new KenoSettingService(), container.GetInstance<ILogger>()),
                Lifestyle.Singleton);
            container.RegisterSingleton<CasinoService, CasinoServiceImpl>();
            container.RegisterSingleton<MemberWinLimitService, MemberWinLimitServiceImpl>();
            container.RegisterSingleton<NG2Service, NG2ServiceImpl>();
            container.RegisterSingleton<ING2StatusService, NG2StatusService>();
        }
    }
}