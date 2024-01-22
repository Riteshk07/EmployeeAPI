using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IPasswordHash
    {
        public string HashPassword(string password);
        public bool Verify(string hashedPass, string inputPass);
    }
}
