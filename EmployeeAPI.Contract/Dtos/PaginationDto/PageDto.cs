using EmployeeAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Dtos.PaginationDto
{
    public class PageDto
    {
        public bool  IsPagination { get; set; }
        public int? Index { get; set; }
        public int? Take { get; set; }
        public string? Search {  get; set; }
        public OrdersType? Orders { get; set; }
        public string? OrderBy { get; set; }
    }
}
