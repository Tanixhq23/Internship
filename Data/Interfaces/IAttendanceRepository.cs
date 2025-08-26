// File: Data/Interfaces/IAttendanceRepository.cs (Updated)

using Entity; // For AttendanceRecord
using Data.Repositories; // If IGenericRepository is used as a base
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<AttendanceRecord>
    {
        // Add attendance-specific query methods here
        Task<AttendanceRecord?> GetActivePunchInForUserAsync(int userId); // Changed from Guid to int
        Task<List<AttendanceRecord>> GetRecordsForUserAndDayAsync(int userId, DateTime date); // Changed from Guid to int
        Task<List<AttendanceRecord>> GetAllRecordsForDayAsync(DateTime date);

        // If your GenericRepository doesn't have a Query method returning IQueryable,
        // you might need to add one here or directly in GenericRepository.
        IQueryable<AttendanceRecord> Query(Expression<Func<AttendanceRecord, bool>>? predicate = null);
    }
}
