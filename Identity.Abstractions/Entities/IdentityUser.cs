using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class IdentityUser
    {
        private IdentityUser() { }
        public IdentityUser(string accountName, string password, Gender gender = Gender.Male)
        {
            this.AccountName = accountName;
            this.Password = password;
            this.Gender = gender;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Gender Gender { get; set; }
        public byte[] Avatar { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public virtual IEnumerable<IdentityOrganizationUser> Organizations { get; set; }
        public virtual IEnumerable<IdentityToken> Tokens { get; set; }
    }

    public enum Gender
    {
        Male = 0, Female
    }
}