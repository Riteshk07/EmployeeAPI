using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.LoginDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseMessage>> UserLogin(LoginDto loginDto)
        {
            var resp = await this.loginService.UserLogin(loginDto);
            if(resp.Status == "success")
            {
                return Ok(resp);
            }
            else if(resp.Status == "failed")
            {
                return Unauthorized();
            }else if(resp.Status == "notfound")
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
