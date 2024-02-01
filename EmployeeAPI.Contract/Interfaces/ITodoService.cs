using EmployeeAPI.Contract.Dtos.PaginationDto;
using EmployeeAPI.Contract.Dtos.TodoDtos;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Contract.Interfaces
{
    public interface ITodoService
    {
        /// <summary>
        /// This method used for Assigning the task for Employee. 
        /// Only Admin and Super Admin have permission to Assign task
        /// Super Admin have permission to Assign task to all user but Admin only can assgn task own department's user.
        /// Working:-
        /// 1. Claming and Fetching User Information. 
        /// 2. Fetching user information if EmployeeId is not null.
        /// 3. Assigning Task if EmployeeId is not null otherwise task  will be created.
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="todoDto"></param>
        /// <returns>ResponseWithObjectMessage - Return a Response with TodoFetchDto Object</returns>
        public Task<ResponseWithObjectMessage<TodoFetchDto>> AssignTask( IEnumerable<Claim> claim, TodoDto todoDto);

        /// <summary>
        /// This method used for Remove the task. Only Admin and Super Admin have permission to Remove task
        /// Super Admin have permission to Remove All the task but Admin only can Remove own department task.
        /// Working :- 
        /// 1. Claming and Fetching User Information 
        /// 2. Fetching todo and Delete it.
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="claim"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> RemoveTask(int todoId, IEnumerable<Claim> claim);

        /// <summary>
        /// This method used for update the task information. Only Admin and Super Admin have permission to update task
        /// Super Admin have permission to update All the task but Admin only can update own department task.
        /// Working :-
        /// 1. Claming and Checking User Information for Updating Task
        /// 2. Checking Employee Information and get Task by id and Update
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="claim"></param>
        /// <param name="todoDto"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> UpdateTask(int todoId, IEnumerable<Claim> claim, TodoDto todoDto);

        /// <summary>
        /// This method used for get all the task which is fullfill the requirement of TodoPageDto. Only Admin and Super Admin have permission to update task
        /// Super Admin have permission to fetch All the task but Admin only can fetch own department task.
        /// Working :-
        /// 1. Claming the user information 
        /// 2. Apply Query for Specific user
        /// 3. Apply Query for searching and counting
        /// 4. Filter Completed Task and Get Order
        /// 5. Checking Page Validation
        /// 6. Then get Tasks
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="pageDto"></param>
        /// <returns>ResponseWithDataAndCount<TodoFetchDto></returns>
        public Task<ResponseWithDataAndCount<TodoFetchDto>> GetAllTask(IEnumerable<Claim> claim, TodoPageDto pageDto);

        /// <summary>
        /// This method used for update the task Completion. Only User have permission to update task
        /// task. now user can update own task completion.
        /// Working:- 
        /// 1. Claming the user information
        /// 2. Finding Employee Task
        /// 3. Set Completed
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="todoDto"></param>
        /// <returns>ResponseMsg</returns>
        public Task<ResponseMsg> SetTodoCompleted(int todoId, SetCompletedTodoDto todoDto, IEnumerable<Claim> claim);
    }
}
