using EmployeeAPI.Contract.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.TodoDtos
{
    public class TodoDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
