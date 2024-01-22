using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace EmployeeAPI.Controllers
{
    [Route("api")]
    [ApiController]

    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService empService;
        // private readonly ILogger<EmployeeController> logger;
        private readonly ILogger logger;

        public EmployeeController(IEmployeeService _empService, ILogger<EmployeeController> logger) {
            empService = _empService;
            this.logger = logger;
        }
        
        /*
        // For Creating Custom Category
        public EmployeeController(IEmployeeService _empService, ILoggerFactory factory)
        {
            empService = _empService;
            this.logger = factory.CreateLogger("Custom.Employee");
        }
        */


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("employees/{page}")]
        public async Task<ActionResult<ResponseWIthEterableMessage<EmployeeFetchUpdateDto>>> GetEmployee(int page) {
            logger.LogInformation("Request Recieved for fetching Employee...");

            logger.LogInformation("Taking Information from claim");
            int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
            int deptId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "DeptId").Value);
            string type = HttpContext.User.Claims.First(c => c.Type== "Role").Value;
            
            EmployeeType empType = (EmployeeType)Enum.Parse(typeof(EmployeeType), type);
            
            var resMessage = await empService.GetEmployees(userId, empType, deptId ,page);

            if (resMessage.Status == "success")
            {
                resMessage.StatusCode = StatusCodes.Status200OK;
                return Ok(resMessage);
            } else if (resMessage.Status == "failed")
            {
                resMessage.StatusCode = StatusCodes.Status401Unauthorized;
                return Unauthorized(resMessage);
            } else if(resMessage.Message == "notfound")
            {
                resMessage.StatusCode = StatusCodes.Status404NotFound;
                return NotFound(resMessage);
            }
            else
            {
                resMessage.StatusCode = StatusCodes.Status500InternalServerError;
                return BadRequest(resMessage);
            }
        }

        [HttpPost("registration")]
        public async Task<ActionResult<ResponseMsg>> Registration([FromBody] EmployeeDto emp)
        {
            logger.LogInformation("Request Recieved for Adding Employees");
            var resp = await empService.EmployeeRegistration(emp);
            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }
            else if (resp.StatusCode == 201)
            {
                return Ok(resp);
            }
            else if (resp.StatusCode == 202)
            {
                return Accepted(resp);
            }
            else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else if (resp.StatusCode == 401)
            {
                return Unauthorized(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }


        [Authorize]
        [HttpGet("userDetails")]
        public async Task<ActionResult<ResponseWithObjectMessage<EmployeeFetchUpdateDto>>> CurrentUserDetail()
        {
            logger.LogInformation("Request Recieved for fetching Employee...");

            logger.LogInformation("Taking Information from claim");
            int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

            var resp = await empService.CurrentUser(userId);

            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }
            else if (resp.StatusCode == 202)
            {
                return Accepted(resp);
            }
            else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else if (resp.StatusCode == 401)
            {
                return Unauthorized(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }

        [Authorize]
        [HttpPut("updateuser/{id}")]
        public async Task<ActionResult<ResponseMsg>> UpdateUser(int id, [FromBody] EmployeeFetchUpdateDto updateEmployeeDto)
        {
            string type = HttpContext.User.Claims.First(e => e.Type == "Role").Value;
            int userId = Convert.ToInt32(HttpContext.User.Claims.First(e => e.Type == "Id").Value);
            EmployeeType empType =(EmployeeType) Enum.Parse(typeof(EmployeeType), type);

            var resp = await  empService.UpdateEmployee(userId ,id,empType, updateEmployeeDto);
            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }
            else if (resp.StatusCode == 202)
            {
                return Accepted(resp);
            }
            else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else if (resp.StatusCode == 401)
            {
                return Unauthorized(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("deleteEmployee/{id}")]
        public async Task<ActionResult<ResponseMsg>> DeleteEmployee(int id)
        {
            var resp = await  empService.DeleteEmployee(id);

            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }else if(resp.StatusCode == 202)
            {
                return Accepted(resp);
            }
            else if (resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else if (resp.StatusCode == 401)
            {
                return Unauthorized(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }
    }
}
