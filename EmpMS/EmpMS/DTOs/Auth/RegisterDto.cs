using System.ComponentModel.DataAnnotations;

namespace EmpMS.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "please enter a username!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "please enter a password!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "please enter a email!")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
