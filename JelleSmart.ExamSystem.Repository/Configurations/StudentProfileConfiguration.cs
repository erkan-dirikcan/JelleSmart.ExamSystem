using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.GradeId).IsRequired(false);
            builder.Property(e => e.StudentNumber).IsRequired(false);
            builder.Property(e => e.EnrollmentDate).IsRequired();

            builder.HasOne(sp => sp.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sp => sp.Grade)
                .WithMany()
                .HasForeignKey(sp => sp.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(sp => sp.Parents)
                .WithOne(sp => sp.StudentProfile)
                .HasForeignKey(sp => sp.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sp => sp.UserId).IsUnique();
        }
    }
}
