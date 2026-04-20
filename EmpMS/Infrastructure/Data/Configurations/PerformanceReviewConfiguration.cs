using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Configurations
{
    public class PerformanceReviewConfiguration : IEntityTypeConfiguration<PerformanceReview>
    {
        public void Configure(EntityTypeBuilder<PerformanceReview> builder)
        {
            builder.ToTable("PerformanceReviews");

            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Id).ValueGeneratedOnAdd();

            // Unique constraint: one review per employee per period
            builder.HasIndex(pr => new { pr.EmployeeId, pr.ReviewPeriod })
                   .IsUnique()
                   .HasDatabaseName("IX_PerformanceReviews_Employee_Period");

            // Review Period
            builder.Property(pr => pr.ReviewPeriod)
                   .IsRequired()
                   .HasMaxLength(50);

            // Rating (1-5)
            builder.Property(pr => pr.Rating).IsRequired();

            // Text fields
            builder.Property(pr => pr.Strengths).IsRequired().HasMaxLength(2000);
            builder.Property(pr => pr.Weaknesses).IsRequired().HasMaxLength(2000);
            builder.Property(pr => pr.Comments).HasMaxLength(2000);
            builder.Property(pr => pr.Goals).HasMaxLength(2000);

            // Dates
            builder.Property(pr => pr.ReviewDate).IsRequired();
            builder.Property(pr => pr.CreatedAt).IsRequired();

            // Audit strings
            builder.Property(pr => pr.CreatedBy).HasMaxLength(100);
            builder.Property(pr => pr.UpdatedBy).HasMaxLength(100);

            // FK: PerformanceReview → Employee (the employee being reviewed)
            builder.HasOne(pr => pr.Employee)
                   .WithMany(e => e.PerformanceReviews)
                   .HasForeignKey(pr => pr.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // FK: PerformanceReview → Reviewer (the manager/HR who wrote the review)
            builder.HasOne(pr => pr.Reviewer)
                   .WithMany(e => e.ReviewsGiven)
                   .HasForeignKey(pr => pr.ReviewerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
