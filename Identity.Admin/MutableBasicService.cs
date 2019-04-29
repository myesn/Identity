using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin
{
    public abstract class MutableBasicService<T> : IMutableBasicService<T> where T : class
    {
        private readonly IUserStore _store;

        public MutableBasicService(IUserStore store)
        {
            this._store = store;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            GetSet().Add(entity);
            await _store.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<T> DeleteAsync(Guid id)
        {
            var entity = await GetSet().FindAsync(id).ConfigureAwait(false);
            if (entity == null)
                return null;

            GetSet().Remove(entity);
            await _store.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }

        public virtual async Task<T> UpdateAsync(Guid id, IDictionary<string, object> updateProperties)
        {
            var entity = await GetSet().FindAsync(id).ConfigureAwait(false);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(T).Name} not found:{id}");

            foreach (var updateProperty in updateProperties)
            {
                var property = entity.GetType().GetProperty(updateProperty.Key);
                property.SetValue(entity, updateProperty.Value);
            }
            GetSet().Update(entity);
            await _store.SaveChangesAsync().ConfigureAwait(false);

            return entity;
        }


        private DbSet<T> GetSet() =>
            (DbSet<T>)_store.Context.Set(typeof(T).FullName);
    }
}