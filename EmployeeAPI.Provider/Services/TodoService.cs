﻿using EmployeeAPI.Contract.Dtos.TodoDtos;
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
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class TodoService: ITodoService
    {
        private readonly EmployeeDBContext context;
        private readonly ILogger<TodoService> logger;

        #region Constructor
        public TodoService(EmployeeDBContext context, ILogger<TodoService> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        #endregion

        #region Method for Task Assigning for Employee
        public async Task<ResponseWithObjectMessage<TodoFetchDto>> AssignTask(IEnumerable<Claim> claim, TodoDto todoDto)
        {
            ResponseWithObjectMessage<TodoFetchDto> todoAssignTaskMessage = new ResponseWithObjectMessage<TodoFetchDto>();
            try
            {
                logger.LogInformation("Claiming Current user information for Assigning Task");

                #region Claming Information
                int userId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), claim.First(e => e.Type == "Role").Value);
                int DeptId = Convert.ToInt32(claim.First(e => e.Type == "DeptId").Value);
                #endregion

                logger.LogInformation("Checking User Information for Assigning Task");
                
                
                    Todo todo = new Todo();
                    if (empType == Contract.Enums.EmployeeType.SuperAdmin || (empType == Contract.Enums.EmployeeType.Admin))
                    {
                        #region Assigning Task
                        logger.LogInformation("Assigning Task");
                        todoAssignTaskMessage.Data = new TodoFetchDto();
                        todoAssignTaskMessage.Data.Title =  todo.Title = todoDto.Title;
                        todoAssignTaskMessage.Data.Description = todo.Description = todoDto.Description;
                        todoAssignTaskMessage.Data.IsCompleted = todo.IsCompleted = todoDto.IsCompleted;
                        todoAssignTaskMessage.Data.EmployeeId = null;
                        todoAssignTaskMessage.Data.AssignBy = todo.AssignBy = empType;
                        todo.AssignById = userId;
                        todo.DepartmentId = empType == EmployeeType.Admin ? DeptId : null;

                        await context.Todos.AddAsync(todo);
                        await context.SaveChangesAsync();


                        todoAssignTaskMessage.Message = "Task Assign Successfully";
                        todoAssignTaskMessage.Status = "success";
                        logger.LogInformation($"{todoAssignTaskMessage.Message} By This userId: {userId}");
                        #endregion
                        return todoAssignTaskMessage;
                    }
                    else
                    {
                        #region Unauthorized User Response
                        logger.LogWarning($"User Id: {userId} does not have the permission to access for Assigning Task.\nOnly Admin and Super Admin have permission to Access.");
                        todoAssignTaskMessage.Message = "You are not a Admin or Super Admin or not a Admin from perticular department";
                        todoAssignTaskMessage.Status = "failed";
                        todoAssignTaskMessage.Data = null;
                        todoAssignTaskMessage.StatusCode = 401;
                        #endregion

                        return todoAssignTaskMessage;
                    }
                
               
            }
            catch (Exception ex)
            {
                #region Handling Exception
                Console.WriteLine(ex.Message);
                todoAssignTaskMessage.Message = "Error from server side";
                todoAssignTaskMessage.Status = "servererr";
                todoAssignTaskMessage.Data = null;
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                #endregion
                return todoAssignTaskMessage;
            }

        }
        #endregion

        #region Remove or Delete Task Method
        public async Task<ResponseMsg> RemoveTask(int todoId, IEnumerable<Claim> claim)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                #region Claming Information
                int userId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), claim.First(e => e.Type == "Role").Value);
                int DeptId = Convert.ToInt32(claim.First(e => e.Type == "DeptId").Value);
                #endregion
                logger.LogInformation("Checking User Information for Removing Task");

                
                logger.LogInformation("Finding Task...");
                #region Fetching Todo Information And Delete It
                var todo = await context.Todos.FindAsync(todoId);
                if (todo == null)
                {
                    #region Todo Null Response
                    message.Message = "Todo Not found";
                    message.Status = "failed";
                    message.StatusCode = 404;
                    logger.LogWarning($"{message.Message} by this Id: {userId}");
                    #endregion

                    return message;
                }
                logger.LogInformation("Checking User Type");

                #region Task Deletion
                if (empType == Contract.Enums.EmployeeType.SuperAdmin || (empType == Contract.Enums.EmployeeType.Admin && DeptId == todo.DepartmentId))
                {
                    context.Todos.Remove(todo);
                    await context.SaveChangesAsync();
                        
                    message.Message = "Task Removed Successfully";
                    message.Status = "success";
                    message.StatusCode = 202;
                    logger.LogWarning($"{message.Message}, by this Id: {userId}");
                }
                #endregion

                #endregion

                return message;
            }
            catch (Exception ex)
            {
                #region Handling Exception
                Console.WriteLine(ex.Message);
                message.Message = "Error from server side";
                message.Status = "failed";
                message.StatusCode = 500;
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                #endregion

                return message;
            }
        }
        #endregion

        #region Method for Task Updation
        public async Task<ResponseMsg> UpdateTask(int todoId, IEnumerable<Claim> claim, TodoDto todoDto)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                #region Claming Information
                int userId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), claim.First(e => e.Type == "Role").Value);
                int DeptId = Convert.ToInt32(claim.First(e => e.Type == "DeptId").Value);
                #endregion

                logger.LogInformation("Checking User Information for Updating Task");
                var emp = await context.Employees.Include(d => d.Department)
                        .FirstOrDefaultAsync(e => e.Id == todoDto.EmployeeId);
                if (emp != null)
                {
                    if (emp.EmployeeType == EmployeeType.SuperAdmin)
                    {
                        message.Message = "User don't have a permission to assign task to SuperAdmin";
                        message.Status = "failed";
                        message.StatusCode = 401;
                        return message;
                    } 
                }

                logger.LogInformation("Finding Task...");
                var todo = await context.Todos.FindAsync(todoId);

                #region Task Not Found Response 
                if (todo == null)
                {
                    message.Message = "Task Not found";
                    message.Status = "notfound";
                    logger.LogWarning($"{message.Message} by this Id: {userId}");
                    return message;
                }
                #endregion

                #region Task Updating
                logger.LogInformation($"Checking User Type for updating task to this Id: {todo.EmployeeId}");
                if (empType == Contract.Enums.EmployeeType.SuperAdmin || (empType == Contract.Enums.EmployeeType.Admin && DeptId == todo.DepartmentId))
                {
                    todo.Title = todoDto.Title.IsNullOrEmpty()? todo.Title: todoDto.Title;
                    todo.Description = todoDto.Description.IsNullOrEmpty() ? todo.Description: todoDto.Description;
                    todo.IsCompleted = todoDto.IsCompleted;
                    if (emp != null)
                    {
                        todo.EmployeeId = emp?.Id;
                        todo.DepartmentId = emp?.DepartmentID;
                    }
                    todo.AssignBy = empType;
                    todo.AssignById = userId;

                    context.Todos.Update(todo); 
                    await context.SaveChangesAsync();

                    message.Message = "Task Updated Successfully";
                    message.Status = "success";
                    logger.LogInformation($"{message.Message} by this Id: {userId}");

                }
                else
                {
                    message.Message = "User don't have a permission to update Task to diffrent Department";
                    message.Status = "failed";
                    message.StatusCode = 401;
                }
                #endregion
                return message;
                
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Console.WriteLine(ex.Message);
                message.Message = "Error from server side";
                message.Status = "servererr";
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                #endregion

                return message;
            }
        }
        #endregion

        #region Get All Task Method 
        public async Task<ResponseWIthEterableMessage<TodoFetchDto>> GetAllTask(IEnumerable<Claim> claim, int page)
        {
            List<Todo> todos;
            ResponseWIthEterableMessage<TodoFetchDto> message = new ResponseWIthEterableMessage<TodoFetchDto>();
            try
            {
                #region Claming Information
                int userId = Convert.ToInt32(claim.First(e => e.Type == "Id").Value);
                EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), claim.First(e => e.Type == "Role").Value);
                int DeptId = Convert.ToInt32(claim.First(e => e.Type == "DeptId").Value);
                #endregion

                #region Apply Query for Specific user
                logger.LogInformation("Checking User Information to Get All Task");

                logger.LogInformation("Checking Employee Type...");
                logger.LogInformation("Finding Task...");
                int startFrom = (page - 1) *10;
                if (empType == Contract.Enums.EmployeeType.SuperAdmin)
                {
                    todos = await context.Todos.Include(e=> e.Employee)
                        .Skip(startFrom).Take(10).ToListAsync();
                }
                else if (empType == Contract.Enums.EmployeeType.Admin)
                {
                    todos = await context.Todos.Include(e => e.Employee)
                        .Where(y =>y.DepartmentId == DeptId).Skip(startFrom).Take(10).ToListAsync();
                }
                else
                {
                    todos = await context.Todos.Include(e => e.Employee)
                        .Where(z => z.EmployeeId == userId).Skip(startFrom).Take(10).ToListAsync();
                }
                var departments = await context.Departments.ToListAsync();
                #endregion


                if (todos != null)
                {
                    #region Inserting Data to Iterable Data
                    logger.LogInformation("Fetching Tasks...");
                    message.IterableData = new List<TodoFetchDto>();
                    foreach (var todo in todos)
                    {
                        message.IterableData.Add(new TodoFetchDto()
                        {
                            Id= todo.Id,
                            Title = todo.Title,
                            Description = todo.Description,
                            IsCompleted = todo.IsCompleted,
                            EmployeeId = todo.EmployeeId,
                            EmployeeName = todo.Employee?.Name,
                            DepartmentId = todo.DepartmentId,
                            DepartmentName = departments.Where(d => d.Id == todo.DepartmentId).FirstOrDefault()?.DepartmentName,
                            AssignBy = todo.AssignBy
                        });
                    }
                    message.Status = "success";
                    message.Message = "Tasks fetched successfully";
                    message.StatusCode = 200;
                    logger.LogInformation($"{message.Message} by This Id: {userId}");
                    #endregion

                    return message;
                }
                else
                {
                    #region Task Not found Response
                    message.Status = "failed";
                    message.Message = "Todos not found";
                    message.StatusCode = 404;
                    message.IterableData = null;
                    logger.LogWarning($"{message.Message} by this Id: {userId}");
                    #endregion

                    return message;
                }
            }
            catch(Exception ex)
            {
                #region Handling Exception
                Console.WriteLine(ex.Message);
                message.Status = "servererr";
                message.Message = "Error from server side";
                message.StatusCode = 500;
                message.IterableData = null;
                #endregion

                return message;
            }
        }
        #endregion
        public async Task<ResponseMsg> SetTodoCompleted(int todoId,  SetCompletedTodoDto todoDto)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == todoId);
                if (todo != null)
                {
                    todo.IsCompleted = todoDto.IsCompleted;
                    context.Todos.Update(todo);
                    await context.SaveChangesAsync();
                    message.Status = "success";
                    message.Message = "Tasks Updated successfully";
                    message.StatusCode = 200;
                    logger.LogInformation($"{message.Message}");
                    return message;
                }
                else
                {
                    #region Task Not found Response
                    message.Status = "failed";
                    message.Message = "Todos not found";
                    message.StatusCode = 404;
                    logger.LogWarning($"{message.Message}");
                    #endregion

                    return message;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                message.Status = "servererr";
                message.Message = "Error from server side";
                message.StatusCode = 500;
                return message;
            }
        }
    }
}
