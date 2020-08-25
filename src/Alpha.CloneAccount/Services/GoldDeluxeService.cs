namespace Alpha.CloneAccount.Services
{
    using System;
    using AutoMapper;
    using Fanex.GoldDeluxe.CoreSetting.CommandResult;
    using Fanex.GoldDeluxe.CoreSetting.DTOs;
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
            int custId,
            int sourceCustId,
            bool disableGoldDeluxe,
            CopyCustomerParam param);
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
            int custId,
            int sourceCustId,
            bool disableGoldDeluxe,
            CopyCustomerParam param)
        {
            try
            {
                // Update WinLimit, PT and Comm.
                var criteria = new UpdateSettingFromExistingMemberParameters
                {
                    CustId = custId,
                    FromCustId = sourceCustId,
                    UpLineId = param.ActorId,
                    ActorId = param.ActorId,
                    SubAccId = param.ActorSubAccId,
                    Ip = param.IP,
                    Language = param.LanguageKey
                };

                var result = UpdateSettingFromExistingMember(criteria);
                var commandResults = result.CommandResultsList;

                var editSettingViewModel = new EditSettingViewModel
                {
                    CustId = sourceCustId,
                    UpLineId = param.ActorId,
                    Level = CustomerLevel.Member,
                    UpLineLevel = CustomerLevel.Agent,
                    ActorId = param.ActorId,
                    Language = param.Language,
                    FormType = FormType.Edit
                };

                var getSettingParams = Mapper.Map<EditSettingViewModel, GetSettingParameters>(editSettingViewModel);
                var settingDTO = GetSourceMemberSetting(getSettingParams);

                var updateSettingParam = new UpdateGameSettingParameters
                {
                    ActorId = param.ActorId,
                    CustId = custId,
                    GameSetting = settingDTO.GameSetting,
                    Ip = param.IP,
                    Language = param.Language
                };

                commandResults.Add(UpdateGameSetting(updateSettingParam));

                if (result.IsSuccessful)
                {
                    // Update Status.
                    var statusParam = new UpdateDisableStatusParameters
                    {
                        CustId = custId,
                        ActorId = param.ActorId,
                        SubAccId = param.ActorSubAccId,
                        Disable = disableGoldDeluxe,
                        Ip = param.IP,
                        SystemId = param.SystemId,
                        IsSC = true
                    };

                    commandResults.Add(UpdateStatus(statusParam));
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                return new MultipleCommandResults();
            }
        }

        private IMultipleCommandResults UpdateSettingFromExistingMember(
            UpdateSettingFromExistingMemberParameters criteria)
            => settingService.UpdateSettingFromExistingMember(criteria);

        private SettingDTO GetSourceMemberSetting(GetSettingParameters settingParam)
            => settingService.GetSetting(settingParam);

        private ICommandResults UpdateGameSetting(UpdateGameSettingParameters settingParam)
            => settingService.UpdateGameSetting(settingParam);

        private ICommandResults UpdateStatus(UpdateDisableStatusParameters statusParam)
            => settingService.UpdateStatus(statusParam);
    }
}
