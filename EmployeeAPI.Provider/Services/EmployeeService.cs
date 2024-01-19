using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.TodoDtos;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDBContext context;
        private readonly ILoginService loginService;
        private readonly ILogger<EmployeeService> logger;
        private readonly IValidationService validService;

        #region Employee Service Constructors
        public EmployeeService(EmployeeDBContext context, ILoginService loginService, ILogger<EmployeeService> logger, IValidationService validService)
        {
            this.context = context;
            this.loginService = loginService;
            this.logger = logger;
            this.validService = validService;
        }
        #endregion

        // This method is used for fetching all Employee
        // We give a permission only Admin and SuperAdmin can Access Employee
        #region Getting Employees Method
        public async Task<ResponseWIthEterableMessage<EmployeeFetchUpdateDto>> GetEmployees(int id, EmployeeType empType, int deptId, int page)
        {
            ResponseWIthEterableMessage<EmployeeFetchUpdateDto> fetchEmployeeMessage = new ResponseWIthEterableMessage<EmployeeFetchUpdateDto>();
            int startFrom;
            try
            {
                #region Checking Page Validation
                if (page <= 0)
                {
                    fetchEmployeeMessage.Message = "Incorrect Page number";
                    fetchEmployeeMessage.Status = "failed";
                    fetchEmployeeMessage.StatusCode = StatusCodes.Status400BadRequest;
                    fetchEmployeeMessage.IterableData = null;
                    return fetchEmployeeMessage;
                }
                startFrom = (page-1) *10;
                #endregion

                #region Server Validation and Fetching Data
                var employees = new List<Employee>();
                logger.LogInformation("Authorizing");
                    
                logger.LogInformation("Fetching Employees from Database...");
                var sw = Stopwatch.StartNew();
                var qry =
                fetchEmployeeMessage.IterableData = await context.Employees.Include(x => x.Department)
                    .Where(x => empType == Contract.Enums.EmployeeType.Admin ? x.DepartmentID == deptId && x.IsActive : (empType == Contract.Enums.EmployeeType.SuperAdmin && x.IsActive))
                    .Select(emp => new EmployeeFetchUpdateDto()
                    {
                        Id = emp.Id,
                        DepartmentID = emp.DepartmentID,
                        DepartmentName = emp.Department != null ? emp.Department.DepartmentName : "",
                        Name = emp.Name,
                        Email = emp.Email,
                        Phone = emp.Phone,
                        Address = emp.Address,
                        EmployeeType = emp.EmployeeType,
                        City = emp.City,
                        Country = emp.Country
                    })
                    .Skip(startFrom)
                    .Take(10)
                    .ToListAsync();
                sw.Stop();

                var timeElips = sw.ElapsedMilliseconds;
                logger.LogInformation($"Query taking Time in EF Core: {timeElips}");
                fetchEmployeeMessage.Message = "Users Fetched Successfully";
                fetchEmployeeMessage.Status = "success";
                #endregion

                logger.LogInformation("Employee Fetched!");
                return fetchEmployeeMessage;
                
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Exception Endpoint: {ex.StackTrace} \n Message: {ex.Message}");

                fetchEmployeeMessage.Message = "Error From server side";
                fetchEmployeeMessage.Status = "servererr";
                fetchEmployeeMessage.IterableData = null;

                return fetchEmployeeMessage;
                #endregion
            }
        }
        #endregion


        // this method is used for getting employee details
        #region Getiing Current User Method
        public async Task<ResponseWithObjectMessage<EmployeeFetchUpdateDto>> CurrentUser(int id)
        {
            ResponseWithObjectMessage<EmployeeFetchUpdateDto> message = new ResponseWithObjectMessage<EmployeeFetchUpdateDto>();
            try
            {
                #region Server Validation and Fetching Data
                logger.LogInformation("Fetching User Information");
                var emp = await context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id ==id);

                if (emp != null)
                {
                    message.Data = new EmployeeFetchUpdateDto();
                    message.Data.Id = emp.Id;
                    message.Data.DepartmentID = emp.DepartmentID;
                    message.Data.Name = emp.Name;
                    message.Data.Email = emp.Email;
                    message.Data.Phone = emp.Phone;
                    message.Data.Address = emp.Address;
                    message.Data.EmployeeType = emp.EmployeeType;
                    message.Data.City = emp.City;
                    message.Data.Country = emp.Country;
                    message.Data.DepartmentName = emp.Department!= null ? emp.Department.DepartmentName:"";

                    message.Status = "success";
                    message.Message = "User Fetched Successfully";
                    message.StatusCode = 200;
                    logger.LogInformation("user Fetched successfully");
                    return message;
                }
                else
                {
                    logger.LogWarning($"Error Generated: User Id is incorrect \nMessage: User not found in database");
                    message.Status = "failed";
                    message.Message = "User Not found";
                    message.StatusCode = 404;
                    message.Data = null;
                    return message;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Exception Endpoint: {ex.StackTrace} \n Message: {ex.Message}");
                message.Status = "servererr";
                message.Message = "Error from server side";
                message.StatusCode = 500;
                message.Data = null;
                return message;
                #endregion
            }
        }
        #endregion


        // this method is used for Adding employee 
        #region Employee Registration
        public async Task<ResponseMsg> EmployeeRegistration(EmployeeDto emp)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                #region Employee Validation and Registration
                logger.LogInformation($"Validating Employee...");
                if (!emp.Name.IsNullOrEmpty() && validService.ValidateEmail(emp.Email) && validService.ValidatePassword(emp.Password))
                {
                    logger.LogInformation($"Validated");
                    Employee obj = new Employee
                    {
                        Name = emp.Name,
                        Email = emp.Email,
                        Address = emp.Address,
                        City = emp.City,
                        Country = emp.Country,
                        DepartmentID = emp.DepartmentID,
                        EmployeeType = emp.EmployeeType,
                        Phone = emp.Phone
                    };
                    var savedEmp =  await context.Employees.AddAsync(obj);
                    
                    await context.SaveChangesAsync();
                    
                    await context.Employees.SingleAsync(em => em.Email == emp.Email);


                    logger.LogInformation($"Employees: {savedEmp.Entity.Name} Adding");

                    Login login = new Login
                    {
                        Password = emp.Password,
                        Email = emp.Email,
                        EmployeeID = savedEmp.Entity.Id
                    };
                    context.Logins.Add(login);
                    context.SaveChanges();

                    message.Message = "User saved Successfully";
                    message.StatusCode = 201;
                    message.Status = "success";
                    logger.LogInformation($"Employees: {savedEmp.Entity.Name} Saved in the database");
                    return message;
                }
                else
                {
                    logger.LogWarning("User should have to provide Required field");
                    message.Message = "You have to provide some required field | Invalid Email or password";
                    message.StatusCode = 400;
                    message.Status = "failed";
                    return message;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Exception Endpoint: {ex.StackTrace} \n Message: {ex.Message}");
                message.Message = "Error from server side";
                message.StatusCode = 500;
                message.Status = "servererr";
                return message;

                #endregion
            }
        }
        #endregion



        // this method is used for Updating employee details
        // In this method it will Update Email first from login database then update user 
        #region Employee Updation
        public async Task<ResponseMsg> UpdateEmployee(int userId, int id, EmployeeType empType, EmployeeFetchUpdateDto emp)
        {
            ResponseMsg updateEmployeeMessage = new ResponseMsg();

            try
            {
                #region Part of Validation
                logger.LogInformation("Validaing...");
                if (empType == EmployeeType.User && userId != id)
                {
                    updateEmployeeMessage.Status = "unauth";
                    updateEmployeeMessage.StatusCode = StatusCodes.Status401Unauthorized;
                    updateEmployeeMessage.Message = "You are not an Admin or SuperAdmin, You don't have the permission to update other employee";
                    return updateEmployeeMessage;
                }else if (!validService.ValidateEmail(emp.Email))
                {
                    updateEmployeeMessage.Status = "invalid";
                    updateEmployeeMessage.StatusCode = StatusCodes.Status400BadRequest;
                    updateEmployeeMessage.Message = "Invalid Email | minimum 8 charecter one uppercase and one lower case and a special charecter";
                    logger.LogInformation($"User Id: {id}| Invalid Email");
                    return updateEmployeeMessage;
                }
                #endregion

                #region Server Validation, Updation and Response
                logger.LogInformation($"Checking information");
                var user = await context.Employees.FindAsync(id);
                

                if (user != null)
                {
                    #region Checking Admin Authorization
                    if (empType == EmployeeType.Admin && user.EmployeeType == EmployeeType.SuperAdmin)
                    {
                        updateEmployeeMessage.Status = "failed";
                        updateEmployeeMessage.StatusCode = 401;
                        updateEmployeeMessage.Message = "You don't have the permission to update super admin";
                        logger.LogWarning($"{updateEmployeeMessage.Message} \nAdmin Id: {userId} are trying to update Super Admin");
                        return updateEmployeeMessage;
                    }
                    #endregion

                    #region Updating Email from Logins
                    if (user.Email != emp.Email )
                    {
                        logger.LogInformation("Updating Email from login database");
                        var logRes = await loginService.ChangeEmail(user.Email, emp.Email);
                        if (logRes.Status != "success")
                        {
                            logger.LogWarning($"Message: {logRes.Message}");
                            return logRes;
                        }
                    }
                    #endregion

                    #region Updating Employee
                    user.Name = emp.Name;
                    user.Email = emp.Email;
                    user.Phone = emp.Phone;
                    user.City = emp.City;
                    user.Country = emp.Country;
                    user.EmployeeType = emp.EmployeeType;
                    user.Address = emp.Address;
                    user.DepartmentID = emp.DepartmentID;

                    logger.LogInformation("Updating User from Employee database");
                    context.Employees.Update(user);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Employee Updated");

                    updateEmployeeMessage.Message = "User Updated successfully";
                    updateEmployeeMessage.Status = "success";
                    updateEmployeeMessage.StatusCode = StatusCodes.Status200OK;

                    return updateEmployeeMessage;
                    #endregion
                }
                else
                {
                    logger.LogWarning($"Provided invalid id: {id}, User not found");
                    updateEmployeeMessage.Message = "User Not found";
                    updateEmployeeMessage.Status = "failed";
                    updateEmployeeMessage.StatusCode = StatusCodes.Status404NotFound;

                    return updateEmployeeMessage;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Exception Endpoint: {ex.StackTrace} \n Message: {ex.Message}");
                Console.WriteLine(ex.Message);
                updateEmployeeMessage.Message = "Error from server side";
                updateEmployeeMessage.Status = "servererr";
                updateEmployeeMessage.StatusCode = StatusCodes.Status500InternalServerError;
                return updateEmployeeMessage;
                #endregion
            }
        }
        #endregion


        // That is used for deleting Employee
        #region Employee Deleting
        public async Task<ResponseMsg> DeleteEmployee(int id)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                #region Server Validation and Deleting
                logger.LogInformation($"Checking information");
                var user = await context.Employees.FindAsync(id);
                
                if (user != null)
                {
                    user.IsActive = false;
                    context.Employees.Update(user);
                    await context.SaveChangesAsync();   
                    message.Message = "User Deleted Successfully";
                    message.Status = "success";
                    message.StatusCode = StatusCodes.Status202Accepted;
                    logger.LogInformation("User Saved successfully");
                    return message;
                }
                else
                {
                    logger.LogWarning($"User Id={id} not found in the database");
                    message.Message = "User Not found for deletion";
                    message.Status = "failed";
                    message.StatusCode = StatusCodes.Status404NotFound;
                    return message;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handling Exception
                logger.LogError($"Exception Endpoint: {ex.StackTrace} \n Message: {ex.Message}");
                message.Message = "Error from server side";
                message.Status = "servererr";
                message.StatusCode = StatusCodes.Status500InternalServerError;
                return message;
                #endregion
            }
        }
        #endregion
    }
}
