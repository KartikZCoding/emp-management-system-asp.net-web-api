using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.EmployeeId).IsRequired();
            builder.Property(a => a.Date).IsRequired();
            builder.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

            builder.Property(a => a.Status).IsRequired().HasMaxLength(20);
            builder.Property(a => a.TotalHours).HasPrecision(5, 2);
            builder.Property(a => a.IsCheckedIn).HasDefaultValue(false);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.UpdatedBy).HasMaxLength(100);

            builder.HasOne(a => a.Employee)
                .WithMany(a => a.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.AttendanceLogs);
        }
    }
}
