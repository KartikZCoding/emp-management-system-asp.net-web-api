using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class PrivilegeDto
    {
        [Required(ErrorMessage = "please enter a privilege name!")]
        public string PrivilegeName { get; set; }
        public string Description { get; set; }
    }
}
