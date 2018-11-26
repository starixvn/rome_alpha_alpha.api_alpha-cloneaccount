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
    using Models;
    using Params;

    public interface KenoService
    {
        IEnumerable<Result> UpdateMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);
    }

    public class KenoServiceImpl : KenoService
    {
        private readonly IKenoSettingService settingService;
        private readonly ILogger logger;

        public KenoServiceImpl(
            IKenoSettingService settingService,
            ILogger logger)
        {
            this.settingService = settingService;
            this.logger = logger;
        }

        public IEnumerable<Result> UpdateMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            if (KenoSection.Instance.ShowSetting)
            {
                try
                {
                    var sourceSettings = GetSourceMemberSettings(sourceCustomer.CustId, actor.CustId, param.ActorSubAccId);

                    sourceSettings.SystemId = param.SystemId;
                    sourceSettings.CustomerInformation.CustId = createdCustomer.CustId;

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
            int sourceCustId,
            int actorId,
            int actorSubAccId)
        {
            var custInfoViewModel = new CustomerInformationViewModel
            {
                CustId = sourceCustId,
                UserId = actorId,
                SubAccId = actorSubAccId,
                IsMultipleEdit = 0
            };

            return settingService.GetSettingInCaseOfAgentEditMember(custInfoViewModel);
        }

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
