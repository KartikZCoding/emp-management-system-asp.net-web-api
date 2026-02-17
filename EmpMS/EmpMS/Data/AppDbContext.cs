using EmpMS.Configurations;
using EmpMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMS.Data
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

        //Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PrivilegeConfiguration());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfiguration());
        }
    }
}
