using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all configurations from the Configurations folder
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Additional AppUser configurations (not covered by Identity)
            builder.Entity<AppUser>()
                .HasOne(u => u.Subject)
                .WithMany()
                .HasForeignKey(u => u.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // AppUser - Grade (Student's grade)
            builder.Entity<AppUser>()
                .HasOne(u => u.Grade)
                .WithMany(g => g.Students)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
