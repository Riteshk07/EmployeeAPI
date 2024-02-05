using EmployeeAPI.Contract.Dtos.NotificationDto;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface INotificationService
    {
        public Task<ResponseWIthEterableMessage<NotificationFetchDto>> GetNotifications(IEnumerable<Claim> claim);
    }
}
