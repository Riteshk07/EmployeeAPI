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
        /// <summary>
        /// This method is used for sending a message for that perticular reciever id.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="claim"></param>
        /// <param name="RecieverId"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> SendMessage(MessageDto message, IEnumerable<Claim> claim, int? RecieverId);

        /// <summary>
        /// This method is used for fetch user Conversation between two Employee
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="RecieverId"></param>
        /// <returns>ResponseWIthEterableMessage - Return a Response with List of MessageBoxDto object</returns>
        public Task<ResponseWIthEterableMessage<MessageBoxDto>> DisplayMessage(IEnumerable<Claim> claim, int? RecieverId);

        /// <summary>
        /// This method is used for Delete the message by it's Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> DeleteMessage(int id);

        /// <summary>
        /// This method is used for getting all Chats between Employee with Last message.
        /// </summary>
        /// <param name="claim"></param>
        /// <returns>ResponseWIthEterableMessage - Return a Response with List of ChatBoxDto object</returns>
        public Task<ResponseWIthEterableMessage<ChatBoxDto>> GetChatBox(IEnumerable<Claim> claim);
    }
}
