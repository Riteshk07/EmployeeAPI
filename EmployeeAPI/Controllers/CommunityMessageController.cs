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

        public CommunityMessageController(ICommunityMessageService communityMessageService) {
            this.communityMessageService = communityMessageService;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseMsg>> SendMessage([FromBody] MessageDto data)
        {
            var resp = await communityMessageService.SendMessage(data);
            return resp;
        }
        [Authorize]
        [HttpGet]
        public async Task <ActionResult<ResponseWIthEterableMessage<MessageBoxDto>>> DisplayMessage()
        {
            var resp = await communityMessageService.DisplayMessage();
            return resp;

        }
        [Authorize]
        [HttpDelete("DeleteMessage/{Id}")]
        public async Task <ActionResult<ResponseMsg>> DeleteMessage(int Id)
        {
            var resp = await communityMessageService.DeleteMessage(Id);
            return resp;
        }
    }
}
