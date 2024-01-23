using EmployeeAPI.Contract.Dtos.DepartmentDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class DepartmentService: IDepartmentService
    {
        private readonly EmployeeDBContext context;
        private readonly ILogger<DepartmentService> logger;

        public DepartmentService(EmployeeDBContext context, ILogger<DepartmentService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        #region Adding Department Method
        public async Task<string> AddDepartment(DepartmentDto dept)
        {
            try
            {
                logger.LogInformation("Adding Department...");
                Department department = new Department() {
                    DepartmentName = dept.DepartmentName
                };
                await context.Departments.AddAsync(department);
                context.SaveChanges();
                logger.LogInformation("Department Added Successfully");
                return "success";
            }catch (Exception ex)
            {
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");

                return "servererr";
            }
        }
        #endregion

        #region Get All Department Method 
        public async Task<ResponseWIthEterableMessage<DepartmentFetchDto>> GetAllDepartment()
        {
            ResponseWIthEterableMessage<DepartmentFetchDto> message = new ResponseWIthEterableMessage<DepartmentFetchDto>();
            try
            {
                #region Fetching All Departments 
                logger.LogInformation($"Feching User Information for Get All Department");

                logger.LogInformation("Checking Employee Type...");

                var departments = await context.Departments.Where(d => d.IsActive).Select(dept => new DepartmentFetchDto()
                {
                    Id = dept.Id,
                    DepartmentName = dept.DepartmentName
                }).ToListAsync();


                message.IterableData = departments;
                message.Status = "success";
                message.Message = "All Department fetched successfully";
                logger.LogInformation($"{message.Message}");
                #endregion

                return message;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                message.IterableData = null;
                message.Status = "servererr";
                message.Message = "Error from server side";
                #endregion

                return message;
            }
        }
        #endregion

        #region Get only Department Method
        public async Task<ResponseWithObjectMessage<DepartmentFetchDto>> GetDepartment(int id)
        {
            ResponseWithObjectMessage<DepartmentFetchDto> message = new ResponseWithObjectMessage<DepartmentFetchDto>();
            try
            {
                logger.LogInformation($"Checking Department Information by This Id: {id}");
                var department = await context.Departments.FindAsync(id);
                if(department != null)
                {
                    #region Checking User Information
                    if (!department.IsActive)
                    {
                        message.Status = "failed";
                        message.Message = "Department Is Inactive or Deleted, Please contact to Administreter";
                        message.StatusCode = StatusCodes.Status404NotFound;
                        return message;
                    }
                    #endregion

                    #region Getting Department and Response
                    logger.LogInformation("Getting Department");
                    message.Data = new DepartmentFetchDto() { 
                        DepartmentName= department.DepartmentName,
                        Id = department.Id
                    };
                    

                    message.Status = "success";
                    message.Message = "All Employee Department fetched successfully";
                    logger.LogInformation($"{message.Message}");
                    #endregion

                    return message;
                }
                else
                {
                    #region Department not found Response 
                    message.Data = null;
                    message.Status = "failed";
                    message.Message = "Department not found";
                    logger.LogWarning($"{message.Message}, Id: {id} are trying to found department");
                    #endregion

                    return message;
                }
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                message.Data = null;
                message.Status = "servererr";
                message.Message = "Error from server side";
                #endregion

                return message;
            }
        }
        #endregion

        #region Delete Department Method
        public async Task<ResponseMsg> DeleteDepartment(int id)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                logger.LogInformation($"Checking Department Information for Deleting by this Id: {id}");
                #region Getting Departments
                var department = await context.Departments.FindAsync(id);
                
                if (department != null)
                {
                    logger.LogInformation("Department Found Successfully");
                    department.IsActive = false;
                    context.Departments.Update(department);
                    await context.SaveChangesAsync();
                    message.Status = "success";
                    message.Message = "Department deleted successfully";
                    logger.LogInformation($"{message.Message}");
                    return message;
                }
                #endregion
                else
                {
                    #region Department not found Response
                    message.Status = "failed";
                    message.Message = "Department not found for deletion";
                    logger.LogWarning($"{message.Message} \nDepartment Id: {id} are trying to delete");
                    #endregion
                    return message;
                }
                
            }catch(Exception ex)
            {
                #region Handling Response
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                message.Status = "servererr";
                message.Message = "Error From server side, Deletion failed";
                #endregion
                return message;
            }
        }
        #endregion

        #region Group by Department Method 
        public async Task<ResponseWIthEterableMessage<GroupByDepartmentDto>> GroupByDepartment()
        {
            ResponseWIthEterableMessage<GroupByDepartmentDto> message = new ResponseWIthEterableMessage<GroupByDepartmentDto>();
            
            try
            {
                #region Getting Group By department
                var groupedEmployees = await context.Employees.GroupBy(e => e.DepartmentID)
                    .Select(group => new GroupByDepartmentDto
                    {
                        Id = group.Key,
                        DepartmentName = group.Count() > 0 ? group.First().Department.DepartmentName: "",
                        EmployeesCount = group.ToList().Count()
                    }).ToListAsync();

                message.IterableData = groupedEmployees;
                message.StatusCode = StatusCodes.Status200OK;
                message.Status = "success";
                message.Message = "Departments Counted Successfully";
                #endregion

                return message;
            }catch( Exception ex )
            {
                #region Handling Exception
                logger.LogError($"");
                message.IterableData = null;
                message.StatusCode = 500;
                message.Status = "failed";
                message.Message = "Error from server side";
                #endregion

                return message;
            }
        }
        #endregion
    }
}
