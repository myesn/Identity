using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Upo.Identity.Admin;

namespace Upo.Identity.Tests
{
    public class Basic
    {
        protected IMutableOrganizationService OrganizationService { get; }
        protected IMutableUserService UserService { get; }
        protected IOrganizationProvider OrganizationProvider { get; }
        protected IUserProvider UserProvider { get; }
        protected IUserPasswordEncryptService EncryptService { get; }

        public Basic()
        {
            var services = new ServiceCollection();

            services
                .AddUserEntityFrameworkInMemory("userdb")
                .AddUserProviders()
                .AddUserServices();

            var provider = services.BuildServiceProvider(validateScopes: true).CreateScope().ServiceProvider;
            this.OrganizationService = provider.GetRequiredService<IMutableOrganizationService>();
            this.UserService = provider.GetRequiredService<IMutableUserService>();
            this.OrganizationProvider = provider.GetRequiredService<IOrganizationProvider>();
            this.UserProvider = provider.GetRequiredService<IUserProvider>();
            this.EncryptService = provider.GetRequiredService<IUserPasswordEncryptService>();
        }
    }
}
