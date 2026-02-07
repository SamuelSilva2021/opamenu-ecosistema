using Authenticator.API.Infrastructure.Mapper.AccessControl.AccessGroup;
using Authenticator.API.Infrastructure.Mapper.AccessControl.Module;
using Authenticator.API.Infrastructure.Mapper.AccessControl.Roles;
using Authenticator.API.Infrastructure.Mapper.AccessControl.UserAccount;
using Authenticator.API.Infrastructure.Mapper.MultiTenant;
using Authenticator.API.Infrastructure.Mapper.Tenant;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace Authenticator.API.Infrastructure.Configurations
{
    /// <summary>  
    /// AutoMapper configuration class  
    /// </summary>  
    [ExcludeFromCodeCoverage]
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UserAccountProfile>();
                cfg.AddProfile<TenantProfile>();
                cfg.AddProfile<TenantProductProfile>();
                cfg.AddProfile<PlanProfile>();
                cfg.AddProfile<SubscriptionProfile>();
                cfg.AddProfile<AccessGroupProfile>();
                cfg.AddProfile<GroupTypeProfile>();
                cfg.AddProfile<ModuleProfile>();
                cfg.AddProfile<RoleProfile>();
            });
        }
    }
}
