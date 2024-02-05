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
        /// <summary>
        /// This method will Check first user and send Email to provided Email id by user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<ResponseMsg> ForgetPassword(string email, string IpAddress);

        /// <summary>
        /// The method check the token and fetch user information and set login password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="token"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> ResetPassword(string password, string token);

        /// <summary>
        /// This method is used for cheking user is alredy exist or not by it's Email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> CheckEmail(string Email);
    }
}
