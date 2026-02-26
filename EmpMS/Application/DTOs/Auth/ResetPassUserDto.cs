using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class ResetPassUserDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
