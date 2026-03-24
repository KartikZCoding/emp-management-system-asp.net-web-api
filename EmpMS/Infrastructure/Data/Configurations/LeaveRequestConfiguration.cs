using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");

            builder.HasKey(lr => lr.Id);
            builder.Property(lr => lr.Id).ValueGeneratedOnAdd();

            builder.Property(lr => lr.StartDate).IsRequired();
            builder.Property(lr => lr.EndDate).IsRequired();
            builder.Property(lr => lr.TotalDays).IsRequired();
            builder.Property(lr => lr.Reason).IsRequired().HasMaxLength(500);
            builder.Property(lr => lr.Status).IsRequired().HasMaxLength(20);
            builder.Property(lr => lr.CreatedAt).IsRequired();

            builder.Property(lr => lr.ApprovedById).IsRequired(false);
            builder.Property(lr => lr.DecisionDate).IsRequired(false);
            builder.Property(lr => lr.DecisionNote).IsRequired(false).HasMaxLength(500);
            builder.Property(lr => lr.UpdatedAt).IsRequired(false);

            builder.HasOne(lr => lr.Employee)
                .WithMany()
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.LeaveType)
                .WithMany(lt => lt.leaveRequests)
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.ApprovedBy)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
