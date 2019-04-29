using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public interface IUserStore
    {
        DbSet<IdentityOrganization> Organizations { get; set; }
        DbSet<IdentityUser> Users { get; set; }
        DbSet<IdentityOrganizationUser> OrganizationUsers { get; set; }
        DbSet<IdentityToken> Tokens { get; set; }

        DbContext Context { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}