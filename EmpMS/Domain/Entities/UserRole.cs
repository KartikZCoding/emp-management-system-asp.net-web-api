namespace Domain.Entities
{
    public class UserRole
    {
        //table prop
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        //navigation prop
        public User User { get; set; } //reference to the User
        public Role Role { get; set; } //reference to the Role
    }
}
