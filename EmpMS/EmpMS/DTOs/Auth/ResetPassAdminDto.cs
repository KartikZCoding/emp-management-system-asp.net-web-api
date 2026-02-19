using System.ComponentModel.DataAnnotations;

namespace EmpMS.DTOs.Auth
{
    public class ResetPassAdminDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
