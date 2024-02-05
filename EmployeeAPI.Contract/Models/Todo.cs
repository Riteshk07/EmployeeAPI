using EmployeeAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Models
{
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public EmployeeType? AssignBy { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public int? DepartmentId { get; set; }
        public int AssignById { get; set; }
        public DateTime? TaskCreatedDateTime { get; set; }
        public DateTime? TaskUpdatedDateTime { get; set; }
        public DateTime? DeadLine { get; set; }
    }
}
