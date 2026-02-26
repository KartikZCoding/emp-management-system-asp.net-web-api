namespace Domain.Entities
{
    public class RolePrivilege
    {
        //table prop
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PrivilegeId { get; set; }

        //navigation prop
        public Role Role { get; set; }
        public Privilege Privilege { get; set; }
    }
}
