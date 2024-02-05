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
        /// <summary>
        /// This Method is used for fetching all the Employee. Only Admin and Super Admin have the permission to fetch the Employee.
        /// Super Admin can fetch all the Emplyee but Admin only can Fetch own Department Employee.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="empType"></param>
        /// <param name="deptId"></param>
        /// <param name="pageDto"></param>
        /// <returns>ResponseWithDataAndCount - Get Response with Employees (List of EmployeeFetchUpdateDto) and Count (Total Employees)</returns>
        public Task<ResponseWithDataAndCount<EmployeeFetchUpdateDto>> GetEmployees(int id, EmployeeType empType, int deptId, PageDto pageDto);

        /// <summary>
        /// This method is used for current user (Logged User) details 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResponseWithObjectMessage - Return a response with EmployeeFetchUpdateDto object</returns>
        public Task<ResponseWithObjectMessage<EmployeeFetchNotificationDto>> CurrentUser(int id);

        /// <summary>
        /// This method is used for User Registration, Validating User Information and Email then Add to database.
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> EmployeeRegistration(EmployeeDto employee);

        /// <summary>
        /// this method is used for Updating employee details
        /// In this method it will Update Email first from login database then update user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="empType"></param>
        /// <param name="employee"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> UpdateEmployee(int userId ,int id,EmployeeType empType, EmployeeFetchUpdateDto employee);

        /// <summary>
        /// this method is used for Delete employee.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> DeleteEmployee(int id);
    }
}
