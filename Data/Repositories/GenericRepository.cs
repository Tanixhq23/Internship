using Data.Interfaces;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationContext _context;

        public GenericRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().FirstOrDefaultAsync(predicate);

        public IGenericRepository<T1> GetGenericRepository<T1>() where T1 : class
        {
            throw new NotImplementedException();
        }

        public void Update(T entity) => _context.Set<T>().Update(entity);
    }
}
