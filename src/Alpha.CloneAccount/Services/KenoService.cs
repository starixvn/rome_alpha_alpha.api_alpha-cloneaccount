namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.CustomerSetting.Module.Core.Models;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.Services;
    using Fanex.CustomerSetting.Module.Keno.KenoSetting.ViewModels;
    using Fanex.Logging;

    public interface KenoService
    {
        IEnumerable<Result> UpdateMemberSettings(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            byte systemId);
    }

    public class KenoServiceImpl : KenoService
    {
        private readonly IKenoSettingService settingService;
        private readonly ILogger logger;
        private readonly Func<bool> showKenoSettingFunc;

        public KenoServiceImpl(
            IKenoSettingService settingService,
            ILogger logger,
            Func<bool> showKenoSettingFunc)
        {
            this.settingService = settingService;
            this.logger = logger;
            this.showKenoSettingFunc = showKenoSettingFunc;
        }

        public IEnumerable<Result> UpdateMemberSettings(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            byte systemId)
        {
            if (showKenoSettingFunc())
            {
                try
                {
                    var custInfoViewModel = new CustomerInformationViewModel
                    {
                        CustId = sourceCustId,
                        UserId = actorId,
                        SubAccId = actorSubAccId,
                        IsMultipleEdit = 0
                    };

                    var sourceSettings = GetSourceMemberSettings(custInfoViewModel);
                    sourceSettings.SystemId = systemId;
                    sourceSettings.CustomerInformation.CustId = custId;

                    return UpdateMemberSettings(sourceSettings);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message);
                }
            }

            return Enumerable.Empty<Result>();
        }

        private AgentEditMemberSettingViewModel GetSourceMemberSettings(
            CustomerInformationViewModel custInfoViewModel)
            => settingService.GetSettingInCaseOfAgentEditMember(custInfoViewModel);

        private IEnumerable<Result> UpdateMemberSettings(AgentEditMemberSettingViewModel sourceSettings)
        {
            foreach (var betSetting in sourceSettings.BetSettings)
            {
                betSetting.IsStatusUpdated = true;
                betSetting.IsBetSettingUpdated = true;
            }

            foreach (var positionTaking in sourceSettings.PositionTakings)
            {
                positionTaking.A1 = positionTaking.A1 < positionTaking.A2 ? positionTaking.A1 : positionTaking.A2;
                positionTaking.IsUpdateA1 = true;
            }

            return settingService.UpdateMemberCustomerSettingInCaseOfAgentEditMember(sourceSettings);
        }
    }
}
