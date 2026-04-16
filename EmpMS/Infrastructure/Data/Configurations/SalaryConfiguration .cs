using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            // Month & Year
            builder.Property(s => s.Month).IsRequired();
            builder.Property(s => s.Year).IsRequired();

            builder.HasIndex(s => new { s.EmployeeId, s.Month, s.Year })
                   .IsUnique()
                   .HasDatabaseName("IX_Salaries_Employee_Month_Year");

            builder.Property(s => s.AnnualCTC).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Basic).HasColumnType("decimal(18,2)");
            builder.Property(s => s.HRA).HasColumnType("decimal(18,2)");
            builder.Property(s => s.DA).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TravelAllowance).HasColumnType("decimal(18,2)");
            builder.Property(s => s.SpecialAllowance).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Bonus).HasColumnType("decimal(18,2)");
            builder.Property(s => s.EmployeePF).HasColumnType("decimal(18,2)");
            builder.Property(s => s.ProfessionalTax).HasColumnType("decimal(18,2)");
            builder.Property(s => s.IncomeTax).HasColumnType("decimal(18,2)");
            builder.Property(s => s.EmployerPF).HasColumnType("decimal(18,2)");
            builder.Property(s => s.Gratuity).HasColumnType("decimal(18,2)");
            builder.Property(s => s.LopDeduction).HasColumnType("decimal(18,2)");
            builder.Property(s => s.GrossEarnings).HasColumnType("decimal(18,2)");
            builder.Property(s => s.TotalDeductions).HasColumnType("decimal(18,2)");
            builder.Property(s => s.NetSalary).HasColumnType("decimal(18,2)");

            // String constraints
            builder.Property(s => s.PayslipStatus).IsRequired().HasMaxLength(20);
            builder.Property(s => s.GeneratedBy).HasMaxLength(100);
            builder.Property(s => s.CreatedBy).HasMaxLength(100);
            builder.Property(s => s.UpdatedBy).HasMaxLength(100);

            builder.Property(s => s.GeneratedDate).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();

            // FK relationship: Salary → Employee
            builder.HasOne(s => s.Employee)
                   .WithMany(e => e.Salaries)
                   .HasForeignKey(s => s.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
