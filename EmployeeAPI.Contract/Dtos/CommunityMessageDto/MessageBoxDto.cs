using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.CommunityMessageDto
{
    public class MessageBoxDto
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Name { get; set; }
        public string? UserType { get; set; }
        public bool IsSeen { get; set; }
        public DateTime MessageDate { get; set; }
    }
}
