using EmployeeAPI.Contract.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Context
{
    public class EmployeeDBContext:DbContext
    {
        public EmployeeDBContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<CommunityMessage> CommunityMessages { get; set; }
    }
}
