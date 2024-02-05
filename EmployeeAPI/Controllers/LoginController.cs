using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.LoginDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using Microsoft.AspNetCore.Authorization;
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
            
            HttpContext.Items.TryGetValue("", out object? IPAddress);
            string IpAddress = IPAddress!= null? (string)IPAddress:"IPAddress";
            var resp = await this.loginService.UserLogin(loginDto, IpAddress);
            if(resp.Status == "success")
            {
                return Ok(resp);
            }
            else if(resp.Status == "failed")
            {
                return Unauthorized(resp);
            }else if(resp.Status == "notfound")
            {
                return NotFound(resp);
            }
            else
            {
                return BadRequest(resp);
            }
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ResponseMsg>> ChangePassword([FromBody] LoginPasswordDto data)
        {
            var e1 = HttpContext.User.Claims;
            var e2= HttpContext.User.Claims.First(c => c.Type == "Email");
            string email = HttpContext.User.Claims.First(c => c.Type == "Email").Value;
            var resp = await loginService.ChangePassword(data, email);
            if(resp.StatusCode == 200) { 
                return Ok(resp);
            }else if(resp.StatusCode == 401)
            {
                return Unauthorized(resp);
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
