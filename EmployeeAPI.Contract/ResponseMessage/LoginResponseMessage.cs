using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.ResponseMessage
{
    public class LoginResponseMessage:ResponseWithObjectMessage<EmployeeFetchUpdateDto>
    {
        public string Token { get; set; }   
    }
}
