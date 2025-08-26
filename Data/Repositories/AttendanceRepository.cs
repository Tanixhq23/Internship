// File: Data/Repositories/AttendanceRepository.cs (Updated)

using Data.Interfaces; // For IAttendanceRepository
using Entity; // For AttendanceRecord
using Microsoft.EntityFrameworkCore; // For DbSet, Querying
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class AttendanceRepository : GenericRepository<AttendanceRecord>, IAttendanceRepository
    {
        // No explicit _context declaration needed here, it's inherited as protected from GenericRepository.

        public AttendanceRepository(ApplicationContext context) : base(context)
        {
            // The _context is passed to the base GenericRepository constructor.
            // You can access it via `_context` within this class methods directly.
        }

        // --- Implement specific methods defined in IAttendanceRepository ---

        public async Task<AttendanceRecord?> GetActivePunchInForUserAsync(int userId) // Changed from Guid to int
        {
            return await _context.Set<AttendanceRecord>() // Use _context directly from base class
                                 .Where(ar => ar.UserId == userId && ar.PunchOutTime == null && ar.Status == "PunchedIn")
                                 .OrderByDescending(ar => ar.PunchInTime)
                                 .FirstOrDefaultAsync();
        }

        public async Task<List<AttendanceRecord>> GetRecordsForUserAndDayAsync(int userId, DateTime date) // Changed from Guid to int
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1);

            return await _context.Set<AttendanceRecord>() // Use _context directly from base class
                                 .Where(ar => ar.UserId == userId &&
                                              ar.PunchInTime >= startOfDay &&
                                              ar.PunchInTime < endOfDay)
                                 .OrderBy(ar => ar.PunchInTime)
                                 .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetAllRecordsForDayAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1);

            return await _context.Set<AttendanceRecord>() // Use _context directly from base class
                                 .Where(ar => ar.PunchInTime >= startOfDay &&
                                              ar.PunchInTime < endOfDay)
                                 .OrderBy(ar => ar.PunchInTime)
                                 .ToListAsync();
        }

        // The Query method is inherited from GenericRepository
        // If you need a specific Query method for AttendanceRepository, you'd implement it here.
    }
}
