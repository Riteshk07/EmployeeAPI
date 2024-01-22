using EmployeeAPI.Contract.Dtos.CommunityMessageDto;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface ICommunityMessageService
    {
        public Task<ResponseMsg> SendMessage(MessageDto message);
        public Task<ResponseWIthEterableMessage<MessageBoxDto>> DisplayMessage();
        public Task<ResponseMsg> DeleteMessage(int id);
    }
}
