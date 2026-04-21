using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            //required fields with max lengths
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Phone).HasMaxLength(15);
            builder.Property(e => e.Gender).HasMaxLength(10);
            builder.Property(e => e.Address).HasMaxLength(500);
            builder.Property(e => e.PhotoPath).HasMaxLength(300);
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.UpdatedBy).HasMaxLength(100);

            //salary precision
            builder.Property(e => e.AnnualCTC).HasColumnType("decimal(18,2)");

            //default values
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            //foreign key -> Departments
            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); //don't cascade delete employees when department is deleted

            //foreign key -> Designations
            builder.HasOne(e => e.Designation)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict); //don't cascade delete employees when designation is deleted

            //self-referencing foreign key -> Manager (Employee -> Employee)
            builder.HasOne(e => e.Manager)
                .WithMany(m => m.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict) //don't cascade delete subordinates when manager is deleted
                .IsRequired(false); //ManagerId is nullable
        }
    }
}
