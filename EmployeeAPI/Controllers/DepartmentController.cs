using EmployeeAPI.Contract.Dtos.DepartmentDtos;
using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService deptService;
        private readonly ILogger<DepartmentController> logger;

        public DepartmentController(IDepartmentService deptService, ILogger<DepartmentController> logger)
        {
            this.deptService = deptService;
            this.logger = logger;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("addDepartment")]
        public async Task<ActionResult<string>> AddDepartment([FromBody] DepartmentDto dept)
        {
            var res = await this.deptService.AddDepartment(dept);
            if (res == "success")
            {
                return Ok(new { Message = "Department add Sussessfully", StatusCode = StatusCodes.Status201Created });
            }
            else if (res == "failed")
            {
                return BadRequest(new { Message = "Department Already Exist"});
            }
            else
            {
                return new ObjectResult(res) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize]
        [HttpGet("deparmentDetails/{id}")]
        public async Task<ActionResult<ResponseWithObjectMessage<DepartmentFetchDto>>> GetDepartment(int id)
        {
            var responseMessage = await deptService.GetDepartment(id);
            if (responseMessage.Status == "success")
            {
                responseMessage.StatusCode = StatusCodes.Status200OK;
                return Ok(responseMessage);
            }
            else
            {
                responseMessage.StatusCode = StatusCodes.Status500InternalServerError;
                return new ObjectResult(responseMessage) { StatusCode = (int)StatusCodes.Status500InternalServerError }; 
            }
        }

        [HttpGet("departments")]
        public async Task<ActionResult<ResponseWIthEterableMessage<GroupByDepartmentDto>>> GetAllDepartment()
        {
            logger.LogInformation("Taking Information from claim");
            var responseMessage = await deptService.GetAllDepartment();
            if (responseMessage.Status == "success")
            {
                responseMessage.StatusCode = StatusCodes.Status200OK;
                return Ok(responseMessage);
            }
            else if(responseMessage.Status =="failed")
            {
                responseMessage.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized(responseMessage);
            }
            else
            {
                responseMessage.StatusCode = StatusCodes.Status500InternalServerError;
                return new ObjectResult(responseMessage) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("deleteDepartment/{id}")]
        public async Task<ActionResult<ResponseMsg>> DeleteDepartment(int id)
        {
            var responseMessage = await deptService.DeleteDepartment(id);
            if (responseMessage.Status == "success")
            {
                responseMessage.StatusCode = StatusCodes.Status202Accepted;
                return Ok(responseMessage);
            }else if (responseMessage.Status=="failed")
            {
                responseMessage.StatusCode = StatusCodes.Status404NotFound;
                return NotFound(responseMessage);
            }
            else
            {
                responseMessage.StatusCode = StatusCodes.Status500InternalServerError;
                return new ObjectResult(responseMessage) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        }
        
    }
}
