using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public interface IMutableOrganizationService : IMutableBasicService<IdentityOrganization>
    {
        Task<IdentityUser> AddUserAsync(Guid organizationId, IdentityUser user);
        Task AddUserAsync(Guid organizationId, Guid userId);
        Task RemoveUserAsync(Guid organizationId, Guid userId);
        Task RemoveUsersAsync(Guid organizationId, Guid[] userIds);
    }
}