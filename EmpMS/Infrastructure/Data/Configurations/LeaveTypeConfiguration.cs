using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
    {
        public void Configure(EntityTypeBuilder<LeaveType> builder)
        {
            builder.ToTable("LeaveTypes");

            builder.HasKey(lt => lt.Id);
            builder.Property(lt => lt.Id).ValueGeneratedOnAdd();

            builder.Property(lt => lt.Name).IsRequired().HasMaxLength(100);
            builder.Property(lt => lt.Description).IsRequired(false).HasMaxLength(500);
            builder.Property(lt => lt.DefaultDays).IsRequired();
            builder.Property(lt => lt.IsPaid).IsRequired();
            builder.Property(lt => lt.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(lt => lt.CreatedAt).IsRequired();
            builder.Property(lt => lt.CreatedBy).IsRequired(false).HasMaxLength(100);
            builder.Property(lt => lt.UpdatedAt).IsRequired(false);
            builder.Property(lt => lt.UpdatedBy).IsRequired(false).HasMaxLength(100);
        }
    }
}
