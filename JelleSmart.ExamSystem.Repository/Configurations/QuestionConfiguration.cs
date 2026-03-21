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

            builder.HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.CreatedByUser)
                .WithMany()
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

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
