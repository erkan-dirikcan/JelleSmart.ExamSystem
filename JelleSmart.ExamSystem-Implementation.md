# JelleSmart.ExamSystem — Implementasyon Rehberi

> .NET Core 10 MVC · Repository Design Pattern · ASP.NET Identity · Claude Code ile Geliştirme

---

## 1. Çözüm Yapısı (Solution Structure)

```
JelleSmart.ExamSystem.sln
│
├── JelleSmart.ExamSystem.Core           (Class Library)
├── JelleSmart.ExamSystem.Repository     (Class Library)
├── JelleSmart.ExamSystem.Service        (Class Library)
└── JelleSmart.ExamSystem.WebUI         (ASP.NET Core 10 MVC)
```

---

## 2. Katman Sorumlulukları

| Katman | Sorumluluk |
|--------|-----------|
| **Core** | Entity sınıfları, Interface'ler (IRepository, IService), DTO'lar, Enum'lar |
| **Repository** | DbContext (EF Core), Repository implementasyonları, Migration'lar |
| **Service** | Business logic, Servis implementasyonları |
| **WebUI** | Controller'lar, View'lar, ViewModels, Middleware, wwwroot |

---

## 3. Proje Oluşturma Komutları

```bash
# Solution oluştur
dotnet new sln -n JelleSmart.ExamSystem

# Projeler
dotnet new classlib -n JelleSmart.ExamSystem.Core       -f net10.0
dotnet new classlib -n JelleSmart.ExamSystem.Repository -f net10.0
dotnet new classlib -n JelleSmart.ExamSystem.Service    -f net10.0
dotnet new mvc     -n JelleSmart.ExamSystem.WebUI       -f net10.0

# Solution'a ekle
dotnet sln add JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj
dotnet sln add JelleSmart.ExamSystem.Repository/JelleSmart.ExamSystem.Repository.csproj
dotnet sln add JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj
dotnet sln add JelleSmart.ExamSystem.WebUI/JelleSmart.ExamSystem.WebUI.csproj

# Referanslar
dotnet add JelleSmart.ExamSystem.Repository/JelleSmart.ExamSystem.Repository.csproj reference JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj
dotnet add JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj reference JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj
dotnet add JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj reference JelleSmart.ExamSystem.Repository/JelleSmart.ExamSystem.Repository.csproj
dotnet add JelleSmart.ExamSystem.WebUI/JelleSmart.ExamSystem.WebUI.csproj reference JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj
dotnet add JelleSmart.ExamSystem.WebUI/JelleSmart.ExamSystem.WebUI.csproj reference JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj
```

### NuGet Paketleri

**Repository:**
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

**WebUI:**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.UI
dotnet add package SixLabors.ImageSharp   # Görsel işleme (isteğe bağlı)
```

---

## 4. Core Katmanı

### 4.1 Klasör Yapısı

```
Core/
├── Entities/
│   ├── Identity/
│   │   ├── AppUser.cs
│   │   └── AppRole.cs
│   ├── Subject.cs           (Ders)
│   ├── Grade.cs             (Sınıf)
│   ├── Unit.cs              (Ünite)
│   ├── Topic.cs             (Konu)
│   ├── Question.cs          (Soru)
│   ├── Choice.cs            (Şık)
│   ├── Exam.cs              (Test/Sınav)
│   ├── ExamQuestion.cs      (Test-Soru ilişkisi)
│   ├── StudentExam.cs       (Öğrenci-Test atama)
│   ├── StudentAnswer.cs     (Öğrenci cevapları)
│   └── StudentSubject.cs    (Öğrenci-Ders atama)
├── Interfaces/
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── ISubjectRepository.cs
│   │   ├── IGradeRepository.cs
│   │   ├── IUnitRepository.cs
│   │   ├── ITopicRepository.cs
│   │   ├── IQuestionRepository.cs
│   │   ├── IExamRepository.cs
│   │   └── IStudentExamRepository.cs
│   └── Services/
│       ├── ISubjectService.cs
│       ├── IGradeService.cs
│       ├── IUnitService.cs
│       ├── ITopicService.cs
│       ├── IQuestionService.cs
│       ├── IExamService.cs
│       ├── IStudentExamService.cs
│       └── IReportService.cs
├── DTOs/
│   ├── ExamResultDto.cs
│   ├── StudentReportDto.cs
│   └── TeacherReportDto.cs
└── Enums/
    ├── UserRole.cs
    └── ExamStatus.cs
