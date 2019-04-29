using Upo.Identity.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public class OrganizationProvider : BasicProvider<IdentityOrganization>, IOrganizationProvider
    {
        private readonly IUserStore _store;
        public OrganizationProvider(IUserStore store) : base(store)
        {
            this.EntityType = typeof(IdentityOrganization);
            this._store = store;
        }

        public async Task<PagingResult> PagingUsersAsync(Guid organizationId, PagingContext context)
        {
            context = context ?? new PagingContext();

            var ous = _store.OrganizationUsers
                .OrderBy(x => x.Order)
                .Where(x => x.OrganizationId == organizationId)
                .Select(x => new
                {
                    x.User.Id,
                    x.User.Name,
                    x.User.AccountName,
                    x.User.Email,
                    x.User.Mobile,
                    x.User.Gender,
                    x.User.IsEnabled,
                    CreateTime = x.User.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            var pageModel = new PagingResult
            {
                PageIndex = context.PageIndex,
                PageSize = context.PageSize,
                TotalCount = await ous.CountAsync().ConfigureAwait(false)
            };
            pageModel.TotalPage = (pageModel.TotalCount + pageModel.PageSize - 1) / pageModel.PageSize;
            pageModel.HasPrevious = pageModel.PageIndex > 0;
            pageModel.HasNext = pageModel.PageIndex < pageModel.TotalPage - 1;
            pageModel.Value = await ous
                 .Skip(context.PageIndex * context.PageSize)
                 .Take(context.PageSize)
                 .ToListAsync()
                 .ConfigureAwait(false);

            return pageModel;
        }

        public async Task<IEnumerable<IdentityOrganization>> GetChildrenAsync(Guid? parentId)
        {
            return await Query()
                .OrderBy(x => x.Order)
                .Where(x => x.ParentId == parentId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}