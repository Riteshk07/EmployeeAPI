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
        /// <summary>
        /// Adding Deparment in Database
        /// </summary>
        /// <param name="dept"></param>
        /// <returns>string</returns>
        public Task<string> AddDepartment(DepartmentDto dept);

        /// <summary>
        /// Fetching all departments 
        /// </summary>
        /// <returns>ResponseWIthEterableMessage - Return a Response with List of Department (DepartmentFetchDto).</returns>
        public Task<ResponseWIthEterableMessage<DepartmentFetchDto>> GetAllDepartment();

        /// <summary>
        /// Get Department Details by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResponseWithObjectMessage - Return Response with DepartmentFetchDto object (Department details)</returns>
        public Task<ResponseWithObjectMessage<DepartmentFetchDto>> GetDepartment(int id);

        /// <summary>
        /// Delete a department by it's Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> DeleteDepartment(int id);

        /// <summary>
        /// Fetch all Group by departments details by and count of employee on percular department.
        /// </summary>
        /// <returns>ResponseWIthEterableMessage - Return Responese with List of GroupByDepartmentDto object.</returns>
        public Task<ResponseWIthEterableMessage<GroupByDepartmentDto>> GroupByDepartment();
    }
}
