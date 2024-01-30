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
        public Task<ResponseWithObjectMessage<TodoFetchDto>> AssignTask( IEnumerable<Claim> claim, TodoDto todoDto);

        public Task<ResponseMsg> RemoveTask(int todoId, IEnumerable<Claim> claim);

        public Task<ResponseMsg> UpdateTask(int todoId, IEnumerable<Claim> claim, TodoDto todoDto);

        public Task<ResponseWIthEterableMessage<TodoFetchDto>> GetAllTask(IEnumerable<Claim> claim, int Page);

    }
}
