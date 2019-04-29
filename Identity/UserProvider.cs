using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Upo.Identity.Admin;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Upo.Identity
{
    public class UserProvider : BasicProvider<IdentityUser>, IUserProvider
    {
        private readonly IUserStore _store;
        public UserProvider(IUserStore store) : base(store)
        {
            this.EntityType = typeof(IdentityUser);
            this._store = store;
        }

        public async Task<IdentityUser> FindByUsernameAsync(string accountname)
        {
            return await Query().FirstOrDefaultAsync(x => x.AccountName == accountname).ConfigureAwait(false);
        }

        public async Task<byte[]> GetAvatarAsync(Guid id)
        {
            return await Query().Where(x => x.Id == id).Select(x => x.Avatar).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IdentityOrganization> GetMainOrganizationAsync(Guid userId)
        {
            var mainOrganization = await _store.OrganizationUsers
                .Include(x => x.Organization)
                .Where(x => x.UserId == userId && x.IsPrimary)
                .Select(x => x.Organization)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (mainOrganization == null)
                throw new KeyNotFoundException($"未找到用户 {userId} 的主要组织");

            return mainOrganization;
        }
    }
}