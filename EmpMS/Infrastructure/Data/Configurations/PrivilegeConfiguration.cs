using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
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

            builder.HasData(
                new Privilege { Id = 1, PrivilegeName = "Employee.Read", Description = "View employee records" },
                new Privilege { Id = 2, PrivilegeName = "Employee.Create", Description = "Create new employees" },
                new Privilege { Id = 3, PrivilegeName = "Employee.Update", Description = "Update employee details" },
                new Privilege { Id = 4, PrivilegeName = "Employee.Delete", Description = "Delete employees" },
                new Privilege { Id = 5, PrivilegeName = "Department.Read", Description = "View departments" },
                new Privilege { Id = 6, PrivilegeName = "Department.Create", Description = "Create departments" },
                new Privilege { Id = 7, PrivilegeName = "Department.Update", Description = "Update departments" },
                new Privilege { Id = 8, PrivilegeName = "Department.Delete", Description = "Delete departments" },
                new Privilege { Id = 9, PrivilegeName = "Designation.Read", Description = "View designations" },
                new Privilege { Id = 10, PrivilegeName = "Designation.Create", Description = "Create designations" },
                new Privilege { Id = 11, PrivilegeName = "Designation.Update", Description = "Update designations" },
                new Privilege { Id = 12, PrivilegeName = "Designation.Delete", Description = "Delete designations" },
                new Privilege { Id = 13, PrivilegeName = "Role.Manage", Description = "Manage roles" },
                new Privilege { Id = 14, PrivilegeName = "Privilege.Manage", Description = "Manage privileges" },
                new Privilege { Id = 15, PrivilegeName = "User.AssignRole", Description = "Assign roles to users" },
                new Privilege { Id = 16, PrivilegeName = "User.Create", Description = "Create new users" },
                new Privilege { Id = 17, PrivilegeName = "Attendance.Read", Description = "Read attendaces of employees" },
                new Privilege { Id = 18, PrivilegeName = "Attendance.ReadReport", Description = "Read report of attendaces" },
                new Privilege { Id = 19, PrivilegeName = "Attendance.Update", Description = "Update a attendace of employee" },
                new Privilege { Id = 20, PrivilegeName = "Leave.Read", Description = "Leave types getting and reading" },
                new Privilege { Id = 21, PrivilegeName = "Leave.Create", Description = "Creating a leaves" },
                new Privilege { Id = 22, PrivilegeName = "Leave.Update", Description = "Updating a existing leaves" },
                new Privilege { Id = 23, PrivilegeName = "Leave.Delete", Description = "Delete a leaves" },
                new Privilege { Id = 24, PrivilegeName = "LeaveRequest.Update", Description = "Updating a leave for Approve, Reject, Cancel" },
                new Privilege { Id = 25, PrivilegeName = "Salary.Create", Description = "Generate a Employees salaries" },
                new Privilege { Id = 26, PrivilegeName = "Salary.Read", Description = "Reading a employees salaries" },
                new Privilege { Id = 27, PrivilegeName = "Salary.Update", Description = "Updating a employee salary" },
                new Privilege { Id = 28, PrivilegeName = "Review.Create", Description = "Create performance reviews for employees" },
                new Privilege { Id = 29, PrivilegeName = "Review.Read", Description = "View performance reviews and department summaries" },
                new Privilege { Id = 30, PrivilegeName = "Review.Update", Description = "Update existing performance reviews" },
                new Privilege { Id = 31, PrivilegeName = "Review.Delete", Description = "Delete performance reviews" },
                new Privilege { Id = 32, PrivilegeName = "Notification.Broadcast", Description = "Send broadcast notifications to all employees" },
                new Privilege { Id = 33, PrivilegeName = "Dashboard.View", Description = "View the graphical analytics dashboard." },
                new Privilege { Id = 34, PrivilegeName = "Report.Employees", Description = "Download the full CSV directory of employees." },
                new Privilege { Id = 35, PrivilegeName = "Report.Attendance", Description = "Download monthly attendance CSV reports." },
                new Privilege { Id = 36, PrivilegeName = "Report.Salary", Description = "Download monthly salary CSV reports." }
            );
        }
    }
}
