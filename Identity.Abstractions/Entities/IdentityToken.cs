using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class IdentityToken
    {
        private IdentityToken() { }
        public IdentityToken(Guid userId)
        {
            this.UserId = userId;
            this.IssueTime = DateTime.Now;
            this.ExpiredTime = DateTime.Now.AddHours(2);
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime IssueTime { get; set; }
        public DateTime ExpiredTime { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}