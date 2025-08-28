// File: Data/Repositories/EmployeeRepository.cs

using Data.Interfaces; // For IEmployeeRepository
using Entity; // For Employee entity
using Microsoft.EntityFrameworkCore; // For MaxAsync
using System.Threading.Tasks;
using System.Linq; // For Query() and Max()

namespace Data.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private const int STARTING_EMP_ID = 10000; // Define your starting ID for new employees

        public EmployeeRepository(ApplicationContext context) : base(context)
        {
            // The _context is inherited from the base GenericRepository constructor.
        }

        // --- Implement specific methods defined in IEmployeeRepository ---

        public async Task<Employee?> GetByOfficeEmailAsync(string officeEmail)
        {
            return await _context.Set<Employee>()
                                 .FirstOrDefaultAsync(e => e.OfficeEmail == officeEmail);
        }

        public async Task<int> GetNextEmpIdAsync()
        {
            // Find the maximum existing EmpId and increment it.
            // If no employees exist, start from STARTING_EMP_ID.
            int nextEmpId = STARTING_EMP_ID;
            var maxEmpId = await _context.Set<Employee>().Select(e => (int?)e.EmpId).MaxAsync();

            if (maxEmpId.HasValue && maxEmpId.Value >= STARTING_EMP_ID)
            {
                nextEmpId = maxEmpId.Value + 1;
            }
            else
            {
                nextEmpId = STARTING_EMP_ID; // Start from 10000 if no employees or if max ID is lower
            }
            return nextEmpId;
        }
    }
}