```

### 4.2 Entity Sınıfları

#### BaseEntity.cs
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
```

#### Identity/AppUser.cs
```csharp
using Microsoft.AspNetCore.Identity;

namespace JelleSmart.ExamSystem.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Öğretmen için branş dersi
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Navigation
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
    }
}
```

#### Identity/AppRole.cs
```csharp
using Microsoft.AspNetCore.Identity;

namespace JelleSmart.ExamSystem.Core.Entities.Identity
{
    public class AppRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
```

#### Subject.cs (Ders)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Türkçe, Matematik vb.
        public string? Description { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public ICollection<AppUser> Teachers { get; set; } = new List<AppUser>();
    }
}
```

#### Grade.cs (Sınıf)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Grade : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // 7. Sınıf, 8. Sınıf vb.
        public int GradeLevel { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
```

#### Unit.cs (Ünite)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public int GradeId { get; set; }
        public Grade Grade { get; set; } = null!;

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
```

#### Topic.cs (Konu)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Topic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
```

#### Question.cs (Soru)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Question : BaseEntity
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? ImagePath { get; set; }       // wwwroot göreli yol
        public int Points { get; set; } = 1;
        public bool IsPublic { get; set; } = false;  // false = sadece kendi öğrencileri

        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;

        // Soruyu ekleyen öğretmen
        public string TeacherId { get; set; } = string.Empty;
        public AppUser Teacher { get; set; } = null!;

        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
```

#### Choice.cs (Şık)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Choice : BaseEntity
    {
        public string Label { get; set; } = string.Empty;  // A, B, C, D, E
        public string ChoiceText { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsCorrect { get; set; } = false;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
```

#### Exam.cs (Test/Sınav)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class Exam : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }

        public string TeacherId { get; set; } = string.Empty;
        public AppUser Teacher { get; set; } = null!;

        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    }
}
```

#### ExamQuestion.cs
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class ExamQuestion : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public int OrderIndex { get; set; }  // Karışık sıra için
    }
}
```

#### StudentExam.cs (Öğrenci-Sınav Atama)
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class StudentExam : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public AppUser Student { get; set; } = null!;
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int? Score { get; set; }
        public int? TotalPoints { get; set; }

        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
```

#### StudentAnswer.cs
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class StudentAnswer : BaseEntity
    {
        public int StudentExamId { get; set; }
        public StudentExam StudentExam { get; set; } = null!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public int? SelectedChoiceId { get; set; }
        public Choice? SelectedChoice { get; set; }
        public bool IsCorrect { get; set; } = false;
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }
}
```

#### StudentSubject.cs
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    public class StudentSubject : BaseEntity
    {
        public string StudentId { get; set; } = string.Empty;
        public AppUser Student { get; set; } = null!;
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}
```

### 4.3 Generic Repository Interface

```csharp
// Core/Interfaces/Repositories/IRepository.cs
namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
```

### 4.4 Enums

```csharp
// Core/Enums/UserRole.cs
namespace JelleSmart.ExamSystem.Core.Enums
{
    public static class UserRoles
    {
        public const string Admin   = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";
    }
}

