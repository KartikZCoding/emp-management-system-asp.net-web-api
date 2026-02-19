using EmpMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpMS.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasData(new User
            {
                Id = 1,
                Username = "Admin",
                Email = "Admin123@gmail.com",
                PasswordHash = "$2a$11$3TiXqeZZ1dUHslmgkYVDUusIDqGmV3Yv/E7n2iAhNI46Gvq20aAFy",
                IsActive = true,
                //CreatedAt = DateTime.Now
            });
        }
    }
}
