using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Upo.Identity.Admin;

namespace Upo.Identity
{
    public abstract class BasicProvider<T> : IBasicProvider<T> where T : class
    {
        private readonly IUserStore _store;
        public Type EntityType { get; set; }

        public BasicProvider(IUserStore store)
        {
            this._store = store;
        }

        public virtual async Task<T> FindAsync(Guid id)
        {
            return await GetSet().FindAsync(id).ConfigureAwait(false);
        }

        public virtual IQueryable<T> Query()
        {
            return (IQueryable<T>)QueryableOfTypeMethod
                  .MakeGenericMethod(this.EntityType)
                  .Invoke(null, new object[] { GetSet() });
        }

        private DbSet<T> GetSet() =>
            (DbSet<T>)_store.Context.Set(typeof(T).FullName);

        private static MethodInfo QueryableOfTypeMethod = typeof(Queryable).GetMethod(nameof(Queryable.OfType));
    }
}