using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Models
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }   
        public string Password { get; set; }
        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public Employee? Employee { get; set; }
    }
}
