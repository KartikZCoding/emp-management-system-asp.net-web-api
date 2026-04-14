using System.Net;

namespace Application.Common
{
    // Base response — for endpoints that return no data (create, update, delete, errors)
    public class APIResponse
    {
        public bool Status { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string? Message { get; set; }
        public string? Error { get; set; }
    }

    // Generic response — for endpoints that return typed data
    public class APIResponse<T> : APIResponse
    {
        public T? Data { get; set; }

        public APIResponse() { }

        public APIResponse(T data, string? message = null)
        {
            Data = data;
            Message = message;
        }
    }
}
