using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
    {
        public void Configure(EntityTypeBuilder<TeacherSubject> builder)
        {
            builder.HasKey(ts => new { ts.TeacherProfileId, ts.SubjectId });

            builder.HasOne(ts => ts.TeacherProfile)
                .WithMany(tp => tp.Subjects)
                .HasForeignKey(ts => ts.TeacherProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ts => ts.Subject)
                .WithMany()
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
