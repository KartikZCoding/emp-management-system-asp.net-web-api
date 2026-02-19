using System.ComponentModel.DataAnnotations;

namespace EmpMS.DTOs.Auth
{
    public class ResetPassUserDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
