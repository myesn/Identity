using System;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Upo.Identity.Tests
{
    public class UserServiceTest : Basic
    {
        #region CRUD

        [Fact]
        public async Task test_user_query()
        {
            var user = new IdentityUser("ac", "pwd");
            await UserService.CreateAsync(user).ConfigureAwait(false);
            var list = await UserProvider.Query().ToListAsync().ConfigureAwait(false);
            Assert.NotNull(list);
            Assert.True(list.Count == 1);
            Assert.Equal(user, list.First());
        }

        [Fact]
        public async Task test_user_create()
        {
            var user = new IdentityUser("ac", "pwd");
            await UserService.CreateAsync(user).ConfigureAwait(false);
            var finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
            Assert.Equal(user, finded);
        }

        [Fact]
        public async Task test_user_delete()
        {
            var parent = new IdentityOrganization("name");
            var user = new IdentityUser("ac", "pw");
            await OrganizationService.CreateAsync(parent).ConfigureAwait(false);
            await OrganizationService.AddUserAsync(parent.Id, user).ConfigureAwait(false);
            var finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(finded);

            await UserService.DeleteAsync(user.Id).ConfigureAwait(false);
            finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.Null(finded);
        }

        [Fact]
        public async Task test_user_update()
        {
            var user = new IdentityUser("ac", "pw");
            await UserService.CreateAsync(user).ConfigureAwait(false);
            var finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
            Assert.Equal(user.AccountName, finded.AccountName);
            Assert.Equal(user.Password, finded.Password);

            var changedValue = "changed";
            await UserService.UpdateAsync(user.Id, new Dictionary<string, object>
            {
                { nameof(IdentityUser.AccountName), changedValue},
                { nameof(IdentityUser.Password), changedValue}
            }).ConfigureAwait(false);
            finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
            Assert.Equal(changedValue, finded.AccountName);
            Assert.Equal(EncryptService.Encrypt(changedValue), finded.Password);
        }


        [Fact]
        public async Task test_user_find()
        {
            var user = new IdentityUser("ac", "pw");
            await UserService.CreateAsync(user).ConfigureAwait(false);
            var finded = await UserProvider.FindAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(finded);
        }

        #endregion

        #region Extensions

        [Fact]
        public async Task test_user_change_main_organization()
        {
            var organization = new IdentityOrganization("o");
            var organization2 = new IdentityOrganization("o2");
            var user = new IdentityUser("ac", "pwd");
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            await OrganizationService.CreateAsync(organization2).ConfigureAwait(false);
            await OrganizationService.AddUserAsync(organization.Id, user).ConfigureAwait(false);

            var mainOrganization = await UserProvider.GetMainOrganizationAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(mainOrganization);
            organization.Users = null;
            Assert.Equal(organization, mainOrganization);

            await UserService.ChangeMainOrganizationAsync(user.Id, organization2.Id).ConfigureAwait(false);
            mainOrganization = await UserProvider.GetMainOrganizationAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(mainOrganization);
            Assert.Equal(organization2, mainOrganization);
        }

        [Fact]
        public async Task test_user_get_main_organization()
        {
            var organization = new IdentityOrganization("o");
            var organization2 = new IdentityOrganization("o2");
            var user = new IdentityUser("ac", "pwd");
            await OrganizationService.CreateAsync(organization).ConfigureAwait(false);
            await OrganizationService.CreateAsync(organization2).ConfigureAwait(false);
            await OrganizationService.AddUserAsync(organization.Id, user).ConfigureAwait(false);

            var mainOrganization = await UserProvider.GetMainOrganizationAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(mainOrganization);
            Assert.Equal(organization, mainOrganization);

            await UserService.ChangeMainOrganizationAsync(user.Id, organization2.Id).ConfigureAwait(false);
            mainOrganization = await UserProvider.GetMainOrganizationAsync(user.Id).ConfigureAwait(false);
            Assert.NotNull(mainOrganization);
            Assert.Equal(organization2, mainOrganization);
        }

        #endregion
    }
}
