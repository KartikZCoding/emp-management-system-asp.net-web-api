using EmpMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpMS.Configurations
{
    public class PrivilegeConfiguration : IEntityTypeConfiguration<Privilege>
    {
        public void Configure(EntityTypeBuilder<Privilege> builder)
        {
            builder.ToTable("Privileges");

            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.PrivilegeName).IsUnique();

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.PrivilegeName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Description).HasMaxLength(200);
        }
    }
}
