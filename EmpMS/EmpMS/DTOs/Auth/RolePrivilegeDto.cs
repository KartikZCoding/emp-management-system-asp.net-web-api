using System.ComponentModel.DataAnnotations;

namespace EmpMS.DTOs.Auth
{
    public class RolePrivilegeDto
    {
        [Required(ErrorMessage = "please enter a role id!")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "please enter a privilege id!")]
        public int PrivilegeId { get; set; }
    }
}