// Core/Enums/ExamStatus.cs
namespace JelleSmart.ExamSystem.Core.Enums
{
    public enum ExamStatus
    {
        NotStarted  = 0,
        InProgress  = 1,
        Completed   = 2
    }
}
```

---

## 5. Repository Katmanı

### 5.1 AppDbContext

```csharp
// Repository/Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Subject>      Subjects      { get; set; }
        public DbSet<Grade>        Grades        { get; set; }
        public DbSet<Unit>         Units         { get; set; }
        public DbSet<Topic>        Topics        { get; set; }
        public DbSet<Question>     Questions     { get; set; }
        public DbSet<Choice>       Choices       { get; set; }
        public DbSet<Exam>         Exams         { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentExam>  StudentExams  { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Soft-delete global filter
            builder.Entity<Question>().HasQueryFilter(q => !q.IsDeleted);
            builder.Entity<Exam>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Subject>().HasQueryFilter(s => !s.IsDeleted);

            // Index'ler
            builder.Entity<StudentSubject>()
                .HasIndex(ss => new { ss.StudentId, ss.SubjectId }).IsUnique();

            builder.Entity<StudentAnswer>()
                .HasIndex(sa => new { sa.StudentExamId, sa.QuestionId }).IsUnique();

            // Cascade davranışları
            builder.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```

### 5.2 Generic Repository

```csharp
// Repository/Repositories/Repository.cs
namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)       => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync()   => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity)              => await _dbSet.AddAsync(entity);
        public void Update(T entity)                      => _dbSet.Update(entity);
        public void Delete(T entity)                      => _dbSet.Remove(entity);
        public async Task SaveChangesAsync()              => await _context.SaveChangesAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();
    }
}
```

### 5.3 Özel Repository Örnekleri

```csharp
// Repository/Repositories/QuestionRepository.cs
public class QuestionRepository : Repository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Question>> GetByTopicAsync(int topicId, string? teacherId = null)
    {
        var query = _dbSet
            .Include(q => q.Choices)
            .Include(q => q.Topic).ThenInclude(t => t.Unit)
            .Where(q => q.TopicId == topicId);

        if (teacherId != null)
            query = query.Where(q => q.IsPublic || q.TeacherId == teacherId);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsForExam(
        int topicId, int count, int totalPoints, string teacherId)
    {
        // Puana göre random soru seçimi
        var questions = await _dbSet
            .Include(q => q.Choices)
            .Where(q => q.TopicId == topicId && (q.IsPublic || q.TeacherId == teacherId))
            .ToListAsync();

        return SelectQuestionsForPoints(questions, count, totalPoints);
    }

    private IEnumerable<Question> SelectQuestionsForPoints(
        List<Question> pool, int count, int totalPoints)
    {
        // Greedy: puana göre sorular seç
        var selected = new List<Question>();
        var rng = new Random();
        var shuffled = pool.OrderBy(_ => rng.Next()).ToList();

        int remaining = totalPoints;
        foreach (var q in shuffled.Take(count))
        {
            selected.Add(q);
            remaining -= q.Points;
            if (selected.Count >= count || remaining <= 0) break;
        }
        return selected;
    }
}
```

```csharp
// Repository/Repositories/StudentExamRepository.cs
public class StudentExamRepository : Repository<StudentExam>, IStudentExamRepository
{
    public StudentExamRepository(AppDbContext context) : base(context) { }

    public async Task<StudentExam?> GetActiveExamAsync(string studentId, int examId)
        => await _dbSet
            .Include(se => se.Exam).ThenInclude(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question).ThenInclude(q => q.Choices)
            .Include(se => se.StudentAnswers)
            .FirstOrDefaultAsync(se => se.StudentId == studentId
                                    && se.ExamId == examId
                                    && !se.IsCompleted);

    public async Task<IEnumerable<StudentExam>> GetPendingExamsAsync(string studentId)
        => await _dbSet
            .Include(se => se.Exam)
            .Where(se => se.StudentId == studentId && !se.IsCompleted)
            .OrderBy(se => se.Exam.Title)
            .ToListAsync();

    public async Task<IEnumerable<StudentExam>> GetCompletedExamsAsync(string studentId)
        => await _dbSet
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .Where(se => se.StudentId == studentId && se.IsCompleted)
            .OrderByDescending(se => se.CompletedAt)
            .ToListAsync();
}
```

---

## 6. Service Katmanı

### 6.1 ExamService (Test Oluşturma)

```csharp
// Service/Services/ExamService.cs
public class ExamService : IExamService
{
    private readonly IExamRepository _examRepo;
    private readonly IQuestionRepository _questionRepo;
    private readonly IStudentExamRepository _studentExamRepo;

