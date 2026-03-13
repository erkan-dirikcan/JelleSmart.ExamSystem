using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Label).IsRequired().HasMaxLength(1);
            builder.Property(e => e.Text).IsRequired();
            builder.Property(e => e.ImageUrl).IsRequired(false);
            builder.Property(e => e.IsCorrect).HasDefaultValue(false);

            builder.HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.StudentAnswers)
                .WithOne(sa => sa.Choice)
                .HasForeignKey(sa => sa.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
