using EmpMS.Exceptions;
using EmpMS.Helpers;
using System.Net;
using System.Text.Json;

namespace EmpMS.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode;

                switch (ex)
                {
                    case NotFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        break;

                    case BadRequestException:
                        statusCode = HttpStatusCode.BadRequest;
                        break;

                    case UnauthorizedException:
                        statusCode = HttpStatusCode.Unauthorized;
                        break;

                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        break;
                }

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    _logger.LogError(ex, ex.Message);
                }
                else
                {
                    _logger.LogWarning(ex.Message);
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = Convert.ToInt32(statusCode);

                var apiResponse = new APIResponse();
                apiResponse.Status = false;
                apiResponse.StatusCode = statusCode;
                apiResponse.Errors.Add(ex.Message);

                var jsonResponse = JsonSerializer.Serialize(apiResponse);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
