using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class SalaryStructureConfiguration : IEntityTypeConfiguration<SalaryStructure>
    {
        public void Configure(EntityTypeBuilder<SalaryStructure> builder)
        {
            builder.ToTable("SalaryStructures");
            builder.HasKey(ss => ss.Id);
            builder.Property(ss => ss.Id).ValueGeneratedOnAdd();
            builder.Property(ss => ss.ComponentName).IsRequired().HasMaxLength(100);
            builder.Property(ss => ss.ComponentType).IsRequired().HasMaxLength(50);
            builder.Property(ss => ss.CalculationType).IsRequired().HasMaxLength(50);
            builder.Property(ss => ss.Value).HasColumnType("decimal(18,2)");
            builder.Property(ss => ss.MaxLimit).HasColumnType("decimal(18,2)");
            builder.Property(ss => ss.CreatedAt).IsRequired();
            builder.Property(ss => ss.CreatedBy).HasMaxLength(100);
            builder.Property(ss => ss.UpdatedBy).HasMaxLength(100);

            builder.HasIndex(ss => ss.ComponentName).IsUnique();

            builder.HasData(
                // EARNINGS
                new SalaryStructure
                {
                    Id = 1,
                    ComponentName = "Basic Salary",
                    ComponentType = "Earning",
                    CalculationType = "PercentageOfCTC",
                    Value = 40,                          // 40% of Annual CTC
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 2,
                    ComponentName = "HRA",
                    ComponentType = "Earning",
                    CalculationType = "PercentageOfBasic",
                    Value = 50,                          // 50% of Basic (metro city rate)
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 3,
                    ComponentName = "Dearness Allowance",
                    ComponentType = "Earning",
                    CalculationType = "PercentageOfBasic",
                    Value = 10,                          // 10% of Basic
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 3,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 4,
                    ComponentName = "Travel Allowance",
                    ComponentType = "Earning",
                    CalculationType = "Fixed",
                    Value = 1600,                        // Flat ₹1,600/month
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 4,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 5,
                    ComponentName = "Special Allowance",
                    ComponentType = "Earning",
                    CalculationType = "Remaining",       // Balancing figure
                    Value = 0,                           // Not used — calculated as remainder
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 5,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                // EMPLOYEE DEDUCTIONS
                new SalaryStructure
                {
                    Id = 6,
                    ComponentName = "Employee PF",
                    ComponentType = "Deduction",
                    CalculationType = "PercentageOfBasic",
                    Value = 12,                          // 12% of Basic
                    MaxLimit = 1800,                     // Capped at ₹1,800 (12% of ₹15,000 ceiling)
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 7,
                    ComponentName = "Professional Tax",
                    ComponentType = "Deduction",
                    CalculationType = "Fixed",
                    Value = 200,                         // ₹200/month (most Indian states)
                    MaxLimit = 2500,                     // Annual cap of ₹2,500
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 8,
                    ComponentName = "Income Tax",
                    ComponentType = "Deduction",
                    CalculationType = "TaxSlab",         // Uses New Tax Regime slabs
                    Value = 0,                           // Not used — calculated from slabs
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 3,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                // EMPLOYER CONTRIBUTIONS
                new SalaryStructure
                {
                    Id = 9,
                    ComponentName = "Employer PF",
                    ComponentType = "EmployerContribution",
                    CalculationType = "PercentageOfBasic",
                    Value = 12,                          // 12% of Basic
                    MaxLimit = 1800,                     // Capped at ₹1,800
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                },
                new SalaryStructure
                {
                    Id = 10,
                    ComponentName = "Gratuity",
                    ComponentType = "EmployerContribution",
                    CalculationType = "PercentageOfBasic",
                    Value = 4.81m,                       // 4.81% of Basic
                    MaxLimit = null,
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = new DateTime(2026, 1, 1),
                    CreatedBy = "System"
                }
            );
        }
    }
}
