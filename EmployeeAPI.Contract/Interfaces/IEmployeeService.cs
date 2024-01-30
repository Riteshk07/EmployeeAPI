using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.PaginationDto;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface IEmployeeService
    {
        public Task<ResponseWIthEterableMessage<EmployeeFetchUpdateDto>> GetEmployees(int id, EmployeeType empType, int deptId, PageDto pageDto);

        public Task<ResponseWithObjectMessage<EmployeeFetchUpdateDto>> CurrentUser(int id);

        public Task<ResponseMsg> EmployeeRegistration(EmployeeDto employee);


        public Task<ResponseMsg> UpdateEmployee(int userId ,int id,EmployeeType empType, EmployeeFetchUpdateDto employee);

        public Task<ResponseMsg> DeleteEmployee(int id);
    }
}
