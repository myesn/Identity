using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class IdentityOrganizationUser
    {
        private IdentityOrganizationUser() { }
        public IdentityOrganizationUser(Guid organizationId, Guid userId, int order, bool isPrimary = false)
        {
            this.OrganizationId = organizationId;
            this.UserId = userId;
            this.Order = order;
            this.IsPrimary = isPrimary;
        }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public int Order { get; set; }
        public bool IsPrimary { get; set; }

        public virtual IdentityOrganization Organization { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}