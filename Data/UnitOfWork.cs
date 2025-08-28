// File: Data/UnitOfWork.cs

using Data.Repositories;
using Data; // Assuming ApplicationContext is here
using Data.Interfaces;
using Entity; // Ensure using statement for Entity

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            AttendanceRecords = new AttendanceRepository(_context);

            // ⬅️ CRITICAL FIX: Initialize the new EmployeeRepository
            Employees = new EmployeeRepository(_context);
        }

        public IUserRepository Users { get; set; }
        public IAttendanceRepository AttendanceRecords { get; set; }

        // ⬅️ Implement the new Employees property
        public IEmployeeRepository Employees { get; set; }

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
