using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.PaginationDto
{
    public class TodoPageDto: PageDto
    {
        public bool? IsCompleted { get; set; }
    }
}
