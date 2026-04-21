using Application.DTOs.Notification;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResultDto> GetMyNotificationsAsync(string email, bool unreadOnly);
        Task MarkAsReadAsync(int notificationId, string email);
        Task<int> MarkAllAsReadAsync(string email);
        Task DeleteNotificationAsync(int notificationId, string email);
        Task<int> BroadcastAsync(BroadcastNotificationDto dto, string createdBy);
    }
}
