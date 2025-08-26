using Data.Interfaces;
using Data; // Assuming ApplicationContext is here
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // ⬅️ Changed to protected to be accessible by derived classes (like AttendanceRepository)
        protected readonly ApplicationContext _context;

        public GenericRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        // ⬅️ Implemented: Gets all entities that match a filter
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) =>
            await _context.Set<T>().Where(predicate).ToListAsync();

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) =>
            await _context.Set<T>().FirstOrDefaultAsync(predicate);

        // ⬅️ Implemented: Provides an IQueryable for flexible queries
        public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return _context.Set<T>().AsQueryable();
            }
            return _context.Set<T>().Where(predicate).AsQueryable(); // Return as IQueryable for chaining
        }

        // ⬅️ Removed: This method is not needed here
        // public IGenericRepository<T1> GetGenericRepository<T1>() where T1 : class
        // {
        //     throw new NotImplementedException();
        // }

        public void Update(T entity) => _context.Set<T>().Update(entity);
    }
}
