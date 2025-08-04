using Data.Repositories;
using Data;
using Data.Interfaces;
using Entity;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
        }
        public IUserRepository Users { get; set;}

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
