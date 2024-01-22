using EmployeeAPI.Contract.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class EncryptMessage : IEncryptMessage
    {
        byte[] key;
        byte[] iv;
        public EncryptMessage() {
            this.key = new byte[16];

            this.iv = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }
        }
        public string Decrypt(string message)
        {
            byte[] decryptbytes = Convert.FromBase64String(message);

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(this.key, this.iv);
                using (MemoryStream ms = new MemoryStream(decryptbytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sw = new StreamReader(cs))
                        {
                            message = sw.ReadToEnd();
                        }
                    }
                }
            }
            return message;
        }

        public string Encrypt(string message)
        {
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(this.key, this.iv);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(message);
                        }
                    }
                    message = Convert.ToBase64String(ms.ToArray());
                }
            }
            return message;
        }
    }
}
