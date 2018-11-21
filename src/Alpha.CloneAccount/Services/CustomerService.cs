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

    public interface CustomerService
    {
        Task<CustomerPTInfo> GetPTInfo(int custId, UserLevel custLevel, string site);

        Task<CloneCustomerResult> CloneCustomer(CloneCustomerParam param);
    }

    public class CustomerServiceImpl : CustomerService
    {
        private readonly IDynamicRepository repository;
        private readonly PasswordService passwordService;
        private readonly SportbooksCloneMemberSpParamsBuilder cloneMemberSpParamsBuilder;
        private readonly ThirdPartyProductService thirdPartyProductService;

        public CustomerServiceImpl(
            IDynamicRepository repository,
            PasswordService passwordService,
            SportbooksCloneMemberSpParamsBuilder cloneMemberSpParamsBuilder,
            ThirdPartyProductService thirdPartyProductService)
        {
            this.repository = repository;
            this.passwordService = passwordService;
            this.cloneMemberSpParamsBuilder = cloneMemberSpParamsBuilder;
            this.thirdPartyProductService = thirdPartyProductService;
        }

        public async Task<CustomerPTInfo> GetPTInfo(int custId, UserLevel custLevel, string site)
        {
            var criteria = GetCustomerPTCriteria(custId, custLevel, site);
            var customerPTInfo = await repository.GetAsync<CustomerPTInfo>(criteria);
            customerPTInfo.CustId = custId;
            customerPTInfo.CustLevel = custLevel.Value;

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

        public async Task<CloneCustomerResult> CloneCustomer(CloneCustomerParam param)
        {
            CheckAndGenerateRawPassword(param);

            var result = param.ActorUserLevel.IsAgentLevel()
                ? await CloneMember(param)
                : await CloneMasterOrAgent(param);

            if (result?.IsSuccess == true)
            {
                await Sync3rdProductSettings(param, result.CustId);
            }

            return result;
        }

        private void CheckAndGenerateRawPassword(CloneCustomerParam param)
        {
            if (IsFromChatBotService(param.SystemId)
                && string.IsNullOrWhiteSpace(param.Password))
            {
                param.SetPassword(passwordService.Generate(8, true, true, true, true));
            }
        }

        private static bool IsFromChatBotService(byte systemId)
            => systemId == SystemIdentity.Chatbot;

        private async Task<CloneCustomerResult> CloneMasterOrAgent(CloneCustomerParam param)
        {
            var criteria = BuildCloneMasterOrAgentCriteria(param);

            await repository.ExecuteAsync(criteria);

            return new CloneCustomerResult(
                criteria.CustId,
                param.UserName,
                param.Password,
                criteria.ErrCode,
                criteria.ErrMsg);
        }

        private CloneCustomerCommand BuildCloneMasterOrAgentCriteria(CloneCustomerParam param)
        {
            if (param.ActorUserLevel.IsMasterLevel())
            {
                return new CloneAgentCommand(
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

            return new CloneMasterCommand(
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

        private async Task<CloneCustomerResult> CloneMember(CloneCustomerParam param)
        {
            var dataTable = await GetMemberSportbooksSetting(param.SourceCustId);

            if (dataTable.Rows.Count > 0)
            {
                using (IObjectDb db = new ObjectDb("Acc_CustomerInsMember_Ag"))
                {
                    var spParams = cloneMemberSpParamsBuilder.BuildSpParams(
                        dataTable.Rows[0],
                        param,
                        passwordService.CfsEnCode(param.Password));

                    await db.ExecuteNonQueryAsync(spParams);
                    var ouputParams = db.GetOuputParams();

                    return new CloneCustomerResult(
                        Convert.ToInt32(ouputParams["custid"]),
                        param.UserName,
                        param.Password,
                        Convert.ToInt32(ouputParams["errcode"]),
                        ouputParams["errmsg"].ToString());
                }
            }

            return null;
        }

        private async Task<DataTable> GetMemberSportbooksSetting(int custId)
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

        public async Task Sync3rdProductSettings(CloneCustomerParam param, int createdCustId)
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
