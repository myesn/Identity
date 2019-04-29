using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity.Admin
{
    public class UserPasswordBase64EncryptService : IUserPasswordEncryptService
    {
        public string Decrypt(string encryptText)
        {
            var base64EncodedBytes = Convert.FromBase64String(encryptText);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string Encrypt(string originText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(originText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}