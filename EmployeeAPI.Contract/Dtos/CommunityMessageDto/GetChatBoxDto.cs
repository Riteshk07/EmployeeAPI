using EmployeeAPI.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.CommunityMessageDto
{
    public class GetChatBoxDto
    {
        public int EmployeeId { get; set; }
        public int Count { get; set; }
        public CommunityMessage? CommunityMessagge { get; set; }
    }
}
