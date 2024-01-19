using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.DepartmentDtos
{
    public class DepartmentFetchDto
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }
    }
}
