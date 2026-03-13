using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Text).IsRequired();
            builder.Property(e => e.ImageUrl).IsRequired(false);
            builder.Property(e => e.Explanation).IsRequired(false);
            builder.Property(e => e.Difficulty).HasDefaultValue(1);
            builder.Property(e => e.Order).HasDefaultValue(0);

            builder.HasOne(q => q.Subject)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Unit)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.Grade)
                .WithMany()
                .HasForeignKey(q => q.GradeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.CreatedByUser)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.ExamQuestions)
                .WithOne(eq => eq.Question)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(q => new { q.SubjectId, q.UnitId, q.TopicId });
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
