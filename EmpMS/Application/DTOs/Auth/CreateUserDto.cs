using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Please enter a username!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter an email!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please select at least one role!")]
        public List<int> RoleIds { get; set; }
        public int? EmployeeId { get; set; }
    }
}
