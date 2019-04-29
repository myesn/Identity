using Upo.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Upo.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UserEntityFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddUserEntityFrameworkSqlServer(
            this IServiceCollection services,
            string sqlConnectionString,
            string migrationsAssemblyName
            )
        {
            return services
                 .AddDbContext<IdentityDbContext>(builder =>
                     builder.UseMySql(sqlConnectionString, options =>
                          options.MigrationsAssembly(migrationsAssemblyName)))
                 .AddScoped<IUserStore, IdentityDbContext>();
        }

        public static IServiceCollection AddUserEntityFrameworkInMemory(
            this IServiceCollection services,
            string databaseName
            )
        {
            return services
                 .AddDbContext<IdentityDbContext>(builder =>
                     builder.UseInMemoryDatabase(databaseName))
                 .AddScoped<IUserStore, IdentityDbContext>();
        }
    }
}