    public ExamService(IExamRepository examRepo,
                       IQuestionRepository questionRepo,
                       IStudentExamRepository studentExamRepo)
    {
        _examRepo       = examRepo;
        _questionRepo   = questionRepo;
        _studentExamRepo = studentExamRepo;
    }

    public async Task<Exam> CreateExamAsync(CreateExamDto dto, string teacherId)
    {
        var exam = new Exam
        {
            Title           = dto.Title,
            DurationMinutes = dto.DurationMinutes,
            TeacherId       = teacherId
        };
        await _examRepo.AddAsync(exam);
        await _examRepo.SaveChangesAsync();

        int order = 0;
        foreach (var section in dto.Sections)
        {
            var questions = await _questionRepo.GetQuestionsForExam(
                section.TopicId, section.QuestionCount,
                section.TotalPoints, teacherId);

            foreach (var q in questions)
            {
                await _examRepo.AddExamQuestionAsync(new ExamQuestion
                {
                    ExamId     = exam.Id,
                    QuestionId = q.Id,
                    OrderIndex = order++
                });
            }
        }

        // Öğrencilere ata
        foreach (var studentId in dto.StudentIds)
        {
            await _studentExamRepo.AddAsync(new StudentExam
            {
                ExamId    = exam.Id,
                StudentId = studentId
            });
        }
        await _examRepo.SaveChangesAsync();
        return exam;
    }
}
```

### 6.2 StudentExamService (Sınav Çözme)

```csharp
// Service/Services/StudentExamService.cs
public class StudentExamService : IStudentExamService
{
    private readonly IStudentExamRepository _repo;
    private readonly IRepository<StudentAnswer> _answerRepo;

    public async Task<StudentExam> StartExamAsync(string studentId, int examId)
    {
        var studentExam = await _repo.GetActiveExamAsync(studentId, examId)
            ?? throw new InvalidOperationException("Sınav bulunamadı.");

        if (studentExam.IsCompleted)
            throw new InvalidOperationException("Bu sınav tamamlanmış, tekrar açılamaz.");

        if (studentExam.StartedAt == null)
            studentExam.StartedAt = DateTime.UtcNow;

        // Soruları karıştır (her öğrenci için tekrarlanabilir seed)
        var seed = HashCode.Combine(studentId, examId);
        var rng  = new Random(seed);
        var questions = studentExam.Exam.ExamQuestions
            .OrderBy(_ => rng.Next()).ToList();
        studentExam.Exam.ExamQuestions = questions;

        _repo.Update(studentExam);
        await _repo.SaveChangesAsync();
        return studentExam;
    }

    public async Task SaveAnswerAsync(string studentId, int studentExamId,
                                      int questionId, int choiceId)
    {
        var studentExam = await _repo.GetByIdAsync(studentExamId)
            ?? throw new InvalidOperationException("Sınav bulunamadı.");

        if (studentExam.StudentId != studentId)
            throw new UnauthorizedAccessException();

        if (studentExam.IsCompleted)
            throw new InvalidOperationException("Tamamlanan sınav düzenlenemez.");

        // Süre kontrolü
        if (studentExam.StartedAt.HasValue)
        {
            var elapsed = DateTime.UtcNow - studentExam.StartedAt.Value;
            if (elapsed.TotalMinutes > studentExam.Exam.DurationMinutes)
                await CompleteExamAsync(studentId, studentExamId);
        }

        var existing = await _answerRepo.FindAsync(
            a => a.StudentExamId == studentExamId && a.QuestionId == questionId);

        var choice = studentExam.Exam.ExamQuestions
            .FirstOrDefault(eq => eq.QuestionId == questionId)
            ?.Question?.Choices.FirstOrDefault(c => c.Id == choiceId);

        if (existing.Any())
        {
            var ans = existing.First();
            ans.SelectedChoiceId = choiceId;
            ans.IsCorrect        = choice?.IsCorrect ?? false;
            ans.AnsweredAt       = DateTime.UtcNow;
            _answerRepo.Update(ans);
        }
        else
        {
            await _answerRepo.AddAsync(new StudentAnswer
            {
                StudentExamId    = studentExamId,
                QuestionId       = questionId,
                SelectedChoiceId = choiceId,
                IsCorrect        = choice?.IsCorrect ?? false
            });
        }
        await _answerRepo.SaveChangesAsync();
    }

