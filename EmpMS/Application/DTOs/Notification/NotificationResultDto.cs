using System.Collections.Generic;

namespace Application.DTOs.Notification
{
    public class NotificationResultDto
    {
        public List<NotificationResponseDto> Notifications { get; set; } = new List<NotificationResponseDto>();
        public int UnreadCount { get; set; }
    }
}
