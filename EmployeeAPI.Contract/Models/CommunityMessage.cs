using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Models
{
    public class CommunityMessage
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int? RecieverId { get; set; }
        public int DepartmentId { get; set; }
        public string? UserType { get; set; }
    }
}
