using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.IsCorrect).HasDefaultValue(false);
            builder.Property(e => e.Points).HasDefaultValue(0.0);

            builder.HasOne(sa => sa.StudentExam)
                .WithMany(se => se.StudentAnswers)
                .HasForeignKey(sa => sa.StudentExamId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sa => sa.Choice)
                .WithMany(c => c.StudentAnswers)
                .HasForeignKey(sa => sa.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sa => sa.Student)
                .WithMany(u => u.StudentAnswers)
                .HasForeignKey(sa => sa.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sa => new { sa.StudentExamId, sa.QuestionId }).IsUnique();
        }
    }
}
