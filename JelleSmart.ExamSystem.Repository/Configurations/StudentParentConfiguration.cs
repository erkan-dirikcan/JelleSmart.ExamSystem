using JelleSmart.ExamSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JelleSmart.ExamSystem.Repository.Configurations
{
    public class StudentParentConfiguration : IEntityTypeConfiguration<StudentParent>
    {
        public void Configure(EntityTypeBuilder<StudentParent> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasMaxLength(36).ValueGeneratedOnAdd();

            builder.Property(e => e.ParentType).IsRequired();
            builder.Property(e => e.FirstName).IsRequired();
            builder.Property(e => e.LastName).IsRequired();
            builder.Property(e => e.PhoneNumber).IsRequired(false);
            builder.Property(e => e.Email).IsRequired(false);

            builder.HasOne(sp => sp.StudentProfile)
                .WithMany(s => s.Parents)
                .HasForeignKey(sp => sp.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sp => new { sp.StudentProfileId, sp.ParentType }).IsUnique();
        }
    }
}
