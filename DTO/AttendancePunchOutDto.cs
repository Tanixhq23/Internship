using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AttendancePunchOutDto
    {
        // UserId will often be inferred from the authenticated user's token
        // For self-punch-out, this could be omitted or validated against JWT.
        public int? UserId { get; set; }

        // Optional: Location data from the client
        public string? Location { get; set; }
    }
}
