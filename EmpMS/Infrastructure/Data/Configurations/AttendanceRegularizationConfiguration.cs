using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class AttendanceRegularizationConfiguration : IEntityTypeConfiguration<AttendanceRegularization>
    {
        public void Configure(EntityTypeBuilder<AttendanceRegularization> builder)
        {
            builder.ToTable("AttendanceRegularizations");

            builder.HasKey(ar => ar.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(ar => ar.Note).HasMaxLength(500);
            builder.Property(ar => ar.Status).IsRequired().HasMaxLength(20);
            builder.Property(ar => ar.RequestedCheckOut).IsRequired();
            builder.Property(ar => ar.CreatedAt).IsRequired();

            builder.HasOne(ar => ar.Employee)
                .WithMany()
                .HasForeignKey(ar => ar.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ar => ar.Attendance)
                .WithMany()
                .HasForeignKey(ar => ar.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ar => ar.HRorAdmin)
                .WithMany()
                .HasForeignKey(ar => ar.HRorAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
