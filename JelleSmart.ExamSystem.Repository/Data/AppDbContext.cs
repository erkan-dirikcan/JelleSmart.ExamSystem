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

        // Phase 1: Profile entities
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Subject - Grade ilişkisi yok (Subject tüm sınıflar için ortak)

            // Subject - Unit (One to Many)
            builder.Entity<Subject>()
                .HasMany(s => s.Units)
                .WithOne(u => u.Subject)
                .HasForeignKey(u => u.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Subject - Question (One to Many)
            builder.Entity<Subject>()
                .HasMany(s => s.Questions)
                .WithOne(q => q.Subject)
                .HasForeignKey(q => q.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Grade - Unit (One to Many)
            builder.Entity<Grade>()
                .HasMany(g => g.Units)
                .WithOne(u => u.Grade)
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unit - Topic (One to Many)
            builder.Entity<Unit>()
                .HasMany(u => u.Topics)
                .WithOne(t => t.Unit)
                .HasForeignKey(t => t.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unit - Question (One to Many)
            builder.Entity<Unit>()
                .HasMany(u => u.Questions)
                .WithOne(q => q.Unit)
                .HasForeignKey(q => q.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            // Topic - Question (One to Many)
            builder.Entity<Topic>()
                .HasMany(t => t.Questions)
                .WithOne(q => q.Topic)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            // Question - Choice (One to Many)
            builder.Entity<Question>()
                .HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exam - ExamQuestion (One to Many)
            builder.Entity<Exam>()
                .HasMany(e => e.ExamQuestions)
                .WithOne(eq => eq.Exam)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamQuestion - Question (Many to One)
            builder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany()
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Exam - StudentExam (One to Many)
            builder.Entity<Exam>()
                .HasMany(e => e.StudentExams)
                .WithOne(se => se.Exam)
                .HasForeignKey(se => se.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentExam - StudentAnswer (One to Many)
            builder.Entity<StudentExam>()
                .HasMany(se => se.StudentAnswers)
                .WithOne(sa => sa.StudentExam)
                .HasForeignKey(sa => sa.StudentExamId)
                .OnDelete(DeleteBehavior.NoAction);

            // Choice - StudentAnswer (One to Many)
            builder.Entity<Choice>()
                .HasMany(c => c.StudentAnswers)
                .WithOne(sa => sa.Choice)
                .HasForeignKey(sa => sa.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentAnswer - Question (Many to One)
            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // AppUser - Question (Teacher creates questions)
            builder.Entity<AppUser>()
                .HasMany(u => u.Questions)
                .WithOne(q => q.CreatedByUser)
                .HasForeignKey(q => q.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser - Exam (Teacher creates exams)
            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedExams)
                .WithOne(e => e.CreatedByUser)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser - StudentSubject (Many to Many - Student)
            builder.Entity<StudentSubject>()
                .HasOne(ss => ss.Student)
                .WithMany(u => u.StudentSubjects)
                .HasForeignKey(ss => ss.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany()
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - StudentExam (Student takes exams)
            builder.Entity<StudentExam>()
                .HasOne(se => se.Student)
                .WithMany(u => u.StudentExams)
                .HasForeignKey(se => se.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - StudentAnswer (Student gives answers)
            builder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Student)
                .WithMany(u => u.StudentAnswers)
                .HasForeignKey(sa => sa.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - Subject (Teacher's branch subject)
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

            // Index configurations
            builder.Entity<Question>()
                .HasIndex(q => new { q.SubjectId, q.UnitId, q.TopicId });

            builder.Entity<Exam>()
                .HasIndex(e => new { e.GradeId, e.SubjectId });

            builder.Entity<StudentExam>()
                .HasIndex(se => new { se.StudentUserId, se.ExamId })
                .IsUnique();

            builder.Entity<StudentAnswer>()
                .HasIndex(sa => new { sa.StudentExamId, sa.QuestionId })
                .IsUnique();

            // ============================================
            // Phase 1: Profile Entity Configurations
            // ============================================

            // AppUser - TeacherProfile (One-to-One)
            builder.Entity<TeacherProfile>()
                .HasOne(tp => tp.User)
                .WithOne(u => u.TeacherProfile)
                .HasForeignKey<TeacherProfile>(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - StudentProfile (One-to-One)
            builder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique indexes for UserId in profiles
            builder.Entity<TeacherProfile>()
                .HasIndex(tp => tp.UserId)
                .IsUnique();

            builder.Entity<StudentProfile>()
                .HasIndex(sp => sp.UserId)
                .IsUnique();

            // StudentProfile - Grade (Many-to-One)
            builder.Entity<StudentProfile>()
                .HasOne(sp => sp.Grade)
                .WithMany()
                .HasForeignKey(sp => sp.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentProfile - StudentParent (One-to-Many)
            builder.Entity<StudentParent>()
                .HasOne(sp => sp.StudentProfile)
                .WithMany(s => s.Parents)
                .HasForeignKey(sp => sp.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // TeacherProfile - TeacherSubject - Subject (Many-to-Many)
            builder.Entity<TeacherSubject>()
                .HasKey(ts => new { ts.TeacherProfileId, ts.SubjectId });

            builder.Entity<TeacherSubject>()
                .HasOne(ts => ts.TeacherProfile)
                .WithMany(tp => tp.Subjects)
                .HasForeignKey(ts => ts.TeacherProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Subject)
                .WithMany()
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentParent unique index (StudentProfile-ParentType combination must be unique)
            builder.Entity<StudentParent>()
                .HasIndex(sp => new { sp.StudentProfileId, sp.ParentType })
                .IsUnique();
        }
    }
}
