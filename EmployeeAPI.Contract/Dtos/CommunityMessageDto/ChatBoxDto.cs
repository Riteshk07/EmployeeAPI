using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EmployeeAPI.Contract.Dtos.CommunityMessageDto
{
    public class ChatBoxDto
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? LastMessage { get; set; }
        public bool IsSeen { get; set; }
        public int NewMessages { get; set; }
    }
}
