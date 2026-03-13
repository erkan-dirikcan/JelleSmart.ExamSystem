using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Level).IsRequired();

            builder.HasMany(g => g.Units)
                .WithOne(u => u.Grade)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Students)
                .WithOne(u => u.Grade)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
