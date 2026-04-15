using Application;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Interfaces;
using EmpMS.Authorization;
using EmpMS.Middleware;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*cache memory for otp*/
builder.Services.AddMemoryCache();

/*serilog configuration*/
/*-------------------------------------------------------------------------------------------------------------------*/
var enableLogs = builder.Configuration.GetValue<bool>("Logging:EnableFileLogging");
var logFolder = builder.Configuration.GetValue<string>("Logging:LogFolder");
var logFile = builder.Configuration.GetValue<string>("Logging:LogFile");
var logPath = Path.Combine(logFolder, logFile);


var logConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console();

if (Directory.Exists(logFolder) && File.Exists(logPath) && enableLogs)
{
    logConfig = logConfig.WriteTo.File(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");
}

Log.Logger = logConfig.CreateLogger();

builder.Host.UseSerilog();
/*-------------------------------------------------------------------------------------------------------------------*/

/*Add services to the container.*/
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

/*Adding a authorization button in swagger UI*/
/*-------------------------------------------------------------------------------------------------------------------*/
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token only. Example: eyJhbGciOiJIUzI1NiIs...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
/*-------------------------------------------------------------------------------------------------------------------*/


/*register ALL infrastructure services*/
builder.Services.AddInfrastructure(builder.Configuration);

/*register ALL Application services*/
builder.Services.AddApplication(builder.Configuration);


/*Configure JWT Authentication*/
/*-------------------------------------------------------------------------------------------------------------------*/
var publicKeyText = File.ReadAllText(builder.Configuration["Jwt:PublicKeyPath"]);
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Tell ASP.NET to read token from cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new RsaSecurityKey(rsa),
        ClockSkew = TimeSpan.Zero
    };
});
/*-------------------------------------------------------------------------------------------------------------------*/

/*automapper registered AutoMapper 13+ syntax*/
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile));

/*Register the permission handler*/
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

/*Register the dynamic policy provider (replaces the default one)*/
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();


var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmpMS v1");
});

app.UseHttpsRedirection();

app.UseStaticFiles(); // serve photos from wwwroot

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
