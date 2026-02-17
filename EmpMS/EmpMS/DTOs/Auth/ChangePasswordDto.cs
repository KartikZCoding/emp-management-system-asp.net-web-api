using System.ComponentModel.DataAnnotations;

namespace EmpMS.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "please enter a old password!")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "please enter a new password!")]
        public string NewPassword { get; set; }
    }
}
