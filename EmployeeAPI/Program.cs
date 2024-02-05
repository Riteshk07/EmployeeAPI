using EmployeeAPI;
using EmployeeAPI.Contract.Interfaces;
using EmployeeAPI.Provider.Context;
using EmployeeAPI.Provider.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// For clearing Logging prpoviders
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

/*
builder.Services.AddLogging(logBuilder =>
{
    logBuilder.AddSimpleConsole(option =>
    {
        option.SingleLine = true;
        option.TimestampFormat = "[HH:MM:ss] ";
    });
});
*/

/*
builder.Services.AddLogging(logBuilder =>
{
    logBuilder.AddJsonConsole(option =>
    {
        option.TimestampFormat = "[HH:MM:ss] ";
        option.JsonWriterOptions = new System.Text.Json.JsonWriterOptions() { 
            Indented = true
        };    
    });
});
*/

/*
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .WriteTo.File(
     path: "D:\\DOt_NET Core\\EmployeeAPI\\EmployeeLog.txt"
    )
    .CreateLogger();
builder.Logging.Services.AddSerilog();
*/
/*
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.Services.AddSerilog();
*/

// Validating Jwt Token for  Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            
        };
    });


// Add services to the container.

builder.Services.AddControllers();

// Database Connectivity
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<EmployeeDBContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IPasswordHash, PasswordHash>();
builder.Services.AddSingleton<IEncryptMessage, EncryptMessage>();
builder.Services.AddScoped<ICommunityMessageService, CommunityMessageService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    opt.OperationFilter<CustomHeaderSwaggerAttribute>();
});

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());


app.Logger.LogInformation("Calling From Program.cs Class");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
