using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public interface IOrganizationProvider : IBasicProvider<IdentityOrganization>
    {
        Task<IEnumerable<IdentityOrganization>> GetChildrenAsync(Guid? parentId);
        Task<PagingResult> PagingUsersAsync(Guid organizationId, PagingContext context = null);
    }
}