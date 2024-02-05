using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.EmployeeDtos
{
    public class EmployeeFetchNotificationDto : EmployeeFetchUpdateDto
    {
        public int NotificationCount { get; set; }
    }
}
