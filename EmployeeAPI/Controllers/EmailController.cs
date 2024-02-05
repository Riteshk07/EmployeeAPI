using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService emailService;

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        [HttpPost("forgetpassword")]
        public async Task<ActionResult<ResponseMsg>> ForgetPassword([FromBody] string email)
        {
            HttpContext.Items.TryGetValue("", out object? IPAddress);
            string IpAddress = IPAddress != null ? (string)IPAddress : "IPAddress";
            var resp = await emailService.ForgetPassword(email, IpAddress);
            if (resp.StatusCode == 200)
            {
                Ok(resp);
            }
            else if (resp.StatusCode == 404)
            {
                NotFound(resp);
            }
            else if (resp.StatusCode == 401)
            {
                Unauthorized(resp);
            }

            return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };

        }
        [HttpPost("resetpassword/{token}")]
        public async Task<ActionResult<ResponseMsg>> ResetPassword(string token, [FromBody] string password)
        {
            if (password.IsNullOrEmpty())
            {
                return NotFound();  
            }
            var resp = await emailService.ResetPassword(password, token);
            if(resp.StatusCode == 200)
            {
                Ok(resp);
            }else if(resp.StatusCode == 404)
            {
                NotFound(resp);
            }else if(resp.StatusCode == 401)
            {
                Unauthorized(resp);
            }
            
            return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
        }

        [HttpGet("CheckEmail/{Email}")]
        public async Task<ActionResult<ResponseMsg>> CheckEmail(string Email)
        {
            var resp = await emailService.CheckEmail(Email);
            if (resp.StatusCode == 200)
            {
                return Ok(resp);
            }else if(resp.StatusCode == 404)
            {
                return NotFound(resp);
            }
            else
            {
                return new ObjectResult(resp) { StatusCode = (int)StatusCodes.Status500InternalServerError };
            }
        } 
    }
}
