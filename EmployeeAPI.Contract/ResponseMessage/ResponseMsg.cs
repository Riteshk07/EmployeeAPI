using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.ResponseMessage
{
    public class ResponseMsg
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
