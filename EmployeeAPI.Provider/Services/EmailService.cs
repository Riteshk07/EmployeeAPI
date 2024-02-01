using EmployeeAPI.Contract.Dtos.LoginDtos;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace EmployeeAPI.Provider.Services
{
    public class EmailService: IEmailService
    {
        private readonly ILoginService loginService;
        private readonly EmployeeDBContext context;
        private readonly ILogger<EmailService> logger;
        private readonly IConfiguration configuration;

        public EmailService(ILoginService loginService, EmployeeDBContext context, ILogger<EmailService> logger, IConfiguration configuration) {
            this.loginService = loginService;
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
        }

        
        public async Task<ResponseMsg> ForgetPassword(string email)
        {
            ResponseMsg msg = new ResponseMsg();
            try
            {
                var emp = await context.Employees.FirstOrDefaultAsync(e => e.Email == email);
                if(emp!= null)
                {
                    #region Checking User Information
                    if (!emp.IsActive)
                    {
                        msg.Status = "failed";
                        msg.Message = "User Is Inactive or Deleted, Please contact to Administreter";
                        msg.StatusCode = StatusCodes.Status404NotFound;
                        return msg;
                    }
                    #endregion

                    var resp = await this.SendEmail(email,emp);
                    if(resp.StatusCode == 200)
                    {
                        logger.LogInformation("Successfull Checked");
                        return resp;

                    }
                    else
                    {
                        return resp;
                    }
                }
                else
                {
                    msg.Status = "notfoud";
                    msg.StatusCode = 404;
                    msg.Message = "Invalid Email | Email not found";
                    logger.LogWarning("User provided Invalid Email for forget password");
                    return msg;
                }
            }
            catch(Exception ex)
            {
                logger.LogError($"Tracing Error: {ex.StackTrace}\nError Message: {ex.Message}");
                msg.Status = "servererr";
                msg.StatusCode = 500;
                msg.Message = "Error from server side";
                return msg;
            }
        }

        /// <summary>
        /// This method is used for send email to Employee. User can send Email is if user not logged on.
        /// </summary>
        /// <param name="toemail"></param>
        /// <param name="emp"></param>
        /// <returns>ResponseMsg</returns>
        public async Task<ResponseMsg> SendEmail(string toemail, Employee emp)
        {

            ResponseMsg message = new ResponseMsg();
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ritesh.pageup@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toemail));

            email.Subject = "EmployeeAPI| Reset Password";
            
            using var smtp = new SmtpClient();
            try
            {
                string link = this.GenerateLink(emp);
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = $"<h2>Employee API</h2>\r\n    <p>Please click on this button for reset password</p>\r\n    <div style='align-items: center; align-content: center; justify-content: center;'>\r\n        <button style=\"padding: 17px 40px;\r\n        border-radius: 10px;\r\n        border: 0;\r\n        background-color: rgb(255, 56, 86);\r\n        letter-spacing: 1.5px;\r\n        font-size: 15px;\r\n        transition: all 0.3s ease;\r\n        box-shadow: rgb(201, 46, 70) 0px 10px 0px 0px;\r\n        color: white;\r\n        cursor: pointer;\"><a href='{link}'>Reset Password</a></button>\r\n    </div>"
                };

                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("ritesh.pageup@gmail.com", "tzlrreysfduasvkp");
                smtp.Send(email);
                
                message.Status = "success";
                message.Message = "Email sent Successfully";
                message.StatusCode = 200;
                logger.LogInformation($"{message.Message}");
                return message;
            }
            catch (Exception ex)
            {
                logger.LogError($"Tracing Error: {ex.StackTrace}\nError Message: {ex.Message}");
                message.Status = "servererr";
                message.Message = "Error from serever side";
                message.StatusCode = 500;
                return message;
            }finally {
                smtp.Disconnect(true);
            }
        }

        /// <summary>
        /// This method is used for generating a link with token
        /// </summary>
        /// <param name="emp"></param>
        /// <returns>string</returns>
        public string GenerateLink(Employee emp)
        {
            string resp = loginService.GeneratingToken(emp);
            string link = "https://localhost:7219/api/reset/" + resp;
            return link;
        }

        
        public async Task<ResponseMsg> ResetPassword(string password , string token)
        {
            ResponseMsg msg = new ResponseMsg();
            try
            {
                var email = this.ValidateToken(token);
                if (email == null)
                {
                    msg.Status = "failed";
                    msg.Message = "Token not found";
                    msg.StatusCode = 404;
                    logger.LogWarning(msg.Message);
                    return msg;
                }


                var empLog = await context.Logins.FirstOrDefaultAsync(e => e.Email == email);
                if (empLog != null)
                {
                    empLog.Password = password;
                    context.Logins.Update(empLog);
                    await context.SaveChangesAsync();   
                    msg.Status = "success";
                    msg.StatusCode = 200;
                    msg.Message = "Password reset Successfully";
                    return msg;
                }
                else
                {
                    msg.Status = "notfound";
                    msg.StatusCode = 404;
                    msg.Message = "Email not found | Invalid Email";
                    return msg;
                }
            }catch(Exception ex)
            {
                logger.LogError($"Tracing Error: {ex.StackTrace}\nError Message: {ex.Message}");
                msg.Status = "servererr";
                msg.Message = "Error from server side";
                msg.StatusCode = 500;
                return msg;

            }
        }

        public string? ValidateToken(string token)
        {
            if (token == null)
                return null;

            logger.LogInformation("Validating Token");
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "email").Value;

                
                return email;
            }
            catch(Exception ex)
            {
                logger.LogError($"Tracing Error: {ex.StackTrace}\nError Message: {ex.Message}");
                return null;
            }
        }
        public async Task<ResponseMsg> CheckEmail(string Email)
        {
            ResponseMsg msg =  new ResponseMsg();
            try
            {
                var employee = await context.Logins.FirstOrDefaultAsync(l => l.Email == Email);
                if (employee != null)
                {
                    msg.Status = "success";
                    msg.StatusCode = 200;
                    msg.Message = "User is Exist";
                }
                else
                {
                    msg.Status = "failed";
                    msg.StatusCode = 404;
                    msg.Message = "User not found";
                }
                return msg;
            }catch(Exception ex)
            {
                logger.LogError($"Tracing Error: {ex.StackTrace}\nError Message: {ex.Message}");
                msg.Status = "servererr";
                msg.StatusCode = 500;
                msg.Message = "Error from server side";
                return msg;
            }
        }

    }
}
