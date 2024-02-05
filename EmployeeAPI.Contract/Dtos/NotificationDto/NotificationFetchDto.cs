using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.NotificationDto
{
    public class NotificationFetchDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsSeen { get; set; }
        public DateTime Created { get; set; }
        public int? TodoId { get; set; }
    }
}
