using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class AttendanceLogConfiguration : IEntityTypeConfiguration<AttendanceLog>
    {
        public void Configure(EntityTypeBuilder<AttendanceLog> builder)
        {
            builder.ToTable("AttendanceLogs");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.CheckIn).IsRequired();
            builder.Property(a => a.SessionHours).HasPrecision(5, 2);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
            
            builder.HasOne(a => a.Attendance)
                .WithMany(a => a.AttendanceLogs)
                .HasForeignKey(a => a.AttendanceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
