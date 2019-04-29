using Upo.Identity.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public class UserPasswordValidator : IUserPasswordValidator
    {
        private readonly IUserProvider _userProvider;
        private readonly IUserPasswordEncryptService _encryptService;
        public UserPasswordValidator(
            IUserProvider userProvider,
            IUserPasswordEncryptService encryptService
            )
        {
            this._userProvider = userProvider;
            this._encryptService = encryptService;
        }

        public async Task<bool> ValidateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var encryptPassword = this._encryptService.Encrypt(password);
            return await this._userProvider.Query().AnyAsync(x =>
                x.AccountName == username && x.Password == encryptPassword).ConfigureAwait(false);
        }
    }
}