    public async Task CompleteExamAsync(string studentId, int studentExamId)
    {
        var studentExam = await _repo.GetByIdAsync(studentExamId)
            ?? throw new InvalidOperationException();

        if (studentExam.IsCompleted)
            throw new InvalidOperationException("Sınav zaten tamamlandı.");

        var answers     = studentExam.StudentAnswers;
        var totalPoints = studentExam.Exam.ExamQuestions.Sum(eq => eq.Question.Points);
        var score       = answers.Where(a => a.IsCorrect)
                                 .Sum(a => a.Question.Points);

        studentExam.IsCompleted  = true;
        studentExam.CompletedAt  = DateTime.UtcNow;
        studentExam.Score        = score;
        studentExam.TotalPoints  = totalPoints;

        _repo.Update(studentExam);
        await _repo.SaveChangesAsync();
    }
}
```

### 6.3 FileService (Görsel Yükleme)

```csharp
// Service/Services/FileService.cs
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public FileService(IWebHostEnvironment env) => _env = env;

    /// <summary>
    /// Görsel kaydeder ve wwwroot'a göre relatif path döner.
    /// Klasör: uploads/{subject}/{grade}/{unit}/{topic}/question
    /// </summary>
    public async Task<string> SaveImageAsync(IFormFile file,
        int subjectId, int gradeId, int unitId, int topicId)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Desteklenmeyen dosya türü.");

        var folder = Path.Combine(_env.WebRootPath, "uploads",
            subjectId.ToString(), gradeId.ToString(),
            unitId.ToString(), topicId.ToString(), "question");

        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(folder, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        // wwwroot göreli yol (/ ile başlar)
        return $"/uploads/{subjectId}/{gradeId}/{unitId}/{topicId}/question/{fileName}";
    }

    public void DeleteImage(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}
```

---

## 7. WebUI Katmanı

### 7.1 Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequiredLength         = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase       = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IQuestionRepository,   QuestionRepository>();
builder.Services.AddScoped<IExamRepository,       ExamRepository>();
builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();

// Services
builder.Services.AddScoped<IExamService,        ExamService>();
builder.Services.AddScoped<IStudentExamService, StudentExamService>();
builder.Services.AddScoped<IQuestionService,    QuestionService>();
builder.Services.AddScoped<IReportService,      ReportService>();
builder.Services.AddScoped<IFileService,        FileService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Gerçek zamanlı süre sayacı için

var app = builder.Build();

// Seed rolleri
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await SeedData.InitializeAsync(roleManager, userManager);
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ExamHub>("/examHub");
app.Run();
```

### 7.2 Alan (Area) Yapısı

```
WebUI/Areas/
├── Admin/
│   ├── Controllers/
│   │   ├── UserManagementController.cs
│   │   ├── SubjectController.cs
│   │   ├── GradeController.cs
│   │   ├── UnitController.cs
│   │   ├── TopicController.cs
│   │   └── ReportController.cs
│   └── Views/...
├── Teacher/
│   ├── Controllers/
│   │   ├── QuestionController.cs
│   │   ├── ExamController.cs
│   │   └── ReportController.cs
│   └── Views/...
└── Student/
    ├── Controllers/
    │   ├── ExamController.cs
    │   └── ReportController.cs
    └── Views/...
```

### 7.3 Seed Data

```csharp
// WebUI/Data/SeedData.cs
public static class SeedData
{
    public static async Task InitializeAsync(
        RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
    {
        string[] roles = { "Admin", "Teacher", "Student" };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });

        // Admin kullanıcı
        if (await userManager.FindByEmailAsync("admin@jellesmart.com") == null)
        {
            var admin = new AppUser
            {
                UserName  = "admin@jellesmart.com",
                Email     = "admin@jellesmart.com",
                FirstName = "Sistem",
                LastName  = "Yöneticisi"
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
```

