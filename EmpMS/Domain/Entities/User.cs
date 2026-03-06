namespace Domain.Entities
{
    public class User
    {
        //table prop
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        //navigation prop
        public ICollection<UserRole> UserRoles { get; set; } //one user can have many roles
    }
}
