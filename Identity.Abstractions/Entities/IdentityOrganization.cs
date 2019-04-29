using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class IdentityOrganization
    {
        private IdentityOrganization() { }
        public IdentityOrganization(string name, int order = 1, Guid? parentId = null)
        {
            this.ParentId = parentId;
            this.Name = name;
            this.Order = order;
            this.CreateTime = DateTime.Now;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Guid? ParentId { get; set; }

        public virtual IdentityOrganization Parent { get; set; }
        public virtual IEnumerable<IdentityOrganization> Children { get; set; }
        public virtual IEnumerable<IdentityOrganizationUser> Users { get; set; }
    }
}