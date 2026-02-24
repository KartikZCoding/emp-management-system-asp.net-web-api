using EmpMS.Exceptionss;
using EmpMS.Helpers;
using System.Net;
using System.Text.Json;

namespace EmpMS.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private RequestDelegate _next;
        ILogger<ExceptionMiddleware> _logger;
        private APIResponse _apiResponse;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _apiResponse = new();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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

                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = Convert.ToInt32(statusCode);

                _apiResponse.Status = false;
                _apiResponse.StatusCode = statusCode;
                _apiResponse.Errors.Add(ex.Message);

                var jsonResponse = JsonSerializer.Serialize(_apiResponse);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
