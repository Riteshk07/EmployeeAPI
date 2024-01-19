using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.DepartmentDtos
{
    public class GroupByDepartmentDto:DepartmentFetchDto
    {
        public int EmployeesCount { get; set; }  
    }
}
