using EmpMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpMS.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            //FK UserRole -> User (UserId)
            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u =>u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Fk UserRole -> Role (RoleId)
            builder.HasOne(ur => ur.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u =>u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique composite index (UserId + RoleId)
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                   .IsUnique();
        }
    }
}
