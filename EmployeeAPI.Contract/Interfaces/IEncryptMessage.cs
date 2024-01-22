using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IEncryptMessage
    {
        public string Encrypt(string message);
        public string Decrypt(string message);
    }
}
