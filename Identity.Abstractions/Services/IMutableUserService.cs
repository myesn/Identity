using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public interface IMutableUserService : IMutableBasicService<IdentityUser>
    {
        Task ChangeMainOrganizationAsync(Guid userId, Guid newMainOrganizationId);
    }
}