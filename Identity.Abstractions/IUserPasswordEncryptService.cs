using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity.Admin
{
    public interface IUserPasswordEncryptService
    {
        string Encrypt(string originText);
        string Decrypt(string encryptText);
    }
}