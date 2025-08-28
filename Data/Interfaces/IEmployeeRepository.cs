// File: Data/Interfaces/IEmployeeRepository.cs

using Entity; // Ensure Entity namespace is referenced
using Data.Interfaces; // For IGenericRepository
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        // Add employee-specific query methods here if needed
        // For example:
        Task<Employee?> GetByOfficeEmailAsync(string officeEmail);
        Task<int> GetNextEmpIdAsync(); // For generating sequential employee IDs
    }
}