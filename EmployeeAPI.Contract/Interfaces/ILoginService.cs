using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.LoginDtos;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface ILoginService
    {
        public Task<LoginResponseMessage> UserLogin(LoginDto loginDto);

        public Task<ResponseWithObjectMessage<Employee>> ChangeEmail(string oldEmail, string email);

        public string GeneratingToken(Employee emp);
    }
}
