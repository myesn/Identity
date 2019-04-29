using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public interface IUserPasswordValidator
    {
        Task<bool> ValidateAsync(string username, string password);
    }
}