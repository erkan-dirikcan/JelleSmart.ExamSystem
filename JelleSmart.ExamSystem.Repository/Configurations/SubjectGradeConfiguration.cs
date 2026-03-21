using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class SubjectGradeConfiguration : IEntityTypeConfiguration<SubjectGrade>
    {
        public void Configure(EntityTypeBuilder<SubjectGrade> builder)
        {
            builder.HasKey(sg => new { sg.SubjectId, sg.GradeId });

            builder.HasIndex(sg => new { sg.SubjectId, sg.GradeId })
                .IsUnique();

            builder.HasOne(sg => sg.Subject)
                .WithMany(s => s.SubjectGrades)
                .HasForeignKey(sg => sg.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sg => sg.Grade)
                .WithMany(g => g.SubjectGrades)
                .HasForeignKey(sg => sg.GradeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
