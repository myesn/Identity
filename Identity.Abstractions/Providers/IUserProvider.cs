using Upo.Identity.Admin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public interface IUserProvider : IBasicProvider<IdentityUser>
    {
        Task<byte[]> GetAvatarAsync(Guid id);
        Task<IdentityUser> FindByUsernameAsync(string accountname);
        Task<IdentityOrganization> GetMainOrganizationAsync(Guid userId);
    }
}