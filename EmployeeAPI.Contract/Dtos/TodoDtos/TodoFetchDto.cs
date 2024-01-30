using EmployeeAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.TodoDtos
{
    public class TodoFetchDto: TodoDto
    {
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public EmployeeType? AssignBy { get; set; }
    }
}
