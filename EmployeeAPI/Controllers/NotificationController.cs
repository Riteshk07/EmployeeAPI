using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.NotificationDto;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService) {
            this.notificationService = notificationService;
        }
        [HttpGet("GetNotifications")]
        public async Task<ActionResult<ResponseWIthEterableMessage<NotificationFetchDto>>> GetNotifications()
        {
            var resp = await notificationService.GetNotifications(HttpContext.User.Claims);
            if (resp.StatusCode == 200 )
            {
                return Ok(resp);
            }else if (resp.StatusCode ==404)
            {
                return NotFound(resp);
            }
            else
            {
                return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }
    }
}
