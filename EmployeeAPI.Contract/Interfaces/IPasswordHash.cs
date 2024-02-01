using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IPasswordHash
    {
        /// <summary>
        /// This method used to make hash on password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>string - Return a Hashed String</returns>
        public string HashPassword(string password);

        /// <summary>
        /// This method used for verify Hashed password string
        /// </summary>
        /// <param name="hashedPass"></param>
        /// <param name="inputPass"></param>
        /// <returns>bool - Return a boolean value.</returns>
        public bool Verify(string hashedPass, string inputPass);
    }
}
