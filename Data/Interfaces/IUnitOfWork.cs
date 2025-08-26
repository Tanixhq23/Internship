
using Entity;

namespace Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository Users { get; set; }
        public IAttendanceRepository AttendanceRecords { get; set; }
        Task CompleteAsync();
    }
}
