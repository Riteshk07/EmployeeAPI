using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IValidationService
    {
        public bool ValidateEmail(string email);
        public bool ValidatePhoneNumber(string phoneNumber);
        public bool ValidatePassword(string password);
    }
}
