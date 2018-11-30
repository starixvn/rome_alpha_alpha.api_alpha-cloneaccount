namespace Alpha.CloneAccount.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.GoldDeluxe.CoreSetting.CommandResult;
    using Fanex.GoldDeluxe.CoreSetting.DTOs;
    using Fanex.GoldDeluxe.CoreSetting.Mappings;
    using Fanex.GoldDeluxe.CoreSetting.ParameterObjects;
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using Fanex.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    [Trait("Category", "Services")]
    public class GoldDeluxeServiceTests
    {
        private readonly ISettingService settingService;
        private readonly ILogger logger;
        private readonly GoldDeluxeService goldDeluxeService;

        public GoldDeluxeServiceTests()
        {
            settingService = Substitute.For<ISettingService>();
            logger = Substitute.For<ILogger>();
            goldDeluxeService = new GoldDeluxeServiceImpl(settingService, logger);
        }

        [Fact]
        public void UpdateMemberSettings_Always_CallsUpdateSettingFromExistingMember()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var disableGoldDeluxe = false;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
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
            goldDeluxeService.UpdateMemberSettings(
                custId,
                sourceCustId,
                disableGoldDeluxe,
                param);

            // Assert
            settingService
                .Received()
                .UpdateSettingFromExistingMember(Arg.Any<UpdateSettingFromExistingMemberParameters>());
        }

        [Fact]
        public void UpdateMemberSettings_Always_CallsGetSetting()
        {
            // Arrange
            Mapper.Initialize(x =>
            {
                x.AddProfile<ViewModelToParameterObjectMappingProfile>();
                x.AddProfile<ParameterObjectToCriteriaMappingProfile>();
            });

            var custId = 33120484;
            var sourceCustId = 33120485;
            var disableGoldDeluxe = false;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
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
                .UpdateSettingFromExistingMember(Arg.Any<UpdateSettingFromExistingMemberParameters>())
                .Returns(new MultipleCommandResults
                {
                    CommandResultsList = new List<ICommandResults>()
                });

            // Act
            goldDeluxeService.UpdateMemberSettings(
                custId,
                sourceCustId,
                disableGoldDeluxe,
                param);

            // Assert
            settingService
                .Received()
                .GetSetting(Arg.Any<GetSettingParameters>());
        }

        [Fact]
        public void UpdateMemberSettings_Always_CallsUpdateGameSetting()
        {
            // Arrange
            Mapper.Initialize(x =>
            {
                x.AddProfile<ViewModelToParameterObjectMappingProfile>();
                x.AddProfile<ParameterObjectToCriteriaMappingProfile>();
            });

            var custId = 33120484;
            var sourceCustId = 33120485;
            var disableGoldDeluxe = false;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
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
                .UpdateSettingFromExistingMember(Arg.Any<UpdateSettingFromExistingMemberParameters>())
                .Returns(new MultipleCommandResults
                {
                    CommandResultsList = new List<ICommandResults>()
                });
            settingService
                .GetSetting(Arg.Any<GetSettingParameters>())
                .Returns(new SettingDTO());

            // Act
            goldDeluxeService.UpdateMemberSettings(
                custId,
                sourceCustId,
                disableGoldDeluxe,
                param);

            // Assert
            settingService
                .Received()
                .UpdateGameSetting(Arg.Any<UpdateGameSettingParameters>());
        }

        [Fact]
        public void UpdateMemberSettings_Always_CallsUpdateStatus()
        {
            // Arrange
            Mapper.Initialize(x =>
            {
                x.AddProfile<ViewModelToParameterObjectMappingProfile>();
                x.AddProfile<ParameterObjectToCriteriaMappingProfile>();
            });

            var custId = 33120484;
            var sourceCustId = 33120485;
            var disableGoldDeluxe = false;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
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
                .UpdateSettingFromExistingMember(Arg.Any<UpdateSettingFromExistingMemberParameters>())
                .Returns(new MultipleCommandResults
                {
                    CommandResultsList = new List<ICommandResults>()
                });
            settingService
                .GetSetting(Arg.Any<GetSettingParameters>())
                .Returns(new SettingDTO());
            settingService
                .UpdateGameSetting(Arg.Any<UpdateGameSettingParameters>())
                .Returns(new CommandResults());

            // Act
            goldDeluxeService.UpdateMemberSettings(
                custId,
                sourceCustId,
                disableGoldDeluxe,
                param);

            // Assert
            settingService
                .Received()
                .UpdateStatus(Arg.Any<UpdateDisableStatusParameters>());
        }

        [Fact]
        public void UpdateMemberSettings_ThrowException_CallsWriteLog()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var disableGoldDeluxe = false;
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
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
                .UpdateSettingFromExistingMember(Arg.Any<UpdateSettingFromExistingMemberParameters>())
                .Throws(new Exception(exceptionMessage));

            // Act
            goldDeluxeService.UpdateMemberSettings(
                custId,
                sourceCustId,
                disableGoldDeluxe,
                param);

            // Assert
            logger.Received().Warn(exceptionMessage);
        }
    }
}