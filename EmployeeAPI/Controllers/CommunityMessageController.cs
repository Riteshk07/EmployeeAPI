using EmployeeAPI.Contract.Dtos.CommunityMessageDto;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityMessageController : ControllerBase
    {
        private readonly ICommunityMessageService communityMessageService;
        private readonly IEncryptMessage encryptMessage;

        public CommunityMessageController(ICommunityMessageService communityMessageService, IEncryptMessage encryptMessage) {
            this.communityMessageService = communityMessageService;
            this.encryptMessage = encryptMessage;
        }
        [Authorize]
        [HttpPost("SendMessage/{RecieverId?}")]
        public async Task<ActionResult<ResponseMsg>> SendMessage([FromBody] MessageDto data, int? RecieverId=null)
        {
            data.Message = encryptMessage.Encrypt(data.Message);
            var resp = await communityMessageService.SendMessage(data, HttpContext.User.Claims, RecieverId);
            if(resp.StatusCode == 200)
            {
                return Ok(resp);
            }else if(resp.StatusCode == 401)
            {
                return Unauthorized(resp);
            }else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else
            {
                return new ObjectResult(resp){ StatusCode = (int) StatusCodes.Status500InternalServerError};
            }
        }
        [Authorize]
        [HttpGet("DisplayMessage/{EmployeeId?}")]
        public async Task <ActionResult<ResponseWIthEterableMessage<MessageBoxDto>>> DisplayMessage(int?EmployeeId=null)
        {
            var resp = await communityMessageService.DisplayMessage(HttpContext.User.Claims, EmployeeId);
            if(resp.StatusCode == 200)
            {
                return Ok(resp);
            }
            else
            {
                return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }

        }
        [Authorize]
        [HttpDelete("DeleteMessage/{Id}")]
        public async Task <ActionResult<ResponseMsg>> DeleteMessage(int Id)
        {
            var resp = await communityMessageService.DeleteMessage(Id);
            if(resp.StatusCode == 200)
            {
                return Ok(resp);
            }else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else
            {
                return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize]
        [HttpGet("GetChatBox")]
        public async Task <ActionResult<ResponseWIthEterableMessage<ChatBoxDto>>> GetChatBox()
        {
            var resp = await communityMessageService.GetChatBox(HttpContext.User.Claims);
            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }
            else
            {
                return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }
    }
}
