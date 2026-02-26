using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
    {
        public void Configure(EntityTypeBuilder<Designation> builder)
        {
            builder.ToTable("Designations");

            builder.HasKey(d => d.Id);
            builder.HasIndex(d => d.DesignationName).IsUnique();

            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.DesignationName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Description).HasMaxLength(300);
            builder.Property(d => d.IsActive).HasDefaultValue(true);
            builder.Property(d => d.CreatedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
