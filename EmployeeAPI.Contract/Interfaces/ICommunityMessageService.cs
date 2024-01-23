using EmployeeAPI.Contract.Dtos.CommunityMessageDto;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface ICommunityMessageService
    {
        public Task<ResponseMsg> SendMessage(MessageDto message, IEnumerable<Claim> claim, int? RecieverId);
        public Task<ResponseWIthEterableMessage<MessageBoxDto>> DisplayMessage(IEnumerable<Claim> claim, int? RecieverId);
        public Task<ResponseMsg> DeleteMessage(int id);

        public Task<ResponseWIthEterableMessage<ChatBoxDto>> GetChatBox(IEnumerable<Claim> claim);
    }
}