### 7.4 Controller Örnekleri

#### Student/ExamController.cs
```csharp
[Area("Student")]
[Authorize(Roles = "Student")]
public class ExamController : Controller
{
    private readonly IStudentExamService _service;
    private readonly UserManager<AppUser> _userManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId  = _userManager.GetUserId(User)!;
        var pending = await _service.GetPendingExamsAsync(userId);
        return View(pending);
    }

    [HttpGet]
    public async Task<IActionResult> Take(int studentExamId)
    {
        var userId     = _userManager.GetUserId(User)!;
        var studentExam = await _service.StartExamAsync(userId, studentExamId);
        return View(studentExam);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAnswer(
        [FromBody] SaveAnswerRequest request)
    {
        var userId = _userManager.GetUserId(User)!;
        await _service.SaveAnswerAsync(userId, request.StudentExamId,
                                       request.QuestionId, request.ChoiceId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int studentExamId)
    {
        var userId = _userManager.GetUserId(User)!;
        var unanswered = await _service.GetUnansweredCountAsync(userId, studentExamId);
        if (unanswered > 0)
            TempData["Warning"] = $"{unanswered} soru cevaplanmadı!";

        await _service.CompleteExamAsync(userId, studentExamId);
        return RedirectToAction("Result", new { studentExamId });
    }
}
```

#### Teacher/QuestionController.cs
```csharp
[Area("Teacher")]
[Authorize(Roles = "Teacher")]
public class QuestionController : Controller
{
    private readonly IQuestionService _questionService;
    private readonly IFileService _fileService;
    private readonly UserManager<AppUser> _userManager;

    [HttpPost]
    public async Task<IActionResult> Create(QuestionCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var teacherId = _userManager.GetUserId(User)!;
        var teacher   = await _userManager.GetUserAsync(User);

        // Öğretmen sadece kendi branşındaki derslere soru ekleyebilir
        var topic = await _questionService.GetTopicWithUnitAsync(model.TopicId);
        if (topic?.Unit.SubjectId != teacher?.SubjectId)
        {
            ModelState.AddModelError("", "Bu konuya soru ekleme yetkiniz yok.");
            return View(model);
        }

        string? imagePath = null;
        if (model.ImageFile != null)
            imagePath = await _fileService.SaveImageAsync(
                model.ImageFile,
                topic.Unit.SubjectId, topic.Unit.GradeId,
                topic.UnitId, topic.Id);

        await _questionService.CreateAsync(model, teacherId, imagePath);
        return RedirectToAction("Index");
    }
}
```

### 7.5 Sınav Çözme View (Geri Sayım + AJAX)

