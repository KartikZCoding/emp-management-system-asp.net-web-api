using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);
            builder.HasIndex(d => d.DepartmentName).IsUnique();

            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.DepartmentName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Description).HasMaxLength(300);
            builder.Property(d => d.IsActive).HasDefaultValue(true);
            builder.Property(d => d.CreatedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
