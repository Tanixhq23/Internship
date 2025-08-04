using System.Linq.Expressions;

namespace Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IGenericRepository<T> GetGenericRepository<T>() where T : class;
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