```html
<!-- Areas/Student/Views/Exam/Take.cshtml -->
@model StudentExam

<div class="exam-container">
    <div class="exam-header d-flex justify-content-between align-items-center mb-4">
        <h4>@Model.Exam.Title</h4>
        <div class="timer-box bg-danger text-white px-4 py-2 rounded fs-3" id="timer">
            --:--
        </div>
    </div>

    <div id="questions-container">
        @foreach (var eq in Model.Exam.ExamQuestions.OrderBy(q => q.OrderIndex))
        {
            var question = eq.Question;
            var answered = Model.StudentAnswers
                .FirstOrDefault(a => a.QuestionId == question.Id);

            <div class="card mb-4 question-card" id="q-@question.Id">
                <div class="card-body">
                    <p class="fw-bold">@question.QuestionText</p>
                    @if (!string.IsNullOrEmpty(question.ImagePath))
                    {
                        <img src="@question.ImagePath" alt="Soru görseli"
                             style="max-width:500px; max-height:300px; object-fit:contain;"
                             class="img-fluid mb-3" />
                    }

                    <div class="choices">
                        @foreach (var choice in question.Choices.OrderBy(c => c.Label))
                        {
                            var isSelected = answered?.SelectedChoiceId == choice.Id;
                            <label class="choice-label d-flex align-items-start gap-2 p-2 mb-2 border rounded
                                          @(isSelected ? "border-primary bg-primary bg-opacity-10" : "")">
                                <input type="radio"
                                       name="q_@question.Id"
                                       value="@choice.Id"
                                       @(isSelected ? "checked" : "")
                                       data-question-id="@question.Id"
                                       class="choice-radio mt-1" />
                                <span class="badge bg-secondary">@choice.Label</span>
                                <span>@choice.ChoiceText</span>
                                @if (!string.IsNullOrEmpty(choice.ImagePath))
                                {
                                    <img src="@choice.ImagePath"
                                         style="max-width:200px; max-height:120px; object-fit:contain;" />
                                }
                            </label>
                        }
                    </div>
                </div>
            </div>
        }
    </div>

    <div class="text-end mt-4">
        <button id="complete-btn" class="btn btn-success btn-lg px-5">
            Sınavı Tamamla
        </button>
    </div>
</div>

@section Scripts {
<script>
    const DURATION_MINUTES = @Model.Exam.DurationMinutes;
    const START_TIME       = new Date('@Model.StartedAt?.ToString("o")');
    const STUDENT_EXAM_ID  = @Model.Id;

    // Geri Sayım
    function updateTimer() {
        const elapsed  = (Date.now() - START_TIME) / 1000;
        const remaining = DURATION_MINUTES * 60 - elapsed;

        if (remaining <= 0) {
            completeExam();
            return;
        }

        const m = Math.floor(remaining / 60).toString().padStart(2, '0');
        const s = Math.floor(remaining % 60).toString().padStart(2, '0');
        document.getElementById('timer').textContent = `${m}:${s}`;

        const timerEl = document.getElementById('timer');
        timerEl.classList.toggle('bg-warning', remaining < 300 && remaining >= 60);
        timerEl.classList.toggle('bg-danger',  remaining < 60);
    }
    setInterval(updateTimer, 1000);
    updateTimer();

    // AJAX cevap kaydetme
    document.querySelectorAll('.choice-radio').forEach(radio => {
        radio.addEventListener('change', async function () {
            await fetch('/Student/Exam/SaveAnswer', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json',
                           'RequestVerificationToken': document.querySelector(
                               'input[name="__RequestVerificationToken"]').value },
                body: JSON.stringify({
                    studentExamId: STUDENT_EXAM_ID,
                    questionId:    parseInt(this.dataset.questionId),
                    choiceId:      parseInt(this.value)
                })
            });

            // Seçili şıkkı vurgula
            document.querySelectorAll(`[name="q_${this.dataset.questionId}"]`)
                .forEach(r => r.closest('label').classList.remove(
                    'border-primary', 'bg-primary', 'bg-opacity-10'));
            this.closest('label').classList.add(
                'border-primary', 'bg-primary', 'bg-opacity-10');
        });
    });

    // Tamamla
    async function completeExam() {
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = `/Student/Exam/Complete`;
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'studentExamId';
        input.value = STUDENT_EXAM_ID;
        const token = document.createElement('input');
        token.type = 'hidden';
        token.name = '__RequestVerificationToken';
        token.value = document.querySelector(
            'input[name="__RequestVerificationToken"]').value;
        form.appendChild(input);
        form.appendChild(token);
        document.body.appendChild(form);
        form.submit();
    }

    document.getElementById('complete-btn').addEventListener('click', function() {
        const total    = document.querySelectorAll('.question-card').length;
        const answered = document.querySelectorAll('.choice-radio:checked').length;
        if (answered < total) {
            if (!confirm(`${total - answered} soru cevaplanmadı. Yine de tamamlamak istiyor musunuz?`))
                return;
        }
        completeExam();
    });
</script>
}
```

---

## 8. Görsel Yükleme Kuralları

