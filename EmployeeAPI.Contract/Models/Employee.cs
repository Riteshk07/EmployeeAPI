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
    // Dependent Entity
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        

        [Required]
        public EmployeeType EmployeeType { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }

        
        public string City { get; set; }

        
        public string Country { get; set; }

        
        public string Phone { get; set; }

        
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; }

        public ICollection<Todo>? Todos { get; set; }
            = new List<Todo>();

        public bool IsActive { get; set; } = true; 
        public DateTime? RecentActiveDateTime { get; set; }
        public DateTime? UserCreatedDateTime { get; set; }
        public DateTime? UserUpdatedDateTime { get; set; }
    }
}
