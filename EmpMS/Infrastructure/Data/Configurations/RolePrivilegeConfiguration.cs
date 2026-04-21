using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RolePrivilegeConfiguration : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolePrivileges");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            //FK RolePrivilege -> Role (RoleId)
            builder.HasOne(ur => ur.Role)
                .WithMany(u => u.RolePrivileges)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            //Fk RolePrivilege -> Privilege (PrivilegeId)
            builder.HasOne(ur => ur.Privilege)
                .WithMany(u => u.RolePrivileges)
                .HasForeignKey(u => u.PrivilegeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique composite index (RoleId + PrivilegeId)
            builder.HasIndex(ur => new { ur.RoleId, ur.PrivilegeId })
                   .IsUnique();

            builder.HasData(
                // Admin (RoleId 1)
                new RolePrivilege { Id = 11, RoleId = 1, PrivilegeId = 1 },
                new RolePrivilege { Id = 9, RoleId = 1, PrivilegeId = 2 },
                new RolePrivilege { Id = 12, RoleId = 1, PrivilegeId = 3 },
                new RolePrivilege { Id = 10, RoleId = 1, PrivilegeId = 4 },
                new RolePrivilege { Id = 3, RoleId = 1, PrivilegeId = 5 },
                new RolePrivilege { Id = 1, RoleId = 1, PrivilegeId = 6 },
                new RolePrivilege { Id = 4, RoleId = 1, PrivilegeId = 7 },
                new RolePrivilege { Id = 2, RoleId = 1, PrivilegeId = 8 },
                new RolePrivilege { Id = 7, RoleId = 1, PrivilegeId = 9 },
                new RolePrivilege { Id = 5, RoleId = 1, PrivilegeId = 10 },
                new RolePrivilege { Id = 8, RoleId = 1, PrivilegeId = 11 },
                new RolePrivilege { Id = 6, RoleId = 1, PrivilegeId = 12 },
                new RolePrivilege { Id = 14, RoleId = 1, PrivilegeId = 13 },
                new RolePrivilege { Id = 13, RoleId = 1, PrivilegeId = 14 },
                new RolePrivilege { Id = 15, RoleId = 1, PrivilegeId = 15 },
                new RolePrivilege { Id = 31, RoleId = 1, PrivilegeId = 16 },
                new RolePrivilege { Id = 33, RoleId = 1, PrivilegeId = 17 },
                new RolePrivilege { Id = 34, RoleId = 1, PrivilegeId = 18 },
                new RolePrivilege { Id = 35, RoleId = 1, PrivilegeId = 19 },
                new RolePrivilege { Id = 40, RoleId = 1, PrivilegeId = 20 },
                new RolePrivilege { Id = 41, RoleId = 1, PrivilegeId = 21 },
                new RolePrivilege { Id = 42, RoleId = 1, PrivilegeId = 22 },
                new RolePrivilege { Id = 43, RoleId = 1, PrivilegeId = 23 },
                new RolePrivilege { Id = 44, RoleId = 1, PrivilegeId = 24 },
                new RolePrivilege { Id = 51, RoleId = 1, PrivilegeId = 25 },
                new RolePrivilege { Id = 52, RoleId = 1, PrivilegeId = 26 },
                new RolePrivilege { Id = 53, RoleId = 1, PrivilegeId = 27 },
                new RolePrivilege { Id = 56, RoleId = 1, PrivilegeId = 28 },
                new RolePrivilege { Id = 57, RoleId = 1, PrivilegeId = 29 },
                new RolePrivilege { Id = 58, RoleId = 1, PrivilegeId = 30 },
                new RolePrivilege { Id = 59, RoleId = 1, PrivilegeId = 31 },
                new RolePrivilege { Id = 66, RoleId = 1, PrivilegeId = 32 },
                new RolePrivilege { Id = 68, RoleId = 1, PrivilegeId = 33 },
                new RolePrivilege { Id = 69, RoleId = 1, PrivilegeId = 34 },
                new RolePrivilege { Id = 70, RoleId = 1, PrivilegeId = 35 },
                new RolePrivilege { Id = 71, RoleId = 1, PrivilegeId = 36 },

                // HR (RoleId 2)
                new RolePrivilege { Id = 26, RoleId = 2, PrivilegeId = 1 },
                new RolePrivilege { Id = 25, RoleId = 2, PrivilegeId = 2 },
                new RolePrivilege { Id = 27, RoleId = 2, PrivilegeId = 3 },
                new RolePrivilege { Id = 20, RoleId = 2, PrivilegeId = 5 },
                new RolePrivilege { Id = 19, RoleId = 2, PrivilegeId = 6 },
                new RolePrivilege { Id = 21, RoleId = 2, PrivilegeId = 7 },
                new RolePrivilege { Id = 23, RoleId = 2, PrivilegeId = 9 },
                new RolePrivilege { Id = 22, RoleId = 2, PrivilegeId = 10 },
                new RolePrivilege { Id = 24, RoleId = 2, PrivilegeId = 11 },
                new RolePrivilege { Id = 32, RoleId = 2, PrivilegeId = 16 },
                new RolePrivilege { Id = 38, RoleId = 2, PrivilegeId = 17 },
                new RolePrivilege { Id = 37, RoleId = 2, PrivilegeId = 18 },
                new RolePrivilege { Id = 36, RoleId = 2, PrivilegeId = 19 },
                new RolePrivilege { Id = 46, RoleId = 2, PrivilegeId = 20 },
                new RolePrivilege { Id = 47, RoleId = 2, PrivilegeId = 21 },
                new RolePrivilege { Id = 48, RoleId = 2, PrivilegeId = 22 },
                new RolePrivilege { Id = 49, RoleId = 2, PrivilegeId = 24 },
                new RolePrivilege { Id = 54, RoleId = 2, PrivilegeId = 25 },
                new RolePrivilege { Id = 55, RoleId = 2, PrivilegeId = 26 },
                new RolePrivilege { Id = 60, RoleId = 2, PrivilegeId = 28 },
                new RolePrivilege { Id = 61, RoleId = 2, PrivilegeId = 29 },
                new RolePrivilege { Id = 62, RoleId = 2, PrivilegeId = 30 },
                new RolePrivilege { Id = 67, RoleId = 2, PrivilegeId = 32 },
                new RolePrivilege { Id = 72, RoleId = 2, PrivilegeId = 33 },
                new RolePrivilege { Id = 73, RoleId = 2, PrivilegeId = 34 },
                new RolePrivilege { Id = 74, RoleId = 2, PrivilegeId = 35 },
                new RolePrivilege { Id = 75, RoleId = 2, PrivilegeId = 36 },

                // Manager (RoleId 3)
                new RolePrivilege { Id = 30, RoleId = 3, PrivilegeId = 1 },
                new RolePrivilege { Id = 28, RoleId = 3, PrivilegeId = 5 },
                new RolePrivilege { Id = 29, RoleId = 3, PrivilegeId = 9 },
                new RolePrivilege { Id = 39, RoleId = 3, PrivilegeId = 17 },
                new RolePrivilege { Id = 50, RoleId = 3, PrivilegeId = 24 },
                new RolePrivilege { Id = 63, RoleId = 3, PrivilegeId = 28 },
                new RolePrivilege { Id = 64, RoleId = 3, PrivilegeId = 29 },
                new RolePrivilege { Id = 65, RoleId = 3, PrivilegeId = 30 }
            );
        }   
    }
}