```
wwwroot/
└── uploads/
    └── {subjectId}/
        └── {gradeId}/
            └── {unitId}/
                └── {topicId}/
                    └── question/
                        ├── 3f4a8c2d-1e2b-4a5f-9c1d-6e7f8a9b0c1e.jpg   (Soru görseli)
                        └── choice/
                            └── 7b8c9d0e-2f3a-4b5c-6d7e-8f9a0b1c2d3e.png (Şık görseli)
```

**View'da kullanım:**
```html
<img src="@question.ImagePath"
     style="max-width: 600px; max-height: 350px; object-fit: contain;"
     class="img-fluid" />
```

---

## 9. Raporlama (Chart.js)

### Öğrenci Raporu ViewModel
```csharp
public class StudentReportDto
{
    public string StudentName { get; set; } = string.Empty;
    public List<SubjectProgress> SubjectProgresses { get; set; } = new();
}

public class SubjectProgress
{
    public string SubjectName    { get; set; } = string.Empty;
    public double AverageScore   { get; set; }
    public int    ExamCount      { get; set; }
    public List<ExamScoreDto> ExamScores { get; set; } = new();
}
```

### Chart.js Entegrasyonu (Radar + Line)
```html
<canvas id="progressChart"></canvas>
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
new Chart(document.getElementById('progressChart'), {
    type: 'radar',
    data: {
        labels: @Json.Serialize(Model.Labels),
        datasets: [{
            label: 'Başarı Oranı (%)',
            data: @Json.Serialize(Model.Scores),
            fill: true,
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)'
        }]
    }
});
</script>
```

---

## 10. Rol Bazlı Yetkilendirme

```csharp
// Öğretmen sadece kendi branşına ait konulara erişebilir
[Authorize(Roles = "Teacher")]
public async Task<IActionResult> MySubjectQuestions()
{
    var teacher = await _userManager.GetUserAsync(User);
    if (teacher?.SubjectId == null)
        return Forbid();

    var questions = await _questionService
        .GetBySubjectAsync(teacher.SubjectId.Value, teacher.Id);
    return View(questions);
}
```

---

## 11. Migration & Veritabanı

```bash
# Repository katmanında
dotnet ef migrations add InitialCreate \
    --project JelleSmart.ExamSystem.Repository \
    --startup-project JelleSmart.ExamSystem.WebUI

dotnet ef database update \
    --project JelleSmart.ExamSystem.Repository \
    --startup-project JelleSmart.ExamSystem.WebUI
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=JelleSmartExamDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 12. Claude Code ile Geliştirme İpuçları

Aşağıdaki komut sırasını Claude Code'da kullanabilirsiniz:

```
1. "JelleSmart.ExamSystem solution'ını yukarıdaki yapıya göre oluştur"
2. "Core katmanındaki tüm entity sınıflarını yaz"
3. "AppDbContext ve generic Repository'i implement et"
4. "IExamService ve ExamService'i implement et"
5. "Admin alanı için CRUD controller'larını yaz"
6. "Teacher/QuestionController soru ekleme/güncelleme/silme ekle"
7. "Student/ExamController sınav çözme ekranını yaz"
8. "Chart.js ile öğrenci rapor sayfasını ekle"
9. "Migration oluştur ve seed data ekle"
```

---

## 13. Kontrol Listesi (Checklist)

- [ ] Solution ve proje yapısı oluşturuldu
- [ ] NuGet paketleri eklendi
- [ ] Core entity'ler yazıldı
- [ ] IRepository ve implementasyonlar hazır
- [ ] AppDbContext ve Identity kurulumu yapıldı
- [ ] Service katmanı implement edildi
- [ ] FileService görsel yükleme çalışıyor
- [ ] Admin area: Kullanıcı, Ders, Sınıf, Ünite, Konu CRUD
- [ ] Teacher area: Soru ekleme (görsel + şık), Test oluşturma
- [ ] Student area: Sınav listesi, Sınav çözme (geri sayım + AJAX), Sonuç
- [ ] Rol bazlı yetkilendirme kontrolü
- [ ] Raporlama (Chart.js radar/line grafik)
- [ ] Soft delete tüm entity'lerde aktif
- [ ] Migration ve Seed data hazır
- [ ] wwwroot/uploads klasör yapısı doğru
