using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public class MutableUserService : MutableBasicService<IdentityUser>, IMutableUserService
    {
        private readonly IUserStore _store;
        private readonly IUserPasswordEncryptService _encryptService;
        public MutableUserService(
            IUserStore store,
            IUserPasswordEncryptService encryptService) : base(store)
        {
            this._store = store;
            this._encryptService = encryptService;
        }

        public async Task ChangeMainOrganizationAsync(Guid userId, Guid newMainOrganizationId)
        {
            var originMainOrganization = await _store.OrganizationUsers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsPrimary)
                .ConfigureAwait(false);
            if (originMainOrganization == null)
                throw new KeyNotFoundException("找不到用户的主要组织");

            if (originMainOrganization.OrganizationId == newMainOrganizationId)
                return;

            originMainOrganization.IsPrimary = false;
            _store.OrganizationUsers.Update(originMainOrganization);

            var existsRelationOfNewMainOrganization = await _store.OrganizationUsers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.OrganizationId == newMainOrganizationId)
                .ConfigureAwait(false);
            if (existsRelationOfNewMainOrganization != null)
            {
                existsRelationOfNewMainOrganization.IsPrimary = true;
                _store.OrganizationUsers.Update(existsRelationOfNewMainOrganization);
            }
            else
            {
                var maxOrder = 0;
                if (await _store.OrganizationUsers.AnyAsync(x => x.OrganizationId == newMainOrganizationId).ConfigureAwait(false))
                {
                    maxOrder = await _store.OrganizationUsers
                        .Where(x => x.OrganizationId == newMainOrganizationId)
                        .MaxAsync(x => x.Order)
                        .ConfigureAwait(false);
                }

                _store.OrganizationUsers.Add(new IdentityOrganizationUser(
                    newMainOrganizationId,
                    userId,
                    maxOrder + 1,
                    isPrimary: true));
            }

            await _store.SaveChangesAsync().ConfigureAwait(false);
        }

        public override async Task<IdentityUser> CreateAsync(IdentityUser entity)
        {
            Check.NotNull(entity, nameof(entity));
            Check.NotEmpty(entity.AccountName, nameof(entity.AccountName));

            if (await _store.Users.AnyAsync(x => x.AccountName == entity.AccountName).ConfigureAwait(false))
                throw new InvalidOperationException($"{entity.AccountName} 已存在");

            if (!string.IsNullOrWhiteSpace(entity.Password))
                entity.Password = this._encryptService.Encrypt(entity.Password);

            return await base.CreateAsync(entity).ConfigureAwait(false);
        }

        public override async Task<IdentityUser> DeleteAsync(Guid id)
        {
            var organizationUsers = _store.OrganizationUsers.Where(x => x.UserId == id);
            _store.OrganizationUsers.RemoveRange(organizationUsers);

            return await base.DeleteAsync(id).ConfigureAwait(false);
        }

        public override async Task<IdentityUser> UpdateAsync(Guid id, IDictionary<string, object> updateProperties)
        {
            Check.NotNull(updateProperties,nameof(updateProperties));

            if (updateProperties.ContainsKey(nameof(IdentityUser.Password)))
            {
                var password = updateProperties[nameof(IdentityUser.Password)];
                if (password != null && !string.IsNullOrWhiteSpace(password.ToString()))
                    updateProperties[nameof(IdentityUser.Password)] = this._encryptService.Encrypt(password.ToString());
            }

            return await base.UpdateAsync(id, updateProperties).ConfigureAwait(false);
        }

    }
}