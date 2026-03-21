using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Code).IsRequired(false);
            builder.Property(e => e.Description).IsRequired(false);

            builder.HasIndex(t => new { t.UnitId, t.Order })
                .IsUnique();

            builder.HasOne(t => t.Unit)
                .WithMany(u => u.Topics)
                .HasForeignKey(t => t.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Grade)
                .WithMany()
                .HasForeignKey(t => t.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Questions)
                .WithOne(q => q.Topic)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(t => t.Order)
                .HasDefaultValue(1);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
