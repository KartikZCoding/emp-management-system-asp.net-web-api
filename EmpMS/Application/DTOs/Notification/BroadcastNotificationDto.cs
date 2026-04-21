using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notification
{
    public class BroadcastNotificationDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000)]
        public string Message { get; set; }
    }
}
