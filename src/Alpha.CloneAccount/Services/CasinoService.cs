namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Criteria;
    using DNAide.Core;
    using Fanex.CustomerSetting.Module.Casino.Configurations;
    using Fanex.CustomerSetting.Module.Casino.Services.Interface;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Models;
    using Params;
    using Utils;

    public interface CasinoService
    {
        Task SyncMasterOrAgentSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param);

        Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param,
            bool disableAutoMasterPT);
    }

    public class CasinoServiceImpl : CasinoService
    {
        private const string CasinoGroup1 = "01";
        private const int CasinoFromSystem = 2; // Alpha.

        private readonly IDynamicRepository repository;
        private readonly MemberWinLimitService memberWinLimitService;
        private readonly ILogger logger;

        public CasinoServiceImpl(
            IDynamicRepository repository,
            MemberWinLimitService memberWinLimitService,
            ILogger logger)
        {
            this.repository = repository;
            this.memberWinLimitService = memberWinLimitService;
            this.logger = logger;
        }

        public async Task SyncMasterOrAgentSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            if (actor.SyncCasino && sourceCustomer.SyncCasino && !CasinoSection.Instance.IsUMTime)
            {
                try
                {
                    Customer customer = new Customer();

                    if (customer.AutoInsertAccountFromExisting(
                        CasinoGroup1,
                        createdCustomer.CustId,
                        sourceCustomer.CustId,
                        createdCustomer.UserName,
                        createdCustomer.UserName2,
                        createdCustomer.FirstName,
                        createdCustomer.LastName,
                        createdCustomer.UserPwd,
                        createdCustomer.Phone,
                        createdCustomer.MobilePhone,
                        createdCustomer.Fax,
                        DateTime.Now,
                        param.IP,
                        actor.CustId,
                        actor.UserName,
                        CasinoFromSystem))
                    {
                        await UpdateCasinoStatus(
                            createdCustomer.CustId,
                            sourceCustomer.DisableCasino,
                            true,
                            true,
                            actor,
                            param);
                    }
                    else
                    {
                        customer.RemoveAccount(
                            createdCustomer.UserName,
                            param.IP,
                            actor.CustId,
                            actor.UserName,
                            CasinoFromSystem);
                    }
                }
                catch (AggregateException ex)
                {
                    logger.Warn(ex.Message);
                }
                catch (ErrorException ex)
                {
                    logger.Info(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message);
                }
            }
        }

        private async Task UpdateCasinoStatus(
            int custId,
            bool disableCasino,
            bool syncCasino,
            bool isUpdateDownline,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            var command = new UpdateCasinoStatusCriteria(
                custId,
                disableCasino,
                syncCasino,
                isUpdateDownline,
                param.SystemId,
                param.IP,
                param.LanguageKey,
                param.ActorId,
                param.ActorSubAccId);

            await repository.ExecuteAsync(command);
            var isSuccessful = command.Status;

            if (isSuccessful)
            {
                var customer = new Customer();

                customer.UpdateCustEnableCasino(
                    custId,
                    !disableCasino,
                    isUpdateDownline,
                    param.IP,
                    actor.CustId,
                    actor.UserName,
                    CasinoFromSystem);
            }
        }

        public async Task SyncMemberSettings(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param,
            bool disableAutoMasterPT)
        {
            if (actor.SyncCasino && sourceCustomer.SyncCasino && !CasinoSection.Instance.IsUMTime)
            {
                try
                {
                    var enableBonus = false;
                    var enableJackpot = false;
                    var isEnablePT = true; //GetIsEnablePT(sourceCustomer.CustId);

                    var customer = new Customer();
                    var isSuccessful = customer.AutoInsertAccount(
                        CasinoGroup1,
                        createdCustomer.CustId,
                        createdCustomer.UserName,
                        createdCustomer.UserName2,
                        createdCustomer.FirstName,
                        createdCustomer.LastName,
                        createdCustomer.UserPwd,
                        createdCustomer.Phone,
                        createdCustomer.MobilePhone,
                        createdCustomer.Currency,
                        createdCustomer.Fax,
                        createdCustomer.Closed,
                        createdCustomer.Suspend,
                        isEnablePT,
                        createdCustomer.SuperId,
                        createdCustomer.MasterId,
                        createdCustomer.AgentId,
                        createdCustomer.RoleId.ToString(),
                        enableBonus,
                        enableJackpot,
                        !disableAutoMasterPT,
                        DateTime.Now,
                        param.IP,
                        actor.CustId,
                        actor.UserName,
                        CasinoFromSystem);

                    if (isSuccessful)
                    {
                        await UpdateCasinoStatus(
                            createdCustomer.CustId,
                            sourceCustomer.DisableCasino,
                            true,
                            true,
                            actor,
                            param);
                        UpdatePositionTaking(createdCustomer, sourceCustomer, actor, param);
                    }
                    else
                    {
                        await UpdateCasinoStatus(
                            createdCustomer.CustId,
                            actor.DisableCasino,
                            false,
                            false,
                            actor,
                            param);
                        customer.RemoveAccount(createdCustomer.UserName, param.IP, actor.CustId, actor.UserName, CasinoFromSystem);
                    }
                }
                catch (AggregateException ex)
                {
                    logger.Warn(ex.Message);
                }
                catch (ErrorException ex)
                {
                    logger.Info(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex.Message);
                }
            }
        }

        private bool GetIsEnablePT(int custId)
        {
            var customer = new Customer();
            var dtCustomer = customer.GetCustSetting(custId);

            return dtCustomer == null
                || dtCustomer.Rows.Count == 0
                || CastAide.AsBoolean(dtCustomer.Rows[0]["IsEnablePT"]);
        }

        private void UpdatePositionTaking(
            CustomerInfo createdCustomer,
            CustomerInfo sourceCustomer,
            CustomerInfo actor,
            CopyCustomerParam param)
        {
            var dtGameKinds = CreateGameKinds();
            var dtGameTypesStake = CreateGameTypesStakes();

            var lsGameKind = sourceCustomer.SyncCasino
                ? GetGameKinds(sourceCustomer.CustId)
                : GetGameKinds(actor.CustId);

            foreach (GameKindDetail gameKindDetail in lsGameKind)
            {
                dtGameKinds.Rows.Add(new object[] {
                    createdCustomer.CustId,
                    gameKindDetail.CodeID,
                    null,
                    null,
                    null,
                    (gameKindDetail.MaxPositionTakingUpline < gameKindDetail.APositionTaking)? gameKindDetail.MaxPositionTakingUpline : gameKindDetail.APositionTaking,
                    gameKindDetail.IsAutoPositionTaking,
                    !gameKindDetail.IsAutoPositionTaking? null : (object)gameKindDetail.AutoPositionTaking,
                    null,
                    0,
                    null,
                    null,
                    null,
                    gameKindDetail.MemberRollingRate,
                    sourceCustomer.RoleId,
                    param.IP,
                    actor.CustId,
                    param.ActorSubAccId == 0 ? actor.UserName : param.ActorSubAccName,
                    CasinoFromSystem
                });

                var lsGameType = sourceCustomer.SyncCasino
                    ? GetGameType(sourceCustomer.CustId, gameKindDetail.CodeID)
                    : GetGameType(gameKindDetail.CodeID, sourceCustomer.CustId, sourceCustomer.Currency);

                if (lsGameType != null && lsGameType.Count > 0)
                {
                    foreach (var item in lsGameType)
                    {
                        var selectedStakeLevels = string.Empty;
                        var lsStakeLevel = sourceCustomer.SyncCasino
                            ? GetCustStakeLevels(sourceCustomer.CustId, item.GameTypeID)
                            : GetCustStakeLevels(actor.CustId, item.GameTypeID);

                        foreach (var betItem in lsStakeLevel)
                        {
                            if (betItem.IsChecked)
                            {
                                selectedStakeLevels += betItem.StakeLevelID + ",";
                            }
                        }

                        if (selectedStakeLevels.EndsWith(","))
                        {
                            selectedStakeLevels = selectedStakeLevels.Remove(selectedStakeLevels.LastIndexOf(","), 1);
                        }

                        dtGameTypesStake.Rows.Add(new object[] {
                            createdCustomer.CustId,
                            item.GameTypeID,
                            selectedStakeLevels,
                            sourceCustomer.RoleId,
                            param.IP,
                            actor.CustId,
                            param.ActorSubAccId == 0 ? actor.UserName : param.ActorSubAccName,
                            CasinoFromSystem
                        });
                    }
                }
            }

            var customer = new Customer();
            customer.UpdateCustGameKind(dtGameKinds);
            customer.UpdateCustGameTypeStakeLevel(dtGameTypesStake);

            UpdateWinLimit(createdCustomer.CustId, sourceCustomer.CustId, param);
        }

        private DataTable CreateGameKinds()
        {
            DataTable dtGameKinds = new DataTable();

            dtGameKinds.Columns.AddRange(new[] {
                new DataColumn("IBCCustID"),
                new DataColumn("GameKind"),
                new DataColumn("HPositionTaking"),
                new DataColumn("SPositionTaking"),
                new DataColumn("MPositionTaking"),
                new DataColumn("APositionTaking"),
                new DataColumn("IsAutoPositionTaking"),
                new DataColumn("AutoPositionTaking"),
                new DataColumn("MinPositionTaking"),
                new DataColumn("MaxPositionTaking"),
                new DataColumn("SMAgentRollingRate"),
                new DataColumn("MAgentRollingRate"),
                new DataColumn("AgentRollingRate"),
                new DataColumn("MemberRollingRate"),
                new DataColumn("RoleId"),
                new DataColumn("FromIP"),
                new DataColumn("ModifyBy"),
                new DataColumn("ModifyUserName"),
                new DataColumn("FromSystem")
            });

            return dtGameKinds;
        }

        private DataTable CreateGameTypesStakes()
        {
            DataTable dtGameTypesStake = new DataTable();

            dtGameTypesStake.Columns.AddRange(new[] {
                new DataColumn("IBCCustID"),
                new DataColumn("GameTypeID"),
                new DataColumn("StakeLevelList"),
                new DataColumn("RoleId"),
                new DataColumn("FromIP"),
                new DataColumn("ModifyBy"),
                new DataColumn("ModifyUserName"),
                new DataColumn("FromSystem")
            });

            return dtGameTypesStake;
        }

        private List<GameKindDetail> GetGameKinds(int custId)
        {
            var lsGameKind = new List<GameKindDetail>();

            var customer = new Customer();
            var dtGameKind = customer.GetCustGameKind(custId);

            foreach (DataRow row in dtGameKind.Rows)
            {
                var gameKind = new GameKindDetail();

                gameKind.CodeID = CastAide.AsInt32(row["GameKind"]);
                gameKind.CodeName = CastAide.AsString(row[LanguageUtils.ToCasinoLanguage("GameKindName")]);
                gameKind.AgentRollingRate = CastAide.AsDouble(row["AgentRollingRate"]);
                gameKind.APositionTaking = CastAide.AsDouble(row["APositionTaking"]);
                gameKind.AutoPositionTaking = CastAide.AsDouble(row["AutoPositionTaking"]);
                gameKind.IsAutoPositionTaking = CastAide.AsBoolean(row["IsAutoPositionTaking"]);
                gameKind.MAgentRollingRate = CastAide.AsDouble(row["MAgentRollingRate"]);
                gameKind.MaxPositionTaking = CastAide.AsDouble(row["MaxPositionTaking"]);
                gameKind.MemberRollingRate = CastAide.AsDouble(row["MemberRollingRate"]);
                // gameKind.MinMPositionTaking = CastAide.AsDouble(row["MinMPositionTaking"]);
                gameKind.MinPositionTaking = CastAide.AsDouble(row["MinPositionTaking"]);
                gameKind.MPositionTaking = CastAide.AsDouble(row["MPositionTaking"]);
                gameKind.SMAgentRollingRate = CastAide.AsDouble(row["SMAgentRollingRate"]);
                gameKind.SPositionTaking = CastAide.AsDouble(row["SPositionTaking"]);
                gameKind.SuperSPositionTaking = CastAide.AsDouble(row["SuperSPositionTaking"]);

                gameKind.AgentRollingRateUpline = CastAide.AsDouble(row["UplineAgentRollingRate"]);
                gameKind.APositionTakingUpline = CastAide.AsDouble(row["UplineAPositionTaking"]);
                gameKind.AutoPositionTakingUpline = CastAide.AsDouble(row["UplineAutoPositionTaking"]);
                gameKind.IsAutoPositionTakingUpline = CastAide.AsBoolean(row["UplineIsAutoPositionTaking"]);
                gameKind.MAgentRollingRateUpline = CastAide.AsDouble(row["UplineMAgentRollingRate"]);
                gameKind.MaxPositionTakingUpline = CastAide.AsDouble(row["UplineMaxPositionTaking"]);
                gameKind.MemberRollingRateUpline = CastAide.AsDouble(row["UplineMemberRollingRate"]);
                // gameKind.MinMPositionTakingUpline = CastAide.AsDouble(row["MinMPositionTaking"]);
                gameKind.MinPositionTakingUpline = CastAide.AsDouble(row["UplineMinPositionTaking"]);
                gameKind.MPositionTakingUpline = CastAide.AsDouble(row["UplineMPositionTaking"]);
                gameKind.SMAgentRollingRateUpline = CastAide.AsDouble(row["UplineSMAgentRollingRate"]);
                gameKind.SPositionTakingUpline = CastAide.AsDouble(row["UplineSPositionTaking"]);

                lsGameKind.Add(gameKind);
            }

            return lsGameKind;
        }

        private List<GameTypeDetail> GetGameType(int custId, int gameKindID)
        {
            var lsGameType = new List<GameTypeDetail>();

            var customer = new Customer();
            var dtGameType = customer.GetCustGameType(custId, gameKindID);

            foreach (DataRow row in dtGameType.Rows)
            {
                var gameType = new GameTypeDetail();

                gameType.GameTypeID = CastAide.AsInt32(row["GameTypeID"]);
                gameType.GameTypeName = CastAide.AsString(row[LanguageUtils.ToCasinoLanguage("GameTypeName")]);
                gameType.AgentRollingRate = CastAide.AsDouble(row["AgentRollingRate"]);
                gameType.APositionTaking = CastAide.AsDouble(row["APositionTaking"]);
                gameType.AutoPositionTaking = CastAide.AsDouble(row["AutoPositionTaking"]);
                gameType.IsAutoPositionTaking = CastAide.AsBoolean(row["IsAutoPositionTaking"]);
                gameType.MAgentRollingRate = CastAide.AsDouble(row["MAgentRollingRate"]);
                gameType.MaxPositionTaking = CastAide.AsDouble(row["MaxPositionTaking"]);
                gameType.MemberRollingRate = CastAide.AsDouble(row["MemberRollingRate"]);
                //gameType.MinMPositionTaking = CastAide.AsDouble(row["MinMPositionTaking"]);
                gameType.MinPositionTaking = CastAide.AsDouble(row["MinPositionTaking"]);
                gameType.MPositionTaking = CastAide.AsDouble(row["MPositionTaking"]);
                gameType.SMAgentRollingRate = CastAide.AsDouble(row["SMAgentRollingRate"]);
                gameType.SPositionTaking = CastAide.AsDouble(row["SPositionTaking"]);

                gameType.AgentRollingRateUpline = CastAide.AsDouble(row["UplineAgentRollingRate"]);
                gameType.APositionTakingUpline = CastAide.AsDouble(row["UplineAPositionTaking"]);
                gameType.AutoPositionTakingUpline = CastAide.AsDouble(row["UplineAutoPositionTaking"]);
                gameType.IsAutoPositionTakingUpline = CastAide.AsBoolean(row["UplineIsAutoPositionTaking"]);
                gameType.MAgentRollingRateUpline = CastAide.AsDouble(row["UplineMAgentRollingRate"]);
                gameType.MaxPositionTakingUpline = CastAide.AsDouble(row["UplineMaxPositionTaking"]);
                gameType.MemberRollingRateUpline = CastAide.AsDouble(row["UplineMemberRollingRate"]);
                //gameType.MinMPositionTaking = CastAide.AsDouble(row["MinMPositionTaking"]);
                gameType.MinPositionTakingUpline = CastAide.AsDouble(row["UplineMinPositionTaking"]);
                gameType.MPositionTakingUpline = CastAide.AsDouble(row["UplineMPositionTaking"]);
                gameType.SMAgentRollingRateUpline = CastAide.AsDouble(row["UplineSMAgentRollingRate"]);
                gameType.SPositionTakingUpline = CastAide.AsDouble(row["UplineSPositionTaking"]);

                lsGameType.Add(gameType);
            }

            return lsGameType;
        }

        private List<GameTypeDetail> GetGameType(int gameKindID, int custId, int exchangeId)
        {
            var lsGameType = new List<GameTypeDetail>();

            var basicInfo = new BasicInfo();
            var dtGameType = basicInfo.GetGameType(gameKindID, custId, exchangeId);

            foreach (DataRow row in dtGameType.Rows)
            {
                var gameType = new GameTypeDetail();

                gameType.CasinoID = CastAide.AsInt32(row["CasinoID"]);
                gameType.GameTypeID = CastAide.AsInt32(row["GameTypeID"]);
                gameType.GameTypeName = CastAide.AsString(row[LanguageUtils.ToCasinoLanguage("GameTypeName")]);
                gameType.GameTypeNo = CastAide.AsString(row["GameTypeNo"]);
                gameType.GameKind = CastAide.AsInt32(row["GameKind"]);
                gameType.GameGroupID = CastAide.AsInt32(row["GameGroupID"]);
                gameType.FlashFileName = CastAide.AsString(row["FlashFileName"]);
                gameType.AlertMaxIdle = CastAide.AsInt32(row["AlertMaxIdle"]);
                gameType.IsTrialTime = CastAide.AsBoolean(row["IsTrialTime"]);
                gameType.MaxIdle = CastAide.AsInt32(row["MaxIdle"]);

                lsGameType.Add(gameType);
            }

            return lsGameType;
        }

        private List<StakeLevelDetail> GetCustStakeLevels(int custId, int gameTypeID)
        {
            var lsStakeLevel = new List<StakeLevelDetail>();

            var customer = new Customer();
            var dtStakeList = customer.GetCustStakeLevel(custId, gameTypeID);

            foreach (DataRow row in dtStakeList.Rows)
            {
                var stake = new StakeLevelDetail();

                stake.StakeLevelID = CastAide.AsInt32(row["StakeLevelID"]);
                stake.StakeLevelName = CastAide.AsString(row["StakeLevelName"]);
                stake.IsChecked = CastAide.AsBoolean(row["IsChecked"]);

                lsStakeLevel.Add(stake);
            }

            return lsStakeLevel;
        }

        private void UpdateWinLimit(
            int createdCustId,
            int sourceCustId,
            CopyCustomerParam param)
        {
            var memberWinLimitSetting = memberWinLimitService.GetMemberWinLimit(sourceCustId);

            memberWinLimitService.UpdateMemberWinLimit(
                createdCustId,
                param.ActorId,
                param.ActorSubAccId,
                param.IP,
                param.Language,
                memberWinLimitSetting);
        }
    }
}
