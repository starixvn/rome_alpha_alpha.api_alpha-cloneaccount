namespace Alpha.CloneAccount.Services
{
    using System;
    using AutoMapper;
    using Fanex.GoldDeluxe.CoreSetting.CommandResult;
    using Fanex.GoldDeluxe.CoreSetting.Enumerations;
    using Fanex.GoldDeluxe.CoreSetting.ParameterObjects;
    using Fanex.GoldDeluxe.CoreSetting.Services;
    using Fanex.GoldDeluxe.CoreSetting.ViewModels;
    using Fanex.Logging;
    using Models;
    using Params;

    public interface GoldDeluxeService
    {
        IMultipleCommandResults UpdateMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CloneCustomerParam param);
    }

    public class GoldDeluxeServiceImpl : GoldDeluxeService
    {
        private readonly ISettingService settingService;
        private readonly ILogger logger;

        public GoldDeluxeServiceImpl(
            ISettingService settingService,
            ILogger logger)
        {
            this.settingService = settingService;
            this.logger = logger;
        }

        public IMultipleCommandResults UpdateMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CloneCustomerParam param)
        {
            try
            {
                // Update WinLimit, PT and Comm.
                var result = UpdateSettingFromExistingMember(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
                var commandResults = result.CommandResultsList;

                var gameSettingResult = UpdateGameSettings(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
                commandResults.Add(gameSettingResult);

                if (result.IsSuccessful)
                {
                    var statusResult = UpdateStatus(
                        createdCustomer,
                        sourceCustomer,
                        actor,
                        param);
                    commandResults.Add(statusResult);
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return null;
            }
        }

        private IMultipleCommandResults UpdateSettingFromExistingMember(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CloneCustomerParam param)
        {
            var criteria = new UpdateSettingFromExistingMemberParameters
            {
                CustId = createdCustomer.CustId,
                FromCustId = sourceCustomer.CustId,
                UpLineId = actor.CustId,
                ActorId = actor.CustId,
                SubAccId = param.ActorSubAccId,
                Ip = param.IP,
                Language = param.LanguageKey
            };

            return settingService.UpdateSettingFromExistingMember(criteria);
        }

        private ICommandResults UpdateGameSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CloneCustomerParam param)
        {
            var editSettingViewModel = new EditSettingViewModel
            {
                CustId = sourceCustomer.CustId,
                UpLineId = actor.CustId,
                Level = CustomerLevel.Member,
                UpLineLevel = CustomerLevel.Agent,
                ActorId = actor.CustId,
                Language = param.Language,
                FormType = FormType.Edit
            };

            var settingParams = Mapper.Map<EditSettingViewModel, GetSettingParameters>(editSettingViewModel);
            var settingDTO = settingService.GetSetting(settingParams);

            var criteria = new UpdateGameSettingParameters
            {
                ActorId = actor.CustId,
                CustId = createdCustomer.CustId,
                GameSetting = settingDTO.GameSetting,
                Ip = param.IP,
                Language = param.Language
            };

            return settingService.UpdateGameSetting(criteria);
        }

        private ICommandResults UpdateStatus(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CloneCustomerParam param)
        {
            var criteria = new UpdateDisableStatusParameters
            {
                CustId = createdCustomer.CustId,
                ActorId = actor.CustId,
                SubAccId = param.ActorSubAccId,
                Disable = sourceCustomer.DisableGoldDeluxe,
                Ip = param.IP
            };

            return settingService.UpdateStatus(criteria);
        }
    }
}
