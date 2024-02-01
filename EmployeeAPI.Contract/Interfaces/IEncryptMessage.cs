using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IEncryptMessage
    {
        /// <summary>
        /// This method used to add Encryption in message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>string - Return a Encrypt message.</returns>
        public string Encrypt(string message);

        /// <summary>
        /// This method is used for decrypt the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>string - Return a Decrypt message.</returns>
        public string Decrypt(string message);
    }
}
