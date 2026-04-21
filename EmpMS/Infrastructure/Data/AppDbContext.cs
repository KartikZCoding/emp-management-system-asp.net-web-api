using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        //constructor for DB Context
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //Dbsets (Tables)
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<AttendanceRegularization> AttendanceRegularizations { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SalaryStructure> SalaryStructures { get; set; }

        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        //Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PrivilegeConfiguration());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new DesignationConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceLogConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceRegularizationConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveBalanceConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveRequestConfiguration());

            modelBuilder.ApplyConfiguration(new SalaryConfiguration());
            modelBuilder.ApplyConfiguration(new SalaryStructureConfiguration());

            modelBuilder.ApplyConfiguration(new PerformanceReviewConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }
}
