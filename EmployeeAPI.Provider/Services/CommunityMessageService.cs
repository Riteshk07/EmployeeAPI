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
                var currUser = await context.Employees.FirstOrDefaultAsync(x => x.Id == empId);
                if (currUser != null)
                {
                    currUser.RecentActiveDateTime = DateTime.UtcNow;
                    context.Employees.Update(currUser);
                    await context.SaveChangesAsync();
                }
                var messages = await context.CommunityMessages
                    .Where(m => m.RecieverId != null ? (m.RecieverId == empId && m.EmployeeId == RecieverId) || (m.RecieverId == RecieverId && m.EmployeeId == empId) : m.DepartmentId ==deptId)
                    .ToListAsync();
                List<MessageBoxDto> messageBoxDtos = new List<MessageBoxDto>();
                foreach (var item in messages)
                {
                    if (item.EmployeeId == RecieverId)
                    {
                        item.IsSeen = true;
                    }
                    
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
                var currUser = await context.Employees.FirstOrDefaultAsync(x => x.Id == empId);
                if (currUser != null)
                {
                    currUser.RecentActiveDateTime = DateTime.UtcNow;
                    context.Employees.Update(currUser);
                    await context.SaveChangesAsync();
                }
                List<GetChatBoxDto> rawChats = await context.CommunityMessages
                    .Where(m => m.RecieverId == empId || m.EmployeeId == empId)
                    .GroupBy(m => m.EmployeeId)
                    .Select(group => new GetChatBoxDto()
                    {
                        CommunityMessagge = group.OrderByDescending(m => m.Id).First(),
                        Count = group.Count(m => !m.IsSeen),
                        EmployeeId = group.Key
                    }).ToListAsync();

                List<NewChatBoxDto> Chats = rawChats
                    .Select(lst => new NewChatBoxDto
                    {
                        Id = lst.CommunityMessagge.Id,
                        EmployeeId = lst.EmployeeId,
                        LastMessage = encryptMessage.Decrypt(lst.CommunityMessagge.Message),
                        EmployeeName = lst.CommunityMessagge.EmployeeName,
                        IsSeen = lst.CommunityMessagge.IsSeen,
                        NewMessages = lst.Count,
                        RecieverName = lst.CommunityMessagge.RecieverName,
                        RecieverId = Convert.ToInt32(lst.CommunityMessagge.RecieverId ?? 0)
                    }).ToList();
                    

                Dictionary<int, NewChatBoxDto> dictChats = new Dictionary<int, NewChatBoxDto>();

                foreach(var item in Chats)
                {
                    if (item.EmployeeId == empId) {
                        if (!dictChats.ContainsKey(item.RecieverId))
                        {
                            item.NewMessages = 0;
                            dictChats.Add(item.RecieverId, item);
                        }
                        else if(dictChats[item.RecieverId].Id < item.Id)
                        {
                            item.NewMessages = 0;
                            dictChats[item.RecieverId] = item;
                        }
                    }
                    else
                    {
                        if (!dictChats.ContainsKey(item.EmployeeId))
                        {
                            dictChats.Add(item.EmployeeId, item);
                        }
                        else if(dictChats[item.EmployeeId].Id < item.Id)
                        {
                            dictChats[item.EmployeeId] = item;
                        }
                    }
                }
                List<NewChatBoxDto> ChatsList = await context.Employees
                    .Where(x=> dictChats.Keys.ToList()
                    .Contains(x.Id))
                    .Select(e=> new NewChatBoxDto()
                {
                        Id = dictChats[e.Id].Id,
                        EmployeeId = dictChats[e.Id].EmployeeId,
                        LastMessage = dictChats[e.Id].LastMessage,
                        EmployeeName = dictChats[e.Id].EmployeeName,
                        IsSeen = dictChats[e.Id].IsSeen,
                        NewMessages = dictChats[e.Id].NewMessages,
                        RecieverId = dictChats[e.Id].RecieverId,
                        RecieverName = dictChats[e.Id].RecieverName,
                        LastActive = e.RecentActiveDateTime

                    }).ToListAsync();
                List<ChatBoxDto> empChats = ChatsList.OrderByDescending(x => x.Id).Select(m => new ChatBoxDto()
                {
                    EmployeeId = m.EmployeeId,
                    LastMessage = m.LastMessage,
                    EmployeeName = m.EmployeeName,
                    IsSeen = m.IsSeen,
                    NewMessages = m.NewMessages,
                    RecieverId = m.RecieverId,
                    RecieverName = m.RecieverName,
                    LastActive = m.LastActive
                }).ToList();

                msg.IterableData = empChats;
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

                var currUser = await context.Employees.FirstOrDefaultAsync(x => x.Id == empId);
                if (currUser != null)
                {
                    currUser.RecentActiveDateTime = DateTime.UtcNow;
                    context.Employees.Update(currUser);
                    await context.SaveChangesAsync();
                }

                logger.LogInformation($"Saving Message Data in database");
                var reciever = await context.Employees.FirstOrDefaultAsync(e => e.Id == RecieverId);
                CommunityMessage communityMessage = new CommunityMessage();
                communityMessage.EmployeeName = empName;
                communityMessage.EmployeeId = empId;
                communityMessage.Message = message.Message;
                communityMessage.UserType = empType;
                communityMessage.DepartmentId = deptId;
                communityMessage.RecieverName = reciever != null ? reciever.Name : "";
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
