using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Description).IsRequired(false);

            builder.HasOne(u => u.Subject)
                .WithMany(s => s.Units)
                .HasForeignKey(u => u.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Grade)
                .WithMany(g => g.Units)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Topics)
                .WithOne(t => t.Unit)
                .HasForeignKey(t => t.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Questions)
                .WithOne(q => q.Unit)
                .HasForeignKey(q => q.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
