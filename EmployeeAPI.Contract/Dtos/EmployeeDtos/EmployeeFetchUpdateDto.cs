using EmployeeAPI.Contract.Dtos.TodoDtos;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.EmployeeDtos
{
    public class EmployeeFetchUpdateDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public EmployeeType EmployeeType { get; set; }


        public string Address { get; set; }


        public string City { get; set; }


        public string Country { get; set; }


        public string Phone { get; set; }


        public int DepartmentID { get; set; }

        public string? DepartmentName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
