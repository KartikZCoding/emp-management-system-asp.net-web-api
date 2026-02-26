namespace Domain.Entities
{
    public class Privilege
    {
        //table prop
        public int Id { get; set; }
        public string PrivilegeName { get; set; }
        public string Description { get; set; }
   
        //navigation prop
        public ICollection<RolePrivilege> RolePrivileges { get; set; } 
    }
}
