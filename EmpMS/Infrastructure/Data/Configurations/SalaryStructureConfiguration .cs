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
        }
    }
}
