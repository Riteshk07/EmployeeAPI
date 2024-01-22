using EmployeeAPI.Contract.Dtos.CommunityMessageDto;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class CommunityMessageService : ICommunityMessageService
    {
        public Task<ResponseMsg> DeleteMessage(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseWIthEterableMessage<MessageBoxDto>> DisplayMessage()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMsg> SendMessage(MessageDto message)
        {
            throw new NotImplementedException();
        }
    }
}
