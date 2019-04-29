using Upo.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UserProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddUserProviders(this IServiceCollection services)
        {
            return services
                .AddScoped<IUserPasswordValidator, UserPasswordValidator>()
                .AddScoped<IUserProvider, UserProvider>()
                .AddScoped<IOrganizationProvider, OrganizationProvider>();
        }
    }
}