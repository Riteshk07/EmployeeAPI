using EmployeeAPI.Contract.Dtos.DepartmentDtos;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IDepartmentService
    {
        public Task<string> AddDepartment(DepartmentDto dept);

        public Task<ResponseWIthEterableMessage<DepartmentFetchDto>> GetAllDepartment();

        public Task<ResponseWithObjectMessage<DepartmentFetchDto>> GetDepartment(int id);
        public Task<ResponseMsg> DeleteDepartment(int id);

        public Task<ResponseWIthEterableMessage<GroupByDepartmentDto>> GroupByDepartment();
    }
}
