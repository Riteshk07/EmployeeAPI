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
        public int EmployeeId { get; set; }
        public EmployeeType? AssignBy { get; set; }
    }
}
