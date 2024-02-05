using EmployeeAPI.Contract.Dtos.NotificationDto;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmployeeDBContext context;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(EmployeeDBContext context, ILogger<NotificationService> logger) {
            this.context = context;
            this.logger = logger;
        }
        public async Task<ResponseWIthEterableMessage<NotificationFetchDto>> GetNotifications(IEnumerable<Claim> claim)
        {
            ResponseWIthEterableMessage<NotificationFetchDto> message = new ResponseWIthEterableMessage<NotificationFetchDto>();
            try
            {
                #region Claming Information
                int userId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), claim.First(e => e.Type == "Role").Value);
                int DeptId = Convert.ToInt32(claim.First(e => e.Type == "DeptId").Value);
                string empName = claim.First(e => e.Type == "Name").Value;
                #endregion

                var notifications = await context.Notifications.OrderByDescending(x => x.Id).Where(n => n.EmployeeId == userId).ToListAsync();

                if(notifications != null || notifications.Count() != 0)
                {
                    message.IterableData = new List<NotificationFetchDto>();
                    foreach (var item in notifications)
                    {
                        message.IterableData.Add(new NotificationFetchDto()
                        {
                            Id = item.Id,
                            Message = item.Message,
                            IsSeen = item.IsSeen = true,
                            Created = item.Created,
                            TodoId = item.TodoId
                        });
                    }
                    await context.SaveChangesAsync();
                    message.Status = "success";
                    message.StatusCode = 200;
                    message.Message = "Notification fetched successfully";
                    return message;
                }
                else
                {
                    message.IterableData = null;
                    message.Status = "failed";
                    message.StatusCode = 404;
                    message.Message = "Notification not found";
                    return message;
                }
            }
            catch (Exception Ex)
            {
                message.IterableData = null;
                message.Status = "servererr";
                message.StatusCode = 500;
                message.Message = "Error from server side";
                return message;
            }
        }
    }
}
