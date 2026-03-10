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
        public int? EmployeeId { get; set; }
        public bool MustChangePassword { get; set; }
        public string? CreatedBy { get; set; }

        //navigation prop
        public ICollection<UserRole> UserRoles { get; set; } //one user can have many roles
        public Employee? Employee { get; set; }
    }
}
