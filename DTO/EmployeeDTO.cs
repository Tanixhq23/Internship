using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class EmployeeDto
    {
        public Guid EmpId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string JobRole { get; set; }
        public DateTime HireDate { get; set; }
        public Guid? DeptId { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? ShiftId { get; set; }
    }
}
