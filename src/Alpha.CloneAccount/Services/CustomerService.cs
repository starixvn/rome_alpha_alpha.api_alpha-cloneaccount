namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Constants;
    using Criteria;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;
    using Params;
    using Utils;

    public interface CustomerService
    {
        Task<Result> ValidateMaxNewMember(int agentId, string language);

        Task<CustomerPTInfo> GetPTInfo(int custId, UserLevel custLevel, string site);

        Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param);
    }

    public class CustomerServiceImpl : CustomerService
    {
        private readonly IDynamicRepository repository;
        private readonly PasswordService passwordService;
        private readonly SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
        private readonly ThirdPartyProductService thirdPartyProductService;

        public CustomerServiceImpl(
            IDynamicRepository repository,
            PasswordService passwordService,
            SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder,
            ThirdPartyProductService thirdPartyProductService)
        {
            this.repository = repository;
            this.passwordService = passwordService;
            this.copyMemberSpParamsBuilder = copyMemberSpParamsBuilder;
            this.thirdPartyProductService = thirdPartyProductService;
        }

        public async Task<Result> ValidateMaxNewMember(int agentId, string language)
        {
            var criteria = new ValidateMaxNewMemberCriteria(agentId, LanguageUtils.ToLanguageKey(language));
            await repository.ExecuteAsync(criteria);

            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public async Task<CustomerPTInfo> GetPTInfo(int custId, UserLevel custLevel, string site)
        {
            var criteria = GetCustomerPTCriteria(custId, custLevel, site);
            var customerPTInfo = await repository.GetAsync<CustomerPTInfo>(criteria);

            if (customerPTInfo != null)
            {
                customerPTInfo.CustId = custId;
                customerPTInfo.CustLevel = custLevel.Value;
            }

            return customerPTInfo;
        }

        private static ICriteria GetCustomerPTCriteria(int custId, UserLevel custLevel, string site)
        {
            if (custLevel.IsAgentLevel())
            {
                return new GetAgentPTCriteria(custId);
            }

            if (custLevel.IsMasterLevel())
            {
                return new GetMasterPTCriteria(custId);
            }

            return new GetSuperPTCriteria(custId, site);
        }

        public async Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param)
        {
            CheckAndGenerateRawPassword(param);

            var result = param.ActorUserLevel.IsAgentLevel()
                ? await CopyMember(param)
                : await CopyMasterOrAgent(param);

            if (result?.IsSuccess == true)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await Sync3rdProductSettings(param, result.CustId);
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            return result;
        }

        private void CheckAndGenerateRawPassword(CopyCustomerParam param)
        {
            if (IsFromChatBotService(param.SystemId)
                && string.IsNullOrWhiteSpace(param.Password))
            {
                param.SetPassword(passwordService.Generate(8, true, true, true, true));
            }
        }

        private static bool IsFromChatBotService(byte systemId)
            => systemId == SystemIdentity.Chatbot;

        private async Task<CopyCustomerResult> CopyMasterOrAgent(CopyCustomerParam param)
        {
            var criteria = BuildCopyMasterOrAgentCriteria(param);

            await repository.ExecuteAsync(criteria);

            return new CopyCustomerResult(
                criteria.CustId,
                param.UserName,
                param.Password,
                param.ActorUserLevel.GetDirectDownLineLevel().Value,
                param.SourceUsername,
                param.Credit,
                param.MaxCredit,
                criteria.ErrCode,
                criteria.ErrMsg);
        }

        private CopyCustomerCommand BuildCopyMasterOrAgentCriteria(CopyCustomerParam param)
        {
            if (param.ActorUserLevel.IsMasterLevel())
            {
                return new CopyAgentCommand(
                    param.UserName,
                    passwordService.CfsEnCode(param.Password),
                    param.FirstName,
                    param.LastName,
                    param.Phone,
                    param.MobilePhone,
                    param.Fax,
                    param.Credit,
                    param.MaxCredit,
                    param.ActorId,
                    param.ActorId,
                    param.SourceCustId,
                    param.ActorSubAccId,
                    param.Language,
                    param.IP,
                    param.SystemId);
            }

            return new CopyMasterCommand(
                param.UserName,
                passwordService.CfsEnCode(param.Password),
                param.FirstName,
                param.LastName,
                param.Phone,
                param.MobilePhone,
                param.Fax,
                param.Credit,
                param.ActorId,
                param.ActorId,
                param.SourceCustId,
                param.ActorSubAccId,
                param.Language,
                param.IP,
                param.SystemId);
        }

        private async Task<CopyCustomerResult> CopyMember(CopyCustomerParam param)
        {
            var dataTable = await GetMemberSportbooksSetting(param.SourceCustId);

            if (dataTable.Rows.Count > 0)
            {
                using (IObjectDb db = new ObjectDb("Acc_CustomerInsMember_Ag"))
                {
                    var spParams = copyMemberSpParamsBuilder.BuildSpParams(
                        dataTable.Rows[0],
                        param,
                        passwordService.CfsEnCode(param.Password));

                    await db.ExecuteNonQueryAsync(spParams);
                    var ouputParams = db.GetOuputParams();

                    return new CopyCustomerResult(
                        Convert.ToInt32(ouputParams["custid"]),
                        param.UserName,
                        param.Password,
                        param.ActorUserLevel.GetDirectDownLineLevel().Value,
                        param.SourceUsername,
                        param.Credit,
                        param.MaxCredit,
                        Convert.ToInt32(ouputParams["errcode"]),
                        ouputParams["errmsg"].ToString());
                }
            }

            return null;
        }

        private static async Task<DataTable> GetMemberSportbooksSetting(int custId)
        {
            ICriteria criteria = new GetMemberSportbooksSettingCriteria(custId);

            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                var dataSet = await db.ExecuteDataSetAsync(criteria.GetSpParams());

                return dataSet?.Tables?.Count == 1
                    ? dataSet.Tables[0]
                    : new DataTable();
            }
        }

        public async Task Sync3rdProductSettings(CopyCustomerParam param, int createdCustId)
        {
            var createdCustomer = await GetCustomerInfo(createdCustId);
            var sourceCustomer = await GetCustomerInfo(param.SourceCustId);
            var actor = await GetCustomerInfo(param.ActorId);

            if (createdCustomer != null && sourceCustomer != null && actor != null)
            {
                if (param.ActorUserLevel.IsAgentLevel())
                {
                    await thirdPartyProductService.SyncMemberSettings(
                        createdCustomer,
                        sourceCustomer,
                        actor,
                        param);
                }
                else
                {
                    await thirdPartyProductService.SyncMasterOrAgentSettings(
                        createdCustomer,
                        sourceCustomer,
                        actor,
                        param);
                }
            }
        }

        private async Task<CustomerInfo> GetCustomerInfo(int custId)
            => await repository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria(custId));
    }
}