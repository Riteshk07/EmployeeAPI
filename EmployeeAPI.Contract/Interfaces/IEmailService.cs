using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IEmailService
    {
        public  Task<ResponseMsg> ForgetPassword(string email);


        public Task<ResponseMsg> ResetPassword(string password, string token);
    }
}
