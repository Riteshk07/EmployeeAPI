using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IValidationService
    {
        /// <summary>
        /// This method is used for Validate Email by Regex
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool - Return a boolean Value.</returns>
        public bool ValidateEmail(string email);

        /// <summary>
        /// This method is used for Validate Phone Number by Regex
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>bool - Return a boolean Value.</returns>
        public bool ValidatePhoneNumber(string phoneNumber);

        /// <summary>
        /// This method is used for Validate password by Regex. Password should be Minimum 8 Charecter, Alfa Numeric, 1 Upper case, 1 small case cherecter and a special cherecter
        /// </summary>
        /// <param name="password"></param>
        /// <returns>bool - Return a boolean Value.</returns>
        public bool ValidatePassword(string password);
    }
}
