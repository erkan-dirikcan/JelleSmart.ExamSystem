using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class StudentExamConfiguration : IEntityTypeConfiguration<StudentExam>
    {
        public void Configure(EntityTypeBuilder<StudentExam> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.StartedAt).IsRequired(false);
            builder.Property(e => e.CompletedAt).IsRequired(false);
            builder.Property(e => e.Status).HasDefaultValue(Core.Enums.ExamStatus.NotStarted);
            builder.Property(e => e.RemainingTime).IsRequired(false);
            builder.Property(e => e.Score).HasDefaultValue(0.0);
            builder.Property(e => e.CorrectCount).HasDefaultValue(0);
            builder.Property(e => e.WrongCount).HasDefaultValue(0);
            builder.Property(e => e.EmptyCount).HasDefaultValue(0);

            builder.HasOne(se => se.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(se => se.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(se => se.Student)
                .WithMany(u => u.StudentExams)
                .HasForeignKey(se => se.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(se => se.StudentAnswers)
                .WithOne(sa => sa.StudentExam)
                .HasForeignKey(sa => sa.StudentExamId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(se => new { se.StudentUserId, se.ExamId }).IsUnique();
        }
    }
}
