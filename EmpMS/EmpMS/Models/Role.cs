namespace EmpMS.Models
{
    public class Role
    {
        //table prop
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        //navigation prop
        public ICollection<UserRole> UserRoles { get; set; } //one role assigned to many users
        public ICollection<RolePrivilege> RolePrivileges { get; set; } //one role has many privileges
    }
}
