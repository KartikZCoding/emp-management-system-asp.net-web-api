using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class RoleDto
    {
        [Required(ErrorMessage = "please enter a role name!")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
