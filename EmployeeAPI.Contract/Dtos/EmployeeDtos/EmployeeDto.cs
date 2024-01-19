using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAPI.Contract.Enums;

namespace EmployeeAPI.Contract.Dtos.EmployeeDtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public EmployeeType EmployeeType { get; set; }


        public string Address { get; set; }


        public string City { get; set; }


        public string Country { get; set; }


        public string Phone { get; set; }


        public int DepartmentID { get; set; }


    }
}
