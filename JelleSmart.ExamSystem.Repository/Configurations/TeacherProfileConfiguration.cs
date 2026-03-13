using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class TeacherProfileConfiguration : IEntityTypeConfiguration<TeacherProfile>
    {
        public void Configure(EntityTypeBuilder<TeacherProfile> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.Title).IsRequired(false);
            builder.Property(e => e.Department).IsRequired(false);
            builder.Property(e => e.HireDate).IsRequired(false);

            builder.HasOne(tp => tp.User)
                .WithOne(u => u.TeacherProfile)
                .HasForeignKey<TeacherProfile>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tp => tp.Subjects)
                .WithOne(ts => ts.TeacherProfile)
                .HasForeignKey(ts => ts.TeacherProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(tp => tp.UserId).IsUnique();
        }
    }
}
