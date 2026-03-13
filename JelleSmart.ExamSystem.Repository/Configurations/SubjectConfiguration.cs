using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Description).IsRequired(false);
            builder.Property(e => e.IconClass).IsRequired(false);

            builder.HasMany(s => s.Units)
                .WithOne(u => u.Subject)
                .HasForeignKey(u => u.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Questions)
                .WithOne(q => q.Subject)
                .HasForeignKey(q => q.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
