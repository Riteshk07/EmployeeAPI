using EmployeeAPI.Contract.Dtos.TodoDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace EmployeeAPI.Controllers
{
    [Route("api/todo")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService todoService;
        private readonly ILogger<TodoController> logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            this.todoService = todoService;
            this.logger = logger;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("add")]
        public async Task<ActionResult<ResponseWithObjectMessage<TodoFetchDto>>> AddTodo([FromBody] TodoDto todoDto)
        {
            logger.LogInformation("Taking Information from claim");
            int userId = Convert.ToInt32(HttpContext.User.Claims.First(e => e.Type == "Id").Value);

            IEnumerable<Claim> claim = HttpContext.User.Claims;

            var responseMessage = await todoService.AssignTask(claim, todoDto);
            if (responseMessage.Status == "success")
            {
                return Ok(responseMessage);
            } else if (responseMessage.Status == "failed")
            {
                return Unauthorized(responseMessage);
            }
            else if (responseMessage.Status == "notfound")
            {
                return NotFound(responseMessage);
            }
            else
            {
                return BadRequest(responseMessage);
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("remove/{todoId}")]
        public async Task<ActionResult<ResponseMsg>> RemoveTodo(int todoId)
        {
            logger.LogInformation("Taking Information from claim");
            IEnumerable<Claim> claim = HttpContext.User.Claims;
            var responseMessage = await todoService.RemoveTask(todoId, claim);
            if (responseMessage.Status == "success")
            {
                return Ok(responseMessage);
            }
            else if (responseMessage.Status == "failed")
            {
                return Unauthorized(responseMessage);
            }
            else if (responseMessage.Status == "notfound")
            {
                return NotFound(responseMessage);
            }
            else
            {
                return BadRequest(responseMessage);
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update/{todoId}")]
        public async Task<ActionResult<ResponseMsg>> UpdateTask(int todoId, [FromBody] TodoDto todoDto)
        {
            logger.LogInformation("Taking Information from claim");

            IEnumerable<Claim> claim = HttpContext.User.Claims;
            var responseMessage = await todoService.UpdateTask(todoId, claim, todoDto);
            if (responseMessage.Status == "success")
            {
                return Ok(responseMessage);
            }
            else if (responseMessage.Status == "failed")
            {
                return Unauthorized(responseMessage);
            }
            else if (responseMessage.Status == "notfound")
            {
                return NotFound(responseMessage);
            }
            else
            {
                return BadRequest(responseMessage);
            }
        }

        [Authorize]
        [HttpGet("tasks/{Page}")]
        public async Task<ActionResult<ResponseWIthEterableMessage<TodoFetchDto>>> GetAllTask(int Page)
        {
            logger.LogInformation("Taking Information from claim");

            IEnumerable<Claim> claim = HttpContext.User.Claims;
            var responseMessage = await todoService.GetAllTask(claim, Page);
            if (responseMessage.Status == "success")
            {
                return Ok(responseMessage);
            }else if (responseMessage.Status == "notfound")
            {
                return NotFound(responseMessage);
            }else if(responseMessage.Status == "failed")
            {
                return Unauthorized(responseMessage);
            }
            else
            {
                return BadRequest(responseMessage);
            }
        }

        [Authorize]
        [HttpPost("SetTodoCompleted/{TodoId}")]
        public async Task<ActionResult<ResponseMsg>> SetTodoCompleted([FromBody]SetCompletedTodoDto setCompletedTodo, int TodoId)
        {
            var resp = await todoService.SetTodoCompleted(TodoId, setCompletedTodo);
            if(resp.StatusCode== 200)
            {
                return Ok(resp);
            }else if(resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }
    }
}
