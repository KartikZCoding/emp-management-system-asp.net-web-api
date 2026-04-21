using Application.Common;
using Application.DTOs.Notification;
using Application.Interfaces;
using EmpMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmpMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<APIResponse<NotificationResultDto>>> GetMyNotifications([FromQuery] bool unreadOnly = false)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var response = await _notificationService.GetMyNotificationsAsync(email, unreadOnly);
            return Ok(new APIResponse<NotificationResultDto>(response));
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> MarkAsRead(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            await _notificationService.MarkAsReadAsync(id, email);
            
            return Ok(new APIResponse { Message = "Notification marked as read" });
        }

        [HttpPut("read-all")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> MarkAllAsRead()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var count = await _notificationService.MarkAllAsReadAsync(email);
            
            return Ok(new APIResponse { Message = $"{count} notifications marked as read" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<APIResponse>> DeleteNotification(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            await _notificationService.DeleteNotificationAsync(id, email);
            
            return Ok(new APIResponse { Message = "Notification deleted successfully" });
        }

        [HttpPost("broadcast")]
        [HasPermission("Notification.Broadcast")]
        public async Task<ActionResult<APIResponse>> BroadcastNotification(BroadcastNotificationDto dto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            
            var count = await _notificationService.BroadcastAsync(dto, username);

            return StatusCode(StatusCodes.Status201Created, new APIResponse
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = $"Broadcast sent successfully to {count} employees"
            });
        }
    }
}
