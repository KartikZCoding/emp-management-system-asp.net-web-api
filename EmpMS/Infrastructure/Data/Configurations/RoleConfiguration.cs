using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.RoleName).IsUnique();

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Description).HasMaxLength(200);
        }
    }
}
