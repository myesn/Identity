using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Upo.Identity.Tests
{
    public class OrganizationServiceTest : Basic
    {
        #region CRUD

        [Fact]
        public async Task test_organization_query()
        {
            var organization = new IdentityOrganization("name", 1);
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            var organizations = await OrganizationProvider.Query().ToListAsync().ConfigureAwait(false);
            Assert.True(organizations.Count == 1);
            Assert.Equal(organization, organizations.First());
        }

        [Fact]
        public async Task test_organization_create()
        {
            var organization = new IdentityOrganization("name", 1);
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            var organizations = await OrganizationProvider.Query().ToListAsync().ConfigureAwait(false);
            Assert.True(organizations.Count == 1);
            Assert.Equal(organization, organizations.First());
        }

        [Fact]
        public async Task test_organization_delete()
        {
            var organization = await OrganizationService.CreateAsync(new IdentityOrganization("name", 1)).ConfigureAwait(false);
            var finded = await OrganizationProvider.FindAsync(organization.Id).ConfigureAwait(false);
            Assert.NotNull(finded);

            await OrganizationService.DeleteAsync(organization.Id).ConfigureAwait(false);
            finded = await OrganizationProvider.FindAsync(organization.Id).ConfigureAwait(false);
            Assert.Null(finded);
        }

        [Fact]
        public async Task test_organization_update()
        {
            var organization = new IdentityOrganization("name", 1);
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            var finded = await OrganizationProvider.FindAsync(organization.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
            Assert.Equal(organization.Name, finded.Name);

            var changedValue = "changed";
            await OrganizationService.UpdateAsync(organization.Id, new Dictionary<string, object>
            {
                { nameof(IdentityOrganization.Name), changedValue}
            }).ConfigureAwait(false);
            finded = await OrganizationProvider.FindAsync(organization.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
            Assert.Equal(changedValue, finded.Name);
        }


        [Fact]
        public async Task test_organization_find()
        {
            var organization = new IdentityOrganization("name", 1);
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            var finded = await OrganizationProvider.FindAsync(organization.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
        }

        #endregion

        #region Extensions

        [Fact]
        public async Task test_organization_get_children()
        {
            var children = await OrganizationProvider.GetChildrenAsync(null).ConfigureAwait(false);
            Assert.NotNull(children);
            Assert.True(children.Count() == 0);
            var parent = new IdentityOrganization("parent", 1);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            children = await OrganizationProvider.GetChildrenAsync(null).ConfigureAwait(false);
            Assert.NotNull(children);
            Assert.True(children.Count() == 1);
            Assert.Equal(parent, children.First());

            var childOne = new IdentityOrganization("childOne", 1, parent.Id);
            await OrganizationService.CreateAsync(childOne).ConfigureAwait(false);
            children = await OrganizationProvider.GetChildrenAsync(parent.Id).ConfigureAwait(false);
            Assert.NotNull(children);
            Assert.True(children.Count() == 1);
            Assert.Equal(childOne, children.First());
        }

        [Fact]
        public async Task test_organization_paging_users()
        {
            var parent = new IdentityOrganization("parent", 1);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            var users = new List<IdentityUser>();
            for (int i = 0; i < 10; i++)
            {
                var user = new IdentityUser($"ac{i}", "pwd");
                await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
                users.Add(user);
            }

            var pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            var value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == users.Count);
            Assert.Equal(users, value);
        }

        [Fact]
        public async Task test_organization_add_user_main()
        {
            var parent = new IdentityOrganization("parent", 1);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            var user = new IdentityUser("ac", "pwd");
            await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
            var pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            var value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == 1);
            Assert.Equal(user, value.First());
        }

        [Fact]
        public async Task test_organization_add_user_secondary()
        {
            var parent = new IdentityOrganization("parent", 1);
            var child = new IdentityOrganization("child", 1, parent.Id);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            await OrganizationService.CreateAsync(child).ConfigureAwait(false);
            var user = new IdentityUser("ac", "pwd");
            await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
            await OrganizationService.AddUserAsync(child.Id, user.Id).ConfigureAwait(false);

            var pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            var value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == 1);
            Assert.Equal(user, value.First());

            pagingResult = await OrganizationProvider.PagingUsersAsync(child.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == 1);
            Assert.Equal(user, value.First());
        }

        [Fact]
        public async Task test_organization_remove_user()
        {
            var parent = new IdentityOrganization("parent", 1);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            var user = new IdentityUser("ac", "pwd");
            await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
            var pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            var value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == 1);
            Assert.Equal(user, value.First());

            await OrganizationService.RemoveUserAsync(parent.Id, user.Id).ConfigureAwait(false);
            pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.TotalCount == 0);
        }

        [Fact]
        public async Task test_organization_remove_users()
        {
            var parent = new IdentityOrganization("parent", 1);
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            var users = new List<IdentityUser>();
            for (int i = 0; i < 10; i++)
            {
                var user = new IdentityUser($"ac{i}", "pwd");
                await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
                users.Add(user);
            }

            var pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.Value is IEnumerable<IdentityUser>);
            var value = pagingResult.Value as IEnumerable<IdentityUser>;
            Assert.True(value.Count() == users.Count);
            Assert.Equal(users, value);

            await OrganizationService.RemoveUsersAsync(parent.Id, users.Select(x => x.Id).ToArray()).ConfigureAwait(false);
            pagingResult = await OrganizationProvider.PagingUsersAsync(parent.Id).ConfigureAwait(false);
            Assert.True(pagingResult.TotalCount == 0);
        }

        #endregion
    }
}
