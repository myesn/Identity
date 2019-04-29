using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Upo.Identity.Admin;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UserServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            return services                
                .AddScoped<IMutableUserService, MutableUserService>()
                .AddScoped<IMutableOrganizationService, MutableOrganizationService>()
                .AddSingleton<IUserPasswordEncryptService, UserPasswordBase64EncryptService>();
        }
    }
}