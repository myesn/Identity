using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public interface IMutableBasicService<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> DeleteAsync(Guid id);
        Task<T> UpdateAsync(Guid id, IDictionary<string, object> updateProperties);
    }
}