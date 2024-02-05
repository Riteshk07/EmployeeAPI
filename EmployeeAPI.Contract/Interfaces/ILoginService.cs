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
        /// <summary>
        /// This Method is used for User login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>LoginResponseMessage - It will return a User details and Token.</returns>
        public Task<LoginResponseMessage> UserLogin(LoginDto loginDto, string IpAddress);
        /// <summary>
        /// This Method is used for Change Email
        /// </summary>
        /// <param name="oldEmail"></param>
        /// <param name="email"></param>
        /// <returns>ResponseWithObjectMessage - Return a Response with Employee Object</returns>
        public Task<ResponseWithObjectMessage<Employee>> ChangeEmail(string oldEmail, string email);

        /// <summary>
        /// This method is used for Generating a token 
        /// </summary>
        /// <param name="emp"></param>
        /// <returns>string - Return a String token.</returns>
        public string GeneratingToken(Employee emp, string IpAddress);

        /// <summary>
        /// this method is used for Changing User Password
        /// </summary>
        /// <param name="data"></param>
        /// <param name="email"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> ChangePassword(LoginPasswordDto data, string email);
    }
}
