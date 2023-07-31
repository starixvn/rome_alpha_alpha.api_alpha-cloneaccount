using Alpha.CloneAccount.Configurations;
using Fanex.Data;

namespace Alpha.CloneAccount.Api.Utilities
{
    public static class DbSettingProviderManagerExtension
    {   
        public static DbSettingProviderManager UseShardDbSettingProvider(this DbSettingProviderManager dbSettingProviderManager)
            => dbSettingProviderManager.Use(new ShardDbSettingProvider(new ShardInfoAccessor(),
                    resourcePath: AppSettings.StoredProcedurePath));

    }
}