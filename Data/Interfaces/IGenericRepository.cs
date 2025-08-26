using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate); // Gets a single entity by filter

        // ⬅️ Updated: Gets all entities without a filter
        Task<IEnumerable<T>> GetAllAsync();
        // ⬅️ Added: Gets all entities that match a filter
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        // ⬅️ Added: Provides an IQueryable for more complex LINQ queries
        // Allows chaining .Where(), .OrderBy(), .Include() etc., before ToListAsync()
        IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null);

        // ⬅️ Removed: GetGenericRepository is typically not part of IGenericRepository itself
        // You'd use UnitOfWork to get specific repositories.
        // IGenericRepository<T1> GetGenericRepository<T1>() where T1 : class;
    }
}
