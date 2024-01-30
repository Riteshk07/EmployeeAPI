using EmployeeAPI.Contract.Dtos.EmployeeDtos;
using EmployeeAPI.Contract.Dtos.LoginDtos;
using EmployeeAPI.Contract.Dtos.TodoDtos;
using EmployeeAPI.Contract.Enums;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Contract.Models;
using EmployeeAPI.Contract.ResponseMessage;
using EmployeeAPI.Provider.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Services
{
    public class LoginService: ILoginService
    {
        private readonly EmployeeDBContext context;
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginService> logger;
        private readonly IValidationService validService;
        private readonly IPasswordHash passwordHash;

        #region Constructors
        public LoginService(EmployeeDBContext context, IConfiguration configuration, ILogger<LoginService> logger, IValidationService validService, IPasswordHash passwordHash) {
            this.context = context;
            this.configuration = configuration;
            this.logger = logger;
            this.validService = validService;
            this.passwordHash = passwordHash;
        }
        #endregion 

        #region Generating Token
        public string GeneratingToken(Employee emp)
        {
            #region Defining Token Handler, Key, Credential and Claims
            logger.LogInformation("Generating Token...");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, emp.EmployeeType.ToString()),
                new Claim("Role", emp.EmployeeType.ToString()),
                new Claim("Id", emp.Id.ToString()),
                new Claim("Name", emp.Name),
                new Claim("Email", emp.Email),
                new Claim("DeptId", emp.DepartmentID.ToString())
            };
            #endregion

            #region Writing Token and Send Token
            var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"], 
                    configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(5),
                    signingCredentials: credential
                );

            return tokenHandler.WriteToken(token);
            #endregion
        }
        #endregion

        // Method for User Login | Checking Information from database Generating token  
        #region User Login Method
        public async Task<LoginResponseMessage> UserLogin(LoginDto loginDto)
        {
            try
            {
                #region Checking Employee Information and Send Response
                logger.LogInformation("Checking Information...");

                #region Checking User Information
                var emp = await context.Employees.FirstOrDefaultAsync(e => e.Email == loginDto.Email);
                if(emp != null)
                {
                    if (!emp.IsActive) {
                        LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                        {
                            Status = "failed",
                            Message = "User Is Inactive or Deleted, Please contact to Administreter",
                            StatusCode = StatusCodes.Status404NotFound,
                            Data = null,
                            Token = ""
                        };
                    }
                }
                #endregion

                var LogDetails = await context.Logins.FirstOrDefaultAsync(em => em.Email == loginDto.Email);
                if (LogDetails != null)
                {
                    #region Verifying password
                    if (!passwordHash.Verify(LogDetails.Password, loginDto.Password))
                    {
                        logger.LogWarning($"Incorrect User Password! - Login id: {LogDetails.Id}");
                        LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                        {
                            Status = "failed",
                            Message = "Incorrect User Password!",
                            Data = null,
                            Token = null
                        };
                        return loginMessageDto;
                    }
                    #endregion

                    logger.LogInformation($"Fetching Employee Information");
                    var employee =  await context.Employees.Include(e=>e.Department).FirstOrDefaultAsync(e => e.Id == LogDetails.EmployeeID);
                   
                    if (employee != null)
                    {
                        #region Inserting Employee Information into EmployeeFetchUpdateDto and send Response
                        EmployeeFetchUpdateDto employeeDto = new EmployeeFetchUpdateDto();

                        employeeDto.Id = employee.Id;
                        employeeDto.DepartmentID = employee.Department != null ?employee.Department.Id : 0 ;
                        employeeDto.Name = employee.Name;
                        employeeDto.Email = employee.Email;
                        employeeDto.Phone = employee.Phone;
                        employeeDto.Address = employee.Address;
                        employeeDto.EmployeeType = employee.EmployeeType;
                        employeeDto.City = employee.City;
                        employeeDto.Country = employee.Country;
                        string token = this.GeneratingToken(employee);

                        LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                        {
                            Status = "success",
                            Message = "Login Successfull",
                            StatusCode = 200,
                            Data = employeeDto,
                            Token= token
                        };
                        logger.LogInformation($"{employee.Name} Logged in Successfull with {employee.Id}");
                        return loginMessageDto;
                        #endregion
                    }
                    else
                    {
                        #region Invalid User Response
                        logger.LogWarning("Message: Invalid User Details");
                        LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                        {
                            Status = "notfound",
                            Message = "User Not Found",
                            StatusCode = 404,
                            Data = null,
                            Token = null
                        };
                        return loginMessageDto;
                        #endregion
                    }

                }
                else
                {
                    #region Invalid Email Response
                    logger.LogWarning("Message: Invalid Email or Password");
                    LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                    {
                        Status = "failed",
                        Message = "Invalid Email id",
                        StatusCode = 401,
                        Data = null,
                        Token = null
                    };
                    return loginMessageDto;
                    #endregion

                }
                #endregion
            }
            catch (Exception Ex)
            {
                #region Handling Exception
                logger.LogError($"Error Tracing: \n{Ex.StackTrace}\nError Message: \n{Ex.Message}");
                LoginResponseMessage loginMessageDto = new LoginResponseMessage()
                {
                    Status = "failed",
                    Message = "Error from Server side",
                    StatusCode =500,
                    Data = null,
                    Token = null
                };
                return loginMessageDto;
                #endregion
            }
        }
        #endregion

        // Method for Email changing from Logins Database
        #region Email Changing Method
        public async Task<ResponseWithObjectMessage<Employee>> ChangeEmail(string oldEmail, string email)
        {
            logger.LogInformation("Fetching Information for Email Change");
            
            ResponseWithObjectMessage<Employee> loginMessageDto = new ResponseWithObjectMessage<Employee>();
            try
            {
                #region Checking User Information and Updation
                if (!validService.ValidateEmail(email))
                {
                    loginMessageDto.Data = null;
                    loginMessageDto.Message = "Invalid Email";
                    loginMessageDto.Status = "failed";
                    loginMessageDto.StatusCode = StatusCodes.Status400BadRequest;
                    return loginMessageDto;
                }

                var user = await context.Logins.FirstOrDefaultAsync(e => e.Email == oldEmail);
                
                if (user != null)
                {
                    logger.LogInformation("Email Changing...");
                    user.Email = email;
                    context.Logins.Update(user);
                    await context.SaveChangesAsync();
                    loginMessageDto.Status = "success";
                    loginMessageDto.Message = "Email Change Successfully";
                   
                    return loginMessageDto;
                }
                else
                {
                    loginMessageDto.Status = "failed";
                    loginMessageDto.Message = "Login user not found";
                    return loginMessageDto;
                }
                #endregion
            }
            catch (Exception ex) {
                #region Handling Exception
                logger.LogError($"Error Tracing: \n{ex.StackTrace}\nError Message: \n{ex.Message}");
                loginMessageDto.Status = "servererr";
                return loginMessageDto;
                #endregion
            }
        }

        #endregion

        #region Change Password Method
        public async Task<ResponseMsg> ChangePassword(LoginPasswordDto data, string email)
        {
            ResponseMsg message = new ResponseMsg();
            try
            {
                var logins = await context.Logins.FirstOrDefaultAsync(l => l.Email == email);
                if (logins != null)
                {
                    #region Changing Password
                    logins.Password = passwordHash.HashPassword(data.NewPassword);
                    context.Logins.Update(logins);
                    await context.SaveChangesAsync();
                    message.Status = "success";
                    message.StatusCode = 200;
                    message.Message = "Password Changed Sussessfully!";
                    logger.LogInformation($"{message.Message} By this Email: {email}");
                    #endregion
                    return message;
                }

                else
                {
                    #region Login Data not found Response
                    message.Status = "failed";
                    message.StatusCode = 404;
                    message.Message = "Login Data not found";
                    logger.LogWarning($"{message.Message} By this Email: {email}");
                    #endregion
                    return message;
                }
            }
            catch (Exception ex)
            {
                #region Handle Exception
                message.Status = "servererr";
                message.StatusCode = 501;
                message.Message = "Error from server side";
                logger.LogError($"Get an Error with Details - User Email: {email}\nStacktrace: {ex.StackTrace}, \nError Message: {ex.Message}");
                #endregion
                return message;
            }
        }
        #endregion
    }
}
