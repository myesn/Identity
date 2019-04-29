using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity
{
    public interface IBasicProvider<T> where T : class
    {
        Type EntityType { get; set; }
        IQueryable<T> Query();
        Task<T> FindAsync(Guid id);
    }
}