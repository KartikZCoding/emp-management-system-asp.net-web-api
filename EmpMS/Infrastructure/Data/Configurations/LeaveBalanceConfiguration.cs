using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
    {
        public void Configure(EntityTypeBuilder<LeaveBalance> builder)
        {
            builder.ToTable("LeaveBalances");

            builder.HasKey(lb => lb.Id);
            builder.Property(lb => lb.Id).ValueGeneratedOnAdd();

            builder.Property(lb => lb.Year).IsRequired();
            builder.Property(lb => lb.TotalLeaves).IsRequired();
            builder.Property(lb => lb.UsedLeaves).IsRequired().HasDefaultValue(0);
            builder.Property(lb => lb.RemainingLeaves).IsRequired();

            builder.HasIndex(lb => new { lb.EmployeeId, lb.LeaveTypeId, lb.Year }).IsUnique();

            builder.HasOne(lb => lb.Employee)
                .WithMany()
                .HasForeignKey(lb => lb.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lb => lb.LeaveType)
                .WithMany(lt => lt.leaveBalances)
                .HasForeignKey(lb => lb.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
