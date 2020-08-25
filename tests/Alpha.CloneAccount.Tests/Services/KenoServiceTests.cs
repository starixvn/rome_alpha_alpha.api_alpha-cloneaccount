namespace Alpha.CloneAccount.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Entities;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Services;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.ViewModels;
    using Fanex.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    [Trait("Category", "Services")]
    public class KenoServiceTests
    {
        private IKenoSettingService settingService;
        private ILogger logger;
        private KenoService kenoService;

        public KenoServiceTests()
        {
            settingService = Substitute.For<IKenoSettingService>();
            logger = Substitute.For<ILogger>();
            kenoService = new KenoServiceImpl(settingService, logger, () => true);
        }

        [Fact]
        public void UpdateMemberSettings_DontShowSetting_DoNothing()
        {
            // Arrange
            settingService = Substitute.For<IKenoSettingService>();
            logger = Substitute.For<ILogger>();
            kenoService = new KenoServiceImpl(settingService, logger, () => false);

            var custId = 33120484;
            var sourceCustId = 33120485;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(2),
                "member",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                sourceCustId,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            kenoService.UpdateMemberSettings(
                custId,
                sourceCustId,
                param);

            // Assert
            settingService
                .DidNotReceive()
                .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>());
        }

        [Fact]
        public void UpdateMemberSettings_ShowSetting_CallsGetSettingInCaseOfAgentEditMember()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(2),
                "member",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                sourceCustId,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);

            // Act
            kenoService.UpdateMemberSettings(
                custId,
                sourceCustId,
                param);

            // Assert
            settingService
                .Received()
                .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>());
        }

        [Fact]
        public void UpdateMemberSettings_ShowSetting_CallsUpdateMemberCustomerSettingInCaseOfAgentEditMember()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(2),
                "member",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                sourceCustId,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);
            settingService
                .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>())
                .Returns(new AgentEditMemberSettingViewModel {
                    CustomerInformation = new CustomerInformation(),
                    BetSettings = new List<BetSetting>(),
                    PositionTakings = new List<AgentEditMemberPositionTaking>()
                });

            // Act
            kenoService.UpdateMemberSettings(
                custId,
                sourceCustId,
                param);

            // Assert
            settingService
                .Received()
                .UpdateMemberCustomerSettingInCaseOfAgentEditMember(Arg.Any<AgentEditMemberSettingViewModel>(), true);
        }

        [Fact]
        public void UpdateMemberSettings_ThrowException_CallsWriteLog()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(2),
                "member",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                sourceCustId,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot);
            var exceptionMessage = "Test Exception";
            settingService
                .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>())
                .Throws(new Exception(exceptionMessage));

            // Act
            kenoService.UpdateMemberSettings(
                custId,
                sourceCustId,
                param);

            // Assert
            logger.Received().Warn(exceptionMessage);
        }
    }
}
