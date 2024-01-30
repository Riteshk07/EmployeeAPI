using EmployeeAPI.Contract.Dtos.CommunityMessageDto;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class CommunityMessageService : ICommunityMessageService
    {
        private readonly EmployeeDBContext context;
        private readonly ILogger<CommunityMessageService> logger;
        private readonly IEncryptMessage encryptMessage;

        public CommunityMessageService(EmployeeDBContext context,  ILogger<CommunityMessageService> logger, IEncryptMessage encryptMessage) {
            this.context = context;
            this.logger = logger;
            this.encryptMessage = encryptMessage;
        }
        public Task<ResponseMsg> DeleteMessage(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseWIthEterableMessage<MessageBoxDto>> DisplayMessage(IEnumerable<Claim> claim, int? RecieverId)
        {
            ResponseWIthEterableMessage<MessageBoxDto> msg = new ResponseWIthEterableMessage<MessageBoxDto>();
            logger.LogInformation($"Claiming Information from method {nameof(SendMessage)}");
            var empId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
            var empType = claim.First(e => e.Type == "Role").Value;
            var empName = claim.First(e => e.Type == "Name").Value;
            int deptId = Convert.ToInt32(claim.First(c => c.Type == "DeptId").Value);
            try
            {
                var messages = await context.CommunityMessages
                    .Where(m => m.RecieverId != null ? (m.RecieverId == empId || m.EmployeeId == RecieverId) || (m.RecieverId == RecieverId || m.EmployeeId == empId) : m.DepartmentId ==deptId)
                    .ToListAsync();
                List<MessageBoxDto> messageBoxDtos = new List<MessageBoxDto>();
                foreach (var item in messages)
                {
                    item.IsSeen = true;
                    
                    messageBoxDtos.Add(new MessageBoxDto()
                    {
                        Id = item.Id,
                        Name = item.EmployeeName,
                        Message = item.Message != null ? encryptMessage.Decrypt(item.Message) : "",
                        UserType = item.UserType,
                        IsSeen = item.IsSeen,
                        MessageDate = item.MessageDate
                    });
                }
                await context.SaveChangesAsync();
                msg.IterableData = messageBoxDtos;
                msg.Message = "Message fetched successfully";
                msg.StatusCode = 200;
                msg.Status = "success";
                return msg;
            }
            catch (Exception Ex)
            {
                logger.LogInformation($"Exception Ouccred \nStacktrace : {Ex.StackTrace}\nMessage: {Ex.Message}");
                msg.IterableData = null;
                msg.Message = "Error from server side";
                msg.StatusCode = 500;
                msg.Status = "failed";
                return msg;
            }
        }

        public async Task<ResponseWIthEterableMessage<ChatBoxDto>> GetChatBox(IEnumerable<Claim> claim)
        {
            ResponseWIthEterableMessage<ChatBoxDto> msg = new ResponseWIthEterableMessage<ChatBoxDto>();

            logger.LogInformation($"Claiming Information from method {nameof(SendMessage)}");
            var empId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
            var empType = claim.First(e => e.Type == "Role").Value;
            var empName = claim.First(e => e.Type == "Name").Value;
            int deptId = Convert.ToInt32(claim.First(c => c.Type == "DeptId").Value);

            try
            {
                
                var Chats = await context.CommunityMessages.Where(m => m.RecieverId == empId)
                    .GroupBy(m => m.EmployeeId)
                    .Select(mGroup => new ChatBoxDto()
                {
                    EmployeeId = mGroup.Key,
                    LastMessage = encryptMessage.Decrypt(mGroup.OrderByDescending(x=> x.Id).FirstOrDefault().Message),
                    EmployeeName = mGroup.First().EmployeeName,
                    IsSeen = mGroup.OrderByDescending(x => x.Id).FirstOrDefault().IsSeen,
                    NewMessages = mGroup.Where(m => m.IsSeen == false).Count()
                })
                    .ToListAsync();
                msg.IterableData = Chats;
                msg.Message = "Chats fetched successfully";
                msg.StatusCode = 200;
                msg.Status = "success";
                return msg;
            }
            catch (Exception Ex)
            {
                logger.LogInformation($"Exception Ouccred \nStacktrace : {Ex.StackTrace}\nMessage: {Ex.Message}");
                msg.IterableData = null;
                msg.Message = "Error from server side";
                msg.StatusCode = 500;
                msg.Status = "failed";
                return msg;
            }
            throw new NotImplementedException();
        }

        public async Task<ResponseMsg> SendMessage(MessageDto message, IEnumerable<Claim> claim, int? RecieverId)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                logger.LogInformation($"Claiming Information from method {nameof(SendMessage)}");
                var empId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                var empType = claim.First(e => e.Type == "Role").Value;
                var empName = claim.First(e => e.Type == "Name").Value;
                int deptId = Convert.ToInt32(claim.First(c => c.Type == "DeptId").Value);

                logger.LogInformation($"Saving Message Data in database");
                CommunityMessage communityMessage = new CommunityMessage();
                communityMessage.EmployeeName = empName;
                communityMessage.EmployeeId = empId;
                communityMessage.Message = message.Message;
                communityMessage.UserType = empType;
                communityMessage.DepartmentId = deptId;
                communityMessage.RecieverId = RecieverId;
                communityMessage.MessageDate = DateTime.UtcNow;
                communityMessage.IsSeen = false;
                await context.CommunityMessages.AddAsync(communityMessage);
                await context.SaveChangesAsync();

                responseMsg.Message = "Message saved successfull";
                responseMsg.StatusCode = 200;
                responseMsg.Status = "success";
                return responseMsg;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Exception Ouccred \nStacktrace : {ex.StackTrace}\nMessage: {ex.Message}");
                responseMsg.Message = "Error from server side";
                responseMsg.StatusCode = 500;
                responseMsg.Status = "servererr";
                return responseMsg;
            }
        }
    }
}
