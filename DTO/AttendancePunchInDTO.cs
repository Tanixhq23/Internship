using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AttendancePunchInDto
    {
        // UserId will often be inferred from the authenticated user's token
        // but can be explicitly sent if an admin/HR is punching in for someone else.
        // For self-punch-in, this could be omitted or validated against JWT.
        public int? UserId { get; set; }

        // Optional: Location data from the client
        public string? Location { get; set; }
    }
}
