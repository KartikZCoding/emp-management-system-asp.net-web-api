using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RolePrivilegeConfiguration : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolePrivileges");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            //FK RolePrivilege -> Role (RoleId)
            builder.HasOne(ur => ur.Role)
                .WithMany(u => u.RolePrivileges)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            //Fk RolePrivilege -> Privilege (PrivilegeId)
            builder.HasOne(ur => ur.Privilege)
                .WithMany(u => u.RolePrivileges)
                .HasForeignKey(u => u.PrivilegeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique composite index (RoleId + PrivilegeId)
            builder.HasIndex(ur => new { ur.RoleId, ur.PrivilegeId })
                   .IsUnique();
        }   
    }
}
