using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public class MutableOrganizationService : MutableBasicService<IdentityOrganization>, IMutableOrganizationService
    {
        private readonly IUserStore _store;
        private readonly IMutableUserService _mutableUserService;
        private readonly IOrganizationProvider _organizationProvider;
        public MutableOrganizationService(
            IUserStore store,
            IMutableUserService mutableUserService,
            IOrganizationProvider organizationProvider)
            : base(store)
        {
            this._store = store;
            this._mutableUserService = mutableUserService;
            this._organizationProvider = organizationProvider;
        }

        public override async Task<IdentityOrganization> UpdateAsync(Guid id, IDictionary<string, object> updateProperties)
        {
            Check.NotNull(updateProperties, nameof(updateProperties));

            if (updateProperties.ContainsKey(nameof(IdentityOrganization.Name)))
            {
                var updateName = updateProperties[nameof(IdentityOrganization.Name)].ToString();
                Check.NotEmpty(updateName, nameof(updateName));

                var parentId = await this._organizationProvider.Query()
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => x.ParentId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (await this._organizationProvider.Query()
                    .AnyAsync(x => x.Id != id && x.ParentId == parentId && x.Name == updateName).ConfigureAwait(false))
                    throw new InvalidOperationException($"{updateName} 组织已存在");
            }

            return await base.UpdateAsync(id, updateProperties).ConfigureAwait(false);
        }

        public override async Task<IdentityOrganization> CreateAsync(IdentityOrganization entity)
        {
            Check.NotNull(entity, nameof(entity));
            Check.NotEmpty(entity.Name, nameof(entity.Name));

            if (await this._organizationProvider.Query()
                .AnyAsync(x => x.ParentId == entity.ParentId && x.Name == entity.Name)
                .ConfigureAwait(false))
                throw new InvalidOperationException($"{entity.Name} 组织已存在");

            int maxOrder = 0;
            if (await this._store.Organizations.AnyAsync(x => x.ParentId == entity.ParentId).ConfigureAwait(false))
                maxOrder = await this._store.Organizations.AsNoTracking()
                    .Where(x => x.ParentId == entity.ParentId)
                    .MaxAsync(x => x.Order).ConfigureAwait(false);

            entity.Order = maxOrder + 1;

            return await base.CreateAsync(entity).ConfigureAwait(false);
        }

        public override async Task<IdentityOrganization> DeleteAsync(Guid id)
        {
            if (await this._store.Organizations.CountAsync(x => x.ParentId == id).ConfigureAwait(false) > 1)
                throw new Exception("当前组织下还有子组织，不能删除");

            if (await this._store.OrganizationUsers.AnyAsync(x => x.OrganizationId == id).ConfigureAwait(false))
                throw new Exception("组织下还有用户，不能删除");

            return await base.DeleteAsync(id).ConfigureAwait(false);
        }

        public async Task<IdentityUser> AddUserAsync(Guid organizationId, IdentityUser user)
        {
            await this._mutableUserService.CreateAsync(user).ConfigureAwait(false);
            await AddUserAsync(organizationId, user.Id, true).ConfigureAwait(false);
            return user;
        }

        public async Task AddUserAsync(Guid organizationId, Guid userId)
        {
            await AddUserAsync(organizationId, userId, false).ConfigureAwait(false);
        }

        public async Task RemoveUserAsync(Guid organizationId, Guid userId)
        {
            var ou = await _store.OrganizationUsers
                .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId)
                .ConfigureAwait(false);
            if (ou == null)
                return;

            _store.OrganizationUsers.Remove(ou);
            await _store.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveUsersAsync(Guid organizationId, Guid[] userIds)
        {
            var ous = _store.OrganizationUsers.Where(x => x.OrganizationId == organizationId && userIds.Contains(x.UserId));
            _store.OrganizationUsers.RemoveRange(ous);
            await _store.SaveChangesAsync().ConfigureAwait(false);
        }


        private async Task AddUserAsync(Guid organizationId, Guid userId, bool isPrimary)
        {
            if (await _store.OrganizationUsers
                .AnyAsync(x => x.OrganizationId == organizationId && x.UserId == userId)
                .ConfigureAwait(false))
                return;

            var maxOrder = 0;
            if (await _store.OrganizationUsers.AnyAsync(x => x.OrganizationId == organizationId).ConfigureAwait(false))
            {
                maxOrder = await _store.OrganizationUsers
                    .Where(x => x.OrganizationId == organizationId)
                    .MaxAsync(x => x.Order).ConfigureAwait(false);
            }

            _store.OrganizationUsers.Add(new IdentityOrganizationUser(
              organizationId,
              userId,
              maxOrder + 1,
              isPrimary: isPrimary));

            await _store.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}