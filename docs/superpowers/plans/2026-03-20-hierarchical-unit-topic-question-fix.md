# Hiyerarşik Ünite-Konu-Soru Düzeltmesi - Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ders → Sınıf → Ünite → Konu → Sorular hiyerarşisini düzeltmek, Select2 eklemek ve validation eklemek

**Architecture:**
- Subject-Grade many-to-many ilişkisi için SubjectGrade join table
- Unit ve Topic entity'lerine GradeId ve Order field eklenmesi
- Question entity'sinden SubjectId, UnitId, GradeId kaldırılıp sadece TopicId ile ilişki
- Cascading dropdown API endpoint'leri (BySubject, ByGrade, ByUnit)
- FluentValidation ile unique constraints
- Select2 ile tüm select list'ler

**Tech Stack:**
- ASP.NET Core 8+
- EF Core
- FluentValidation
- Select2 (jQuery plugin)
- Metronic 7 theme

---

## Chunk 1: Database Reset and New Migrations

> Bu chunk mevcut database'i siler ve yeni hiyerarşik yapı ile baştan oluşturur. Test verisi olduğu için güvenlidir.

### Task 1: Drop Existing Database and Migrations

**Files:**
- Delete: `JelleSmart.ExamSystem.Repository/Migrations/*.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Data/AppDbContext.cs`

- [ ] **Step 1: Delete all migration files**

Run:
```bash
cd JelleSmart.ExamSystem.Repository
rm -rf Migrations/*.cs
```

Expected: Migrations klasörü boşalır (sadece .gitkeep varsa)

- [ ] **Step 2: Drop the database**

Run SQL (SSMS veya:
```sql
USE master;
GO
ALTER DATABASE [JelleSmartExamSystem] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE [JelleSmartExamSystem];
GO
```

Veya CLI:
```bash
dotnet ef database drop --force
```

Expected: Database silinir

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "feat: drop database and migrations for hierarchical refactor"
```

---

### Task 2: Create SubjectGrade Entity

**Files:**
- Create: `JelleSmart.ExamSystem.Core/Entities/SubjectGrade.cs`

- [ ] **Step 1: Create SubjectGrade entity**

Create file `JelleSmart.ExamSystem.Core/Entities/SubjectGrade.cs`:
```csharp
using JelleSmart.ExamSystem.Core.Entities.Base;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ders-Sınıf ilişkisi (Many-to-Many)
    /// Her dersin hangi sınıflarda öğretildiğini tanımlar
    /// </summary>
    public class SubjectGrade : BaseEntity
    {
        public string SubjectId { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;

        // Navigation properties
        public Subject? Subject { get; set; }
        public Grade? Grade { get; set; }
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build JelleSmart.ExamSystem.Core
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/SubjectGrade.cs
git commit -m "feat: add SubjectGrade many-to-many entity"
```

---

### Task 3: Update Subject Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Subject.cs`

- [ ] **Step 1: Update Subject entity navigation properties**

Replace content of `JelleSmart.ExamSystem.Core/Entities/Subject.cs`:
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ders (Örn: Matematik, Türkçe, Fen Bilimleri)
    /// </summary>
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; } // CSS icon class (FontAwesome vb.)

        // Navigation properties
        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS (Question reference hata verebilir, sonraki task'te düzeltilecek)

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Subject.cs
git commit -m "refactor: update Subject with SubjectGrades navigation"
```

---

### Task 4: Update Grade Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Grade.cs`

- [ ] **Step 1: Update Grade entity navigation properties**

Replace content of `JelleSmart.ExamSystem.Core/Entities/Grade.cs`:
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Sınıf (1, 2, 3, 4)
    /// </summary>
    public class Grade : BaseEntity
    {
        public int Level { get; set; } // 1, 2, 3, 4
        public string Name { get; set; } = string.Empty; // "1. Sınıf", "2. Sınıf" vb.

        // Navigation properties
        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<AppUser> Students { get; set; } = new List<AppUser>();
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Grade.cs
git commit -m "refactor: update Grade with SubjectGrades and Units navigation"
```

---

### Task 5: Update Unit Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Unit.cs`

- [ ] **Step 1: Update Unit entity**

Replace content of `JelleSmart.ExamSystem.Core/Entities/Unit.cs`:
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ünite (Dersin alt başlıkları)
    /// Her ünite bir sınıfa aittir
    /// </summary>
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; } = 1; // Sıra numarası

        // Foreign keys
        public string GradeId { get; set; } = string.Empty; // Required

        // Navigation properties
        public Grade? Grade { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Unit.cs
git commit -m "refactor: update Unit with GradeId and Order field"
```

---

### Task 6: Update Topic Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Topic.cs`

- [ ] **Step 1: Update Topic entity**

Replace content of `JelleSmart.ExamSystem.Core/Entities/Topic.cs`:
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Konu (Ünitenin alt başlıkları)
    /// Öğretmenler soruları konulara göre oluşturur
    /// Sınav oluştururken konu seçilir ve rastgele sorular çekilir
    /// </summary>
    public class Topic : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; } // Konu kodu (Örn: M.1.1.1)
        public int Order { get; set; } = 1; // Sıra numarası

        // Foreign keys
        public string UnitId { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty; // Required

        // Navigation properties
        public Unit? Unit { get; set; }
        public Grade? Grade { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Topic.cs
git commit -m "refactor: update Topic with GradeId and Order field"
```

---

### Task 7: Update Question Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Question.cs`

- [ ] **Step 1: Update Question entity**

Replace content of `JelleSmart.ExamSystem.Core/Entities/Question.cs`:
```csharp
using JelleSmart.ExamSystem.Core.Entities.Identity;

namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Soru
    /// Her soru mutlaka bir konuya aittir
    /// Ders, Sınıf ve Ünite bilgisi konudan inherit edilir
    /// </summary>
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // Soru görseli
        public string? Explanation { get; set; } // Açıklama/Cevap anahtarı notu
        public int Difficulty { get; set; } = 1; // 1: Kolay, 2: Orta, 3: Zor

        // Foreign keys
        public string TopicId { get; set; } = string.Empty; // Required
        public string CreatedByUserId { get; set; } = string.Empty;

        // Navigation properties
        public Topic? Topic { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Question.cs
git commit -m "refactor: remove SubjectId/UnitId/GradeId from Question, keep only TopicId"
```

---

### Task 8: Update Exam Entity

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Exam.cs`

- [ ] **Step 1: Read current Exam entity**

Run:
```bash
cat JelleSmart.ExamSystem.Core/Entities/Exam.cs
```

- [ ] **Step 2: Add OrderQuestionsByTopicSequence field**

Find the property section and add:
```csharp
public bool OrderQuestionsByTopicSequence { get; set; } = false;
```

Full example (after existing fields):
```csharp
// ... existing fields ...
public bool OrderQuestionsByTopicSequence { get; set; } = false;
// ... existing fields ...
```

- [ ] **Step 3: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Exam.cs
git commit -m "feat: add OrderQuestionsByTopicSequence to Exam"
```

---

### Task 9: Update AppDbContext

**Files:**
- Modify: `JelleSmart.ExamSystem.Repository/Data/AppDbContext.cs`

- [ ] **Step 1: Add SubjectGrade DbSet**

Add to AppDbContext:
```csharp
public DbSet<SubjectGrade> SubjectGrades { get; set; }
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Repository/Data/AppDbContext.cs
git commit -m "feat: add SubjectGrades DbSet to AppDbContext"
```

---

### Task 10: Create EF Core Configurations

**Files:**
- Create: `JelleSmart.ExamSystem.Repository/Configurations/SubjectGradeConfiguration.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Configurations/SubjectConfiguration.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Configurations/GradeConfiguration.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Configurations/UnitConfiguration.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Configurations/TopicConfiguration.cs`

- [ ] **Step 1: Create SubjectGradeConfiguration**

Create `JelleSmart.ExamSystem.Repository/Configurations/SubjectGradeConfiguration.cs`:
```csharp
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

            // Composite unique index - aynı ders-sınıf ilişkisi tekrar edilemez
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
```

- [ ] **Step 2: Update SubjectConfiguration for unique name**

Read current `JelleSmart.ExamSystem.Repository/Configurations/SubjectConfiguration.cs` and add unique constraint:
```csharp
builder.HasIndex(s => s.Name)
    .IsUnique()
    .HasFilter("LOWER(Name) = LOWER(Name)");
```

- [ ] **Step 3: Update GradeConfiguration for unique Level**

Read current `JelleSmart.ExamSystem.Repository/Configurations/GradeConfiguration.cs` and add:
```csharp
builder.HasIndex(g => g.Level)
    .IsUnique();
```

- [ ] **Step 4: Update UnitConfiguration**

Read current `JelleSmart.ExamSystem.Repository/Configurations/UnitConfiguration.cs` and update:
```csharp
public void Configure(EntityTypeBuilder<Unit> builder)
{
    builder.HasKey(u => u.Id);

    // Composite unique index - aynı sınıfta aynı sıra numarası olamaz
    builder.HasIndex(u => new { u.GradeId, u.Order })
        .IsUnique();

    builder.HasOne(u => u.Grade)
        .WithMany(g => g.Units)
        .HasForeignKey(u => u.GradeId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(u => u.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(u => u.Description)
        .HasMaxLength(1000);

    builder.Property(u => u.Order)
        .HasDefaultValue(1);
}
```

- [ ] **Step 5: Update TopicConfiguration**

Read current `JelleSmart.ExamSystem.Repository/Configurations/TopicConfiguration.cs` and update:
```csharp
public void Configure(EntityTypeBuilder<Topic> builder)
{
    builder.HasKey(t => t.Id);

    // Composite unique index - aynı ünitede aynı sıra numarası olamaz
    builder.HasIndex(t => new { t.UnitId, t.Order })
        .IsUnique();

    builder.HasOne(t => t.Unit)
        .WithMany(u => u.Topics)
        .HasForeignKey(t => t.UnitId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(t => t.Grade)
        .WithMany()
        .HasForeignKey(t => t.GradeId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Property(t => t.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(t => t.Code)
        .HasMaxLength(50);

    builder.Property(t => t.Description)
        .HasMaxLength(1000);

    builder.Property(t => t.Order)
        .HasDefaultValue(1);
}
```

- [ ] **Step 6: Register configurations in AppDbContext**

In `OnModelCreating` method, add:
```csharp
modelBuilder.ApplyConfiguration(new SubjectGradeConfiguration());
```

- [ ] **Step 7: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 8: Commit**

```bash
git add JelleSmart.ExamSystem.Repository/Configurations/
git commit -m "feat: add EF Core configurations with unique constraints"
```

---

### Task 11: Create Initial Migration

**Files:**
- Create: `JelleSmart.ExamSystem.Repository/Migrations/*.cs`

- [ ] **Step 1: Create migration**

Run:
```bash
cd JelleSmart.ExamSystem.Repository
dotnet ef migrations add InitialHierarchicalStructure --startup-project ../JelleSmart.ExamSystem.WebUI
```

Expected: Migration files created in Migrations folder

- [ ] **Step 2: Review generated migration**

Run:
```bash
cat Migrations/*_InitialHierarchicalStructure.cs
```

Verify:
- SubjectGrade table created
- Unit has GradeId and Order
- Topic has GradeId and Order
- Question only has TopicId
- Unique indexes created

- [ ] **Step 3: Apply migration**

Run:
```bash
dotnet ef database update --startup-project ../JelleSmart.ExamSystem.WebUI
```

Expected: Database created with new schema

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Repository/Migrations/
git commit -m "feat: create initial migration for hierarchical structure"
```

---

## Chunk 2: Repository Layer

> Bu chunk repository layer'ı günceller - yeni metodlar ekler.

### Task 12: Create ISubjectGradeRepository and Implementation

**Files:**
- Create: `JelleSmart.ExamSystem.Core/Interfaces/Repositories/ISubjectGradeRepository.cs`
- Create: `JelleSmart.ExamSystem.Repository/Repositories/SubjectGradeRepository.cs`

- [ ] **Step 1: Create ISubjectGradeRepository interface**

Create `JelleSmart.ExamSystem.Core/Interfaces/Repositories/ISubjectGradeRepository.cs`:
```csharp
using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface ISubjectGradeRepository
    {
        Task<IEnumerable<SubjectGrade>> GetAllAsync();
        Task<SubjectGrade?> GetByIdAsync(string subjectId, string gradeId);
        Task<SubjectGrade?> GetRelationAsync(string subjectId, string gradeId);
        Task AddAsync(SubjectGrade subjectGrade);
        Task DeleteAsync(SubjectGrade subjectGrade);
        Task<bool> ExistsAsync(string subjectId, string gradeId);
        Task<IEnumerable<Grade>> GetGradesBySubjectIdAsync(string subjectId);
        Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId);
    }
}
```

- [ ] **Step 2: Create SubjectGradeRepository**

Create `JelleSmart.ExamSystem.Repository/Repositories/SubjectGradeRepository.cs`:
```csharp
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class SubjectGradeRepository : ISubjectGradeRepository
    {
        private readonly AppDbContext _context;

        public SubjectGradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectGrade>> GetAllAsync()
        {
            return await _context.SubjectGrades
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .ToListAsync();
        }

        public async Task<SubjectGrade?> GetByIdAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .FirstOrDefaultAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task<SubjectGrade?> GetRelationAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .FirstOrDefaultAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task AddAsync(SubjectGrade subjectGrade)
        {
            await _context.SubjectGrades.AddAsync(subjectGrade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SubjectGrade subjectGrade)
        {
            _context.SubjectGrades.Remove(subjectGrade);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string subjectId, string gradeId)
        {
            return await _context.SubjectGrades
                .AnyAsync(sg => sg.SubjectId == subjectId && sg.GradeId == gradeId);
        }

        public async Task<IEnumerable<Grade>> GetGradesBySubjectIdAsync(string subjectId)
        {
            return await _context.SubjectGrades
                .Where(sg => sg.SubjectId == subjectId)
                .Select(sg => sg.Grade)
                .OrderBy(g => g.Level)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            return await _context.SubjectGrades
                .Where(sg => sg.GradeId == gradeId)
                .Select(sg => sg.Subject)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
```

- [ ] **Step 3: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Repositories/ISubjectGradeRepository.cs
git add JelleSmart.ExamSystem.Repository/Repositories/SubjectGradeRepository.cs
git commit -m "feat: add SubjectGradeRepository with cascading methods"
```

---

### Task 13: Update IUnitRepository and Implementation

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Repositories/IUnitRepository.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Repositories/UnitRepository.cs`

- [ ] **Step 1: Read current IUnitRepository**

Run:
```bash
cat JelleSmart.ExamSystem.Core/Interfaces/Repositories/IUnitRepository.cs
```

- [ ] **Step 2: Add new methods to IUnitRepository**

Add these methods:
```csharp
Task<IEnumerable<Unit>> GetByGradeIdAsync(string gradeId);
Task<Unit?> GetByGradeAndOrderAsync(string gradeId, int order);
Task<int> GetNextOrderNumberAsync(string gradeId);
```

- [ ] **Step 3: Update UnitRepository implementation**

Add implementations:
```csharp
public async Task<IEnumerable<Unit>> GetByGradeIdAsync(string gradeId)
{
    return await _context.Units
        .Where(u => u.GradeId == gradeId)
        .OrderBy(u => u.Order)
        .ToListAsync();
}

public async Task<Unit?> GetByGradeAndOrderAsync(string gradeId, int order)
{
    return await _context.Units
        .FirstOrDefaultAsync(u => u.GradeId == gradeId && u.Order == order);
}

public async Task<int> GetNextOrderNumberAsync(string gradeId)
{
    var lastOrder = await _context.Units
        .Where(u => u.GradeId == gradeId)
        .MaxAsync(u => (int?)u.Order) ?? 0;
    return lastOrder + 1;
}
```

- [ ] **Step 4: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 5: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Repositories/IUnitRepository.cs
git add JelleSmart.ExamSystem.Repository/Repositories/UnitRepository.cs
git commit -m "feat: add Grade-based query methods to UnitRepository"
```

---

### Task 14: Update ITopicRepository and Implementation

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Repositories/ITopicRepository.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Repositories/TopicRepository.cs`

- [ ] **Step 1: Read current ITopicRepository**

Run:
```bash
cat JelleSmart.ExamSystem.Core/Interfaces/Repositories/ITopicRepository.cs
```

- [ ] **Step 2: Add new methods to ITopicRepository**

Add these methods:
```csharp
Task<IEnumerable<Topic>> GetByUnitIdAsync(string unitId);
Task<Topic?> GetByUnitAndOrderAsync(string unitId, int order);
Task<int> GetNextOrderNumberAsync(string unitId);
```

- [ ] **Step 3: Update TopicRepository implementation**

Add implementations:
```csharp
public async Task<IEnumerable<Topic>> GetByUnitIdAsync(string unitId)
{
    return await _context.Topics
        .Where(t => t.UnitId == unitId)
        .OrderBy(t => t.Order)
        .ToListAsync();
}

public async Task<Topic?> GetByUnitAndOrderAsync(string unitId, int order)
{
    return await _context.Topics
        .FirstOrDefaultAsync(t => t.UnitId == unitId && t.Order == order);
}

public async Task<int> GetNextOrderNumberAsync(string unitId)
{
    var lastOrder = await _context.Topics
        .Where(t => t.UnitId == unitId)
        .MaxAsync(t => (int?)t.Order) ?? 0;
    return lastOrder + 1;
}
```

- [ ] **Step 4: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 5: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Repositories/ITopicRepository.cs
git add JelleSmart.ExamSystem.Repository/Repositories/TopicRepository.cs
git commit -m "feat: add Unit-based query methods to TopicRepository"
```

---

### Task 15: Update IQuestionRepository and Implementation

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Repositories/IQuestionRepository.cs`
- Modify: `JelleSmart.ExamSystem.Repository/Repositories/QuestionRepository.cs`

- [ ] **Step 1: Read current IQuestionRepository**

Run:
```bash
cat JelleSmart.ExamSystem.Core/Interfaces/Repositories/IQuestionRepository.cs
```

- [ ] **Step 2: Simplify - only Topic-based methods needed**

Update to only use TopicId:
```csharp
Task<IEnumerable<Question>> GetByTopicIdAsync(string topicId);
```

- [ ] **Step 3: Update QuestionRepository implementation**

```csharp
public async Task<IEnumerable<Question>> GetByTopicIdAsync(string topicId)
{
    return await _context.Questions
        .Include(q => q.Topic)
            .ThenInclude(t => t.Unit)
        .Include(q => q.Choices)
        .Where(q => q.TopicId == topicId)
        .ToListAsync();
}
```

- [ ] **Step 4: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 5: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Repositories/IQuestionRepository.cs
git add JelleSmart.ExamSystem.Repository/Repositories/QuestionRepository.cs
git commit -m "refactor: simplify QuestionRepository to use only TopicId"
```

---

## Chunk 3: ViewModels

> Bu chunk ViewModels'ı günceller.

### Task 16: Update UnitViewModel

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/ViewModels/UnitViewModel.cs`

- [ ] **Step 1: Update UnitViewModel**

Replace content:
```csharp
using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class UnitViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Ünite adı gereklidir")]
        [StringLength(200, ErrorMessage = "Ünite adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string GradeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sıra numarası gereklidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1'den büyük olmalıdır")]
        public int Order { get; set; } = 1;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        // For display purposes
        public string? GradeName { get; set; }
        public string? SubjectName { get; set; } // Derived from Grade
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Core/ViewModels/UnitViewModel.cs
git commit -m "refactor: update UnitViewModel with GradeId and Order"
```

---

### Task 17: Update TopicViewModel

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/ViewModels/TopicViewModel.cs`

- [ ] **Step 1: Update TopicViewModel**

Replace content:
```csharp
using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class TopicViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Konu adı gereklidir")]
        [StringLength(200, ErrorMessage = "Konu adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ünite seçimi gereklidir")]
        public string UnitId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string GradeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sıra numarası gereklidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Sıra numarası 1'den büyük olmalıdır")]
        public int Order { get; set; } = 1;

        [StringLength(50, ErrorMessage = "Kod en fazla 50 karakter olabilir")]
        public string? Code { get; set; }

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        // For display purposes
        public string? UnitName { get; set; }
        public string? GradeName { get; set; }
        public string? SubjectName { get; set; } // Derived from Unit
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Core/ViewModels/TopicViewModel.cs
git commit -m "refactor: update TopicViewModel with GradeId and Order"
```

---

### Task 18: Update QuestionViewModel

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/ViewModels/QuestionViewModels.cs`

- [ ] **Step 1: Read current QuestionViewModels**

Run:
```bash
cat JelleSmart.ExamSystem.Core/ViewModels/QuestionViewModels.cs
```

- [ ] **Step 2: Update QuestionViewModel**

Replace the QuestionViewModel class:
```csharp
public class QuestionViewModel
{
    public string? Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Difficulty { get; set; } = 1;
    public string TopicId { get; set; } = string.Empty; // Required

    // UI cascade helper fields (not submitted)
    public string? SubjectId { get; set; }
    public string? GradeId { get; set; }
    public string? UnitId { get; set; }

    public IFormFile? ImageFile { get; set; }
    public List<ChoiceViewModel> Choices { get; set; } = new();
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/ViewModels/QuestionViewModels.cs
git commit -m "refactor: update QuestionViewModel - only TopicId, add cascade helpers"
```

---

### Task 19: Create SubjectGradeViewModel

**Files:**
- Create: `JelleSmart.ExamSystem.Core/ViewModels/SubjectGradeViewModel.cs`

- [ ] **Step 1: Create SubjectGradeViewModel**

Create file:
```csharp
using System.ComponentModel.DataAnnotations;

namespace JelleSmart.ExamSystem.Core.ViewModels
{
    public class SubjectGradeViewModel
    {
        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sınıf seçimi gereklidir")]
        public string GradeId { get; set; } = string.Empty;

        // For display
        public string? SubjectName { get; set; }
        public string? GradeName { get; set; }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Core/ViewModels/SubjectGradeViewModel.cs
git commit -m "feat: add SubjectGradeViewModel"
```

---

## Chunk 4: FluentValidation

> Bu chunk validation katmanını ekler.

### Task 20: Add FluentValidation Package

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj`

- [ ] **Step 1: Add FluentValidation package**

Run:
```bash
dotnet add JelleSmart.ExamSystem.Service package FluentValidation
dotnet add JelleSmart.ExamSystem.Service package FluentValidation.AspNetCore
```

Expected: Packages added

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj
git commit -m "deps: add FluentValidation packages"
```

---

### Task 21: Create Validators Folder and Base Validator

**Files:**
- Create: `JelleSmart.ExamSystem.Service/Validators/BaseValidator.cs`

- [ ] **Step 1: Create Validators folder**

Run:
```bash
mkdir -p JelleSmart.ExamSystem.Service/Validators
```

- [ ] **Step 2: Create BaseValidator**

Create `JelleSmart.ExamSystem.Service/Validators/BaseValidator.cs`:
```csharp
namespace JelleSmart.ExamSystem.Service.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseValidator()
        {
            // Common validation rules can be added here
        }
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Validators/
git commit -m "feat: add Validators folder and BaseValidator"
```

---

### Task 22: Create UnitValidator

**Files:**
- Create: `JelleSmart.ExamSystem.Service/Validators/UnitValidator.cs`

- [ ] **Step 1: Create UnitValidator**

Create file:
```csharp
using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class UnitValidator : BaseValidator<UnitViewModel>
    {
        private readonly IUnitRepository _unitRepository;

        public UnitValidator(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;

            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Ünite adı gereklidir")
                .MaximumLength(200).WithMessage("Ünite adı en fazla 200 karakter olabilir");

            RuleFor(u => u.Order)
                .GreaterThan(0).WithMessage("Sıra numarası 0'dan büyük olmalıdır");

            RuleFor(u => u.GradeId)
                .NotEmpty().WithMessage("Sınıf seçimi gereklidir");

            RuleFor(u => u)
                .MustAsync(async (unit, ct) =>
                {
                    var existing = await _unitRepository.GetByGradeAndOrderAsync(unit.GradeId, unit.Order);
                    return existing == null || existing.Id == unit.Id;
                }).WithMessage("Bu sınıfta bu sıra numarası zaten kullanımda")
                .WithName("Order");
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Validators/UnitValidator.cs
git commit -m "feat: add UnitValidator with unique Order validation"
```

---

### Task 23: Create TopicValidator

**Files:**
- Create: `JelleSmart.ExamSystem.Service/Validators/TopicValidator.cs`

- [ ] **Step 1: Create TopicValidator**

Create file:
```csharp
using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class TopicValidator : BaseValidator<TopicViewModel>
    {
        private readonly ITopicRepository _topicRepository;

        public TopicValidator(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;

            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Konu adı gereklidir")
                .MaximumLength(200).WithMessage("Konu adı en fazla 200 karakter olabilir");

            RuleFor(t => t.Order)
                .GreaterThan(0).WithMessage("Sıra numarası 0'dan büyük olmalıdır");

            RuleFor(t => t.UnitId)
                .NotEmpty().WithMessage("Ünite seçimi gereklidir");

            RuleFor(t => t.GradeId)
                .NotEmpty().WithMessage("Sınıf seçimi gereklidir");

            RuleFor(t => t)
                .MustAsync(async (topic, ct) =>
                {
                    var existing = await _topicRepository.GetByUnitAndOrderAsync(topic.UnitId, topic.Order);
                    return existing == null || existing.Id == topic.Id;
                }).WithMessage("Bu ünitede bu sıra numarası zaten kullanımda")
                .WithName("Order");
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Validators/TopicValidator.cs
git commit -m "feat: add TopicValidator with unique Order validation"
```

---

### Task 24: Create SubjectValidator, GradeValidator, SubjectGradeValidator

**Files:**
- Create: `JelleSmart.ExamSystem.Service/Validators/SubjectValidator.cs`
- Create: `JelleSmart.ExamSystem.Service/Validators/GradeValidator.cs`
- Create: `JelleSmart.ExamSystem.Service/Validators/SubjectGradeValidator.cs`

- [ ] **Step 1: Create SubjectValidator**

Create `JelleSmart.ExamSystem.Service/Validators/SubjectValidator.cs`:
```csharp
using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class SubjectValidator : BaseValidator<Subject>
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectValidator(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;

            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Ders adı gereklidir")
                .MaximumLength(200).WithMessage("Ders adı en fazla 200 karakter olabilir")
                .MustAsync(async (name, ct) =>
                {
                    var existing = await _subjectRepository.GetByNameAsync(name);
                    return existing == null;
                }).WithMessage("Bu isimde bir ders zaten mevcut");
        }
    }
}
```

- [ ] **Step 2: Create GradeValidator**

Create `JelleSmart.ExamSystem.Service/Validators/GradeValidator.cs`:
```csharp
using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class GradeValidator : BaseValidator<Grade>
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeValidator(IGradeRepository gradeRepository)
        {
            _gradeRepository = gradeRepository;

            RuleFor(g => g.Level)
                .InclusiveBetween(1, 12).WithMessage("Sınıf seviyesi 1-12 arasında olmalıdır")
                .MustAsync(async (level, ct) =>
                {
                    var existing = await _gradeRepository.GetByLevelAsync(level);
                    return existing == null;
                }).WithMessage("Bu seviyede bir sınıf zaten mevcut");

            RuleFor(g => g.Name)
                .NotEmpty().WithMessage("Sınıf adı gereklidir");
        }
    }
}
```

- [ ] **Step 3: Create SubjectGradeValidator**

Create `JelleSmart.ExamSystem.Service/Validators/SubjectGradeValidator.cs`:
```csharp
using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class SubjectGradeValidator : BaseValidator<SubjectGrade>
    {
        private readonly ISubjectGradeRepository _subjectGradeRepository;

        public SubjectGradeValidator(ISubjectGradeRepository subjectGradeRepository)
        {
            _subjectGradeRepository = subjectGradeRepository;

            RuleFor(sg => new { sg.SubjectId, sg.GradeId })
                .MustAsync(async (ids, ct) =>
                {
                    var existing = await _subjectGradeRepository.GetRelationAsync(ids.SubjectId, ids.GradeId);
                    return existing == null;
                }).WithMessage("Bu ders-sınıf ilişkisi zaten mevcut");
        }
    }
}
```

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Validators/
git commit -m "feat: add SubjectValidator, GradeValidator, SubjectGradeValidator"
```

---

### Task 25: Register FluentValidation in Program.cs

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Program.cs`

- [ ] **Step 1: Add FluentValidation services**

In Program.cs, add before `var app = builder.Build();`:
```csharp
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

- [ ] **Step 2: Build to verify**

Run:
```bash
dotnet build
```

Expected: BUILD SUCCESS

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Program.cs
git commit -m "feat: register FluentValidation in Program.cs"
```

---

## Chunk 5: Service Layer

> Bu chunk service layer'ı günceller.

### Task 26: Update UnitService

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

- [ ] **Step 1: Read current UnitService**

Run:
```bash
cat JelleSmart.ExamSystem.Service/Services/UnitService.cs
```

- [ ] **Step 2: Update UnitService for GradeId and Order**

Update methods to use GradeId instead of SubjectId:
- Create: Use GradeId, auto-assign Order
- Update: Validate GradeId and Order uniqueness

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/UnitService.cs
git commit -m "refactor: update UnitService for Grade-based hierarchy"
```

---

### Task 27: Update TopicService

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/TopicService.cs`

- [ ] **Step 1: Read current TopicService**

Run:
```bash
cat JelleSmart.ExamSystem.Service/Services/TopicService.cs
```

- [ ] **Step 2: Update TopicService for GradeId and Order**

Update methods to include GradeId validation and Order auto-assignment

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/TopicService.cs
git commit -m "refactor: update TopicService for GradeId and Order"
```

---

### Task 28: Update QuestionService for Exam Ordering

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/ExamService.cs`

- [ ] **Step 1: Read current ExamService**

Run:
```bash
cat JelleSmart.ExamSystem.Service/Services/ExamService.cs
```

- [ ] **Step 2: Add OrderQuestionsByTopicSequence logic**

Find the method that gets questions for an exam and add:
```csharp
if (exam.OrderQuestionsByTopicSequence)
{
    questions = questions
        .Include(q => q.Topic)
            .ThenInclude(t => t.Unit)
        .OrderBy(q => q.Topic!.Unit!.Order)
        .ThenBy(q => q.Topic!.Order)
        .ToList();
}
else
{
    questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/ExamService.cs
git commit -m "feat: add OrderQuestionsByTopicSequence to ExamService"
```

---

## Chunk 6: API Controllers

> Bu chunk cascading dropdown API endpoint'lerini ekler.

### Task 29: Add Cascading Endpoints to GradeController

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/GradeController.cs`

- [ ] **Step 1: Read current GradeController**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Controllers/GradeController.cs
```

- [ ] **Step 2: Add BySubject endpoint**

Add:
```csharp
[HttpPost("BySubject/{subjectId}")]
public async Task<IActionResult> GetBySubject(string subjectId)
{
    var grades = await _subjectGradeRepository.GetGradesBySubjectIdAsync(subjectId);
    var result = grades.Select(g => new { id = g.Id, name = g.Name });
    return Json(result);
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/GradeController.cs
git commit -m "feat: add BySubject cascading endpoint to GradeController"
```

---

### Task 30: Add ByGrade Endpoint to UnitController

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Read current UnitController**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
```

- [ ] **Step 2: Add ByGrade endpoint**

Add:
```csharp
[HttpPost("ByGrade/{gradeId}")]
public async Task<IActionResult> GetByGrade(string gradeId)
{
    var units = await _unitRepository.GetByGradeIdAsync(gradeId);
    var result = units.Select(u => new { id = u.Id, name = $"{u.Order}. {u.Name}" });
    return Json(result);
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "feat: add ByGrade cascading endpoint to UnitController"
```

---

### Task 31: Add ByUnit Endpoint to TopicController

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Read current TopicController**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
```

- [ ] **Step 2: Add ByUnit endpoint**

Add:
```csharp
[HttpPost("ByUnit/{unitId}")]
public async Task<IActionResult> GetByUnit(string unitId)
{
    var topics = await _topicRepository.GetByUnitIdAsync(unitId);
    var result = topics.Select(t => new
    {
        id = t.Id,
        name = string.IsNullOrEmpty(t.Code) ? $"{t.Order}. {t.Name}" : $"{t.Code} - {t.Name}"
    });
    return Json(result);
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "feat: add ByUnit cascading endpoint to TopicController"
```

---

### Task 32: Update Teacher QuestionController

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/Teacher/QuestionController.cs`

- [ ] **Step 1: Read current QuestionController**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Controllers/Teacher/QuestionController.cs
```

- [ ] **Step 2: Update Create/Update methods to only use TopicId**

Remove SubjectId, UnitId, GradeId handling. Just validate TopicId and extract from Topic.

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/Teacher/QuestionController.cs
git commit -m "refactor: update QuestionController to use only TopicId"
```

---

## Chunk 7: UI - HierarchySelect.js

> Bu chunk yeni JavaScript modülünü ekler.

### Task 33: Create HierarchySelect.js

**Files:**
- Create: `JelleSmart.ExamSystem.WebUI/wwwroot/js/hierarchy-select.js`

- [ ] **Step 1: Create hierarchy-select.js**

Create file with full Select2 cascading logic:
```javascript
(function() {
    'use strict';

    window.HierarchySelect = {
        options: {},

        init: function(options) {
            this.options = $.extend({}, {
                subjectSelect: null,
                gradeSelect: null,
                unitSelect: null,
                topicSelect: null,
                endPoint: 'unit' // 'unit', 'topic', or 'question'
            }, options);

            this._initSelect2();
            this._bindEvents();
        },

        _initSelect2: function() {
            var select2Config = {
                theme: 'bootstrap-5',
                width: '100%',
                language: 'tr',
                dropdownParent: $(this.options.subjectSelect).closest('.card-body').length ?
                    $(this.options.subjectSelect).closest('.card-body').first() :
                    $('body')
            };

            $(this.options.subjectSelect).select2(select2Config);
            $(this.options.gradeSelect).select2(select2Config);

            if (this.options.unitSelect) {
                $(this.options.unitSelect).select2(select2Config);
            }
            if (this.options.topicSelect) {
                $(this.options.topicSelect).select2(select2Config);
            }
        },

        _bindEvents: function() {
            var self = this;

            $(this.options.subjectSelect).on('change', function() {
                var subjectId = $(this).val();
                self._loadGrades(subjectId);
                self._clearDownstream('subject');
            });

            $(this.options.gradeSelect).on('change', function() {
                var gradeId = $(this).val();
                self._loadUnits(gradeId);
                self._clearDownstream('grade');
            });

            if (this.options.topicSelect) {
                $(this.options.unitSelect).on('change', function() {
                    var unitId = $(this).val();
                    self._loadTopics(unitId);
                });
            }
        },

        _loadGrades: function(subjectId) {
            var self = this;
            var $gradeSelect = $(this.options.gradeSelect);

            if (!subjectId) {
                $gradeSelect.empty().append('<option value="">Seçiniz...</option>');
                $gradeSelect.prop('disabled', true);
                return;
            }

            $.ajax({
                url: '/Grade/BySubject/' + subjectId,
                method: 'POST',
                success: function(data) {
                    $gradeSelect.empty().append('<option value="">Seçiniz...</option>');
                    $.each(data, function(i, item) {
                        $gradeSelect.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                    $gradeSelect.prop('disabled', false);
                },
                error: function() {
                    toastr.error('Sınıflar yüklenirken hata oluştu');
                }
            });
        },

        _loadUnits: function(gradeId) {
            var self = this;
            var $unitSelect = $(this.options.unitSelect);

            if (!gradeId) {
                $unitSelect.empty().append('<option value="">Seçiniz...</option>');
                $unitSelect.prop('disabled', true);
                return;
            }

            $.ajax({
                url: '/Unit/ByGrade/' + gradeId,
                method: 'POST',
                success: function(data) {
                    $unitSelect.empty().append('<option value="">Seçiniz...</option>');
                    $.each(data, function(i, item) {
                        $unitSelect.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                    $unitSelect.prop('disabled', false);
                },
                error: function() {
                    toastr.error('Üniteler yüklenirken hata oluştu');
                }
            });
        },

        _loadTopics: function(unitId) {
            var self = this;
            var $topicSelect = $(this.options.topicSelect);

            if (!unitId) {
                $topicSelect.empty().append('<option value="">Seçiniz...</option>');
                return;
            }

            $.ajax({
                url: '/Topic/ByUnit/' + unitId,
                method: 'POST',
                success: function(data) {
                    $topicSelect.empty().append('<option value="">Seçiniz...</option>');
                    $.each(data, function(i, item) {
                        $topicSelect.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                },
                error: function() {
                    toastr.error('Konular yüklenirken hata oluştu');
                }
            });
        },

        _clearDownstream: function(level) {
            if (level === 'subject') {
                $(this.options.gradeSelect).val(null).trigger('change');
                if (this.options.unitSelect) {
                    $(this.options.unitSelect).empty().append('<option value="">Seçiniz...</option>').prop('disabled', true);
                }
                if (this.options.topicSelect) {
                    $(this.options.topicSelect).empty().append('<option value="">Seçiniz...</option>');
                }
            } else if (level === 'grade') {
                if (this.options.unitSelect) {
                    $(this.options.unitSelect).val(null).trigger('change');
                }
                if (this.options.topicSelect) {
                    $(this.options.topicSelect).empty().append('<option value="">Seçiniz...</option>');
                }
            }
        },

        destroy: function() {
            $(this.options.subjectSelect).select2('destroy');
            $(this.options.gradeSelect).select2('destroy');
            if (this.options.unitSelect) {
                $(this.options.unitSelect).select2('destroy');
            }
            if (this.options.topicSelect) {
                $(this.options.topicSelect).select2('destroy');
            }
        }
    };
})();
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/wwwroot/js/hierarchy-select.js
git commit -m "feat: add HierarchySelect.js with Select2 cascading dropdowns"
```

---

## Chunk 8: Views - Unit Forms

> Bu chunk Unit Create/Edit views'larını günceller.

### Task 34: Update Unit Create View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Admin/Unit/Create.cshtml`

- [ ] **Step 1: Replace Unit Create view**

Replace content:
```cshtml
@model JelleSmart.ExamSystem.Core.ViewModels.UnitViewModel

@{
    ViewData["Title"] = "Yeni Ünite";
    var subjects = ViewBag.Subjects as IEnumerable<JelleSmart.ExamSystem.Core.ViewModels.SubjectViewModel>;
}

<div class="d-flex flex-column-fluid">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12">
                <div class="card card-xl-stretch mb-5 mb-xl-8">
                    <div class="card-header border-0 pt-5">
                        <h3 class="card-title align-items-start flex-column">
                            <span class="card-label fw-bold fs-3 mb-1">Yeni Ünite Ekle</span>
                            <span class="text-muted fw-semibold fs-7">Sisteme yeni ünite ekleyin</span>
                        </h3>
                    </div>
                    <div class="card-body py-3">
                        @if (subjects == null || !subjects.Any())
                        {
                            <div class="alert alert-warning d-flex align-items-center p-5 mb-5">
                                <i class="ki-outline ki-information-5 fs-1 me-4"></i>
                                <div class="d-flex flex-column">
                                    <h4 class="fw-bold mb-1">Ders Bulunamadı</h4>
                                    <span class="fs-6">Ünite ekleyebilmek için önce ders oluşturmanız gerekmektedir.</span>
                                </div>
                            </div>
                        }

                        <form asp-action="Create" method="post" id="unitForm">
                            <div asp-validation-summary="ModelOnly" class="alert alert-danger d-none" role="alert"></div>

                            <div class="row mb-5">
                                <div class="col-md-6">
                                    <label asp-for="SubjectId" class="form-label required">Ders</label>
                                    <select asp-for="SubjectId" class="form-select form-select-solid" id="subjectSelect">
                                        <option value="">Seçiniz...</option>
                                        @if (subjects != null)
                                        {
                                            @foreach (var subject in subjects)
                                            {
                                                <option value="@subject.Id">@subject.Name</option>
                                            }
                                        }
                                    </select>
                                    <span asp-validation-for="SubjectId" class="text-danger fs-7"></span>
                                </div>

                                <div class="col-md-6">
                                    <label asp-for="GradeId" class="form-label required">Sınıf</label>
                                    <select asp-for="GradeId" class="form-select form-select-solid" id="gradeSelect" disabled>
                                        <option value="">Önce Ders Seçiniz</option>
                                    </select>
                                    <span asp-validation-for="GradeId" class="text-danger fs-7"></span>
                                </div>
                            </div>

                            <div class="row mb-5">
                                <div class="col-md-3">
                                    <label asp-for="Order" class="form-label required">Sıra No</label>
                                    <input asp-for="Order" type="number" class="form-control form-control-solid" min="1" value="1" />
                                    <span asp-validation-for="Order" class="text-danger fs-7"></span>
                                </div>

                                <div class="col-md-9">
                                    <label asp-for="Name" class="form-label required">Ünite Adı</label>
                                    <input asp-for="Name" class="form-control form-control-solid" placeholder="Ünite adını girin" />
                                    <span asp-validation-for="Name" class="text-danger fs-7"></span>
                                </div>
                            </div>

                            <div class="mb-5">
                                <label asp-for="Description" class="form-label">Açıklama</label>
                                <textarea asp-for="Description" class="form-control form-control-solid" rows="3" placeholder="Ünite açıklamasını girin"></textarea>
                                <span asp-validation-for="Description" class="text-danger fs-7"></span>
                            </div>

                            <div class="d-flex justify-content-end">
                                <a asp-action="Index" class="btn btn-light me-3">İptal</a>
                                <button type="submit" class="btn btn-primary">
                                    <span class="indicator-label">Kaydet</span>
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
    <script src="~/js/hierarchy-select.js"></script>
    <script>
        $(document).ready(function() {
            HierarchySelect.init({
                subjectSelect: '#subjectSelect',
                gradeSelect: '#gradeSelect',
                unitSelect: null,
                endPoint: 'unit'
            });
        });
    </script>
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Admin/Unit/Create.cshtml
git commit -m "refactor: update Unit Create view with cascading dropdowns"
```

---

### Task 35: Update Unit Edit View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Admin/Unit/Edit.cshtml`

- [ ] **Step 1: Read current Edit view**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Views/Admin/Unit/Edit.cshtml
```

- [ ] **Step 2: Update Edit view similar to Create**

Apply similar changes as Create view, but populate with existing values.

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Admin/Unit/Edit.cshtml
git commit -m "refactor: update Unit Edit view with cascading dropdowns"
```

---

## Chunk 9: Views - Topic Forms

> Bu chunk Topic Create/Edit views'larını günceller.

### Task 36: Update Topic Create View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Admin/Topic/Create.cshtml`

- [ ] **Step 1: Replace Topic Create view**

Replace content with cascading dropdowns (Subject → Grade → Unit) and Order field.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Admin/Topic/Create.cshtml
git commit -m "refactor: update Topic Create view with cascading dropdowns"
```

---

### Task 37: Update Topic Edit View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Admin/Topic/Edit.cshtml`

- [ ] **Step 1: Update Edit view**

Apply similar changes with existing values.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Admin/Topic/Edit.cshtml
git commit -m "refactor: update Topic Edit view with cascading dropdowns"
```

---

## Chunk 10: Views - Question Forms

> Bu chunk Question Create/Edit views'larını günceller.

### Task 38: Update Question Create View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Teacher/Question/Create.cshtml`

- [ ] **Step 1: Replace Question Create view**

Replace with cascading dropdowns (Subject → Grade → Unit → Topic) and remove SubjectId/UnitId/GradeId hidden fields.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Teacher/Question/Create.cshtml
git commit -m "refactor: update Question Create view with full cascading dropdowns"
```

---

### Task 39: Update Question Edit View

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Teacher/Question/Edit.cshtml`

- [ ] **Step 1: Update Edit view**

Apply similar changes with existing values.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Teacher/Question/Edit.cshtml
git commit -m "refactor: update Question Edit view with full cascading dropdowns"
```

---

## Chunk 11: Exam View Update

> Bu chunk Exam Create view'ına OrderQuestionsByTopicSequence seçeneğini ekler.

### Task 40: Add OrderQuestionsByTopicSequence to Exam Create

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Teacher/Exam/Create.cshtml`

- [ ] **Step 1: Read current Exam Create view**

Run:
```bash
cat JelleSmart.ExamSystem.WebUI/Views/Teacher/Exam/Create.cshtml
```

- [ ] **Step 2: Add checkbox for question ordering**

Add:
```cshtml
<div class="form-check form-switch mb-3">
    <input class="form-check-input" type="checkbox" asp-for="OrderQuestionsByTopicSequence">
    <label class="form-check-label" asp-for="OrderQuestionsByTopicSequence">
        Soruları ünite ve konu sırasına göre getir
    </label>
    <span class="text-muted fs-7 d-block">İşaretlenirse sorular ünite ve konu sırasına göre gelir. İşaretlenmezse sorular karışık gelir.</span>
</div>
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Teacher/Exam/Create.cshtml
git commit -m "feat: add OrderQuestionsByTopicSequence option to Exam Create"
```

---

## Chunk 12: Controllers Update for Forms

> Bu chunk controller action'larını yeni ViewModels ile günceller.

### Task 41: Update UnitController Actions

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Update Create GET action**

Populate Subjects ViewBag, initialize Order to next available.

- [ ] **Step 2: Update Create POST action**

Use UnitValidator, handle GradeId from Subject selection.

- [ ] **Step 3: Update Edit GET/POST actions**

Similar updates.

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "refactor: update UnitController for new hierarchy"
```

---

### Task 42: Update TopicController Actions

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Update Create/Edit actions**

Handle Subject → Grade → Unit cascade, populate appropriate ViewBags.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "refactor: update TopicController for new hierarchy"
```

---

### Task 43: Update QuestionController Actions

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/Teacher/QuestionController.cs`

- [ ] **Step 1: Update Create/Edit actions**

Only use TopicId, populate cascade helpers for UI.

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/Teacher/QuestionController.cs
git commit -m "refactor: update QuestionController for Topic-only hierarchy"
```

---

## Chunk 13: Final Testing and Documentation

> Bu chunk test senaryolarını ve documentation'ı tamamlar.

### Task 44: Create Seed Data

**Files:**
- Create: `JelleSmart.ExamSystem.Repository/Data/SeedData.cs`

- [ ] **Step 1: Create seed data**

Create hierarchical seed data:
- 1 Subject (Matematik)
- 2 Grades (1. Sınıf, 2. Sınıf)
- SubjectGrade relations
- 2 Units per Grade
- 2 Topics per Unit
- Sample Questions

- [ ] **Step 2: Update Program.cs to seed data**

Add seed data call in Development mode.

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Repository/Data/SeedData.cs
git add JelleSmart.ExamSystem.WebUI/Program.cs
git commit -m "feat: add hierarchical seed data for testing"
```

---

### Task 45: Run Integration Tests

**Files:**
- None (manual testing)

- [ ] **Step 1: Test cascading dropdowns**

Manual test:
1. Navigate to Unit/Create
2. Select a Subject → Grades should load
3. Select a Grade → Units should be available for next selection
4. Verify Select2 styling

- [ ] **Step 2: Test unique constraints**

1. Try to create duplicate Subject name → Should fail
2. Try to create duplicate Grade Level → Should fail
3. Try to create duplicate Subject-Grade relation → Should fail
4. Try to create duplicate Unit Order in same Grade → Should fail
5. Try to create duplicate Topic Order in same Unit → Should fail

- [ ] **Step 3: Test question ordering**

1. Create exam with OrderQuestionsByTopicSequence=true
2. Take exam → Questions should be ordered by Unit.Order → Topic.Order
3. Create exam with OrderQuestionsByTopicSequence=false
4. Take exam → Questions should be random

- [ ] **Step 4: Document any issues found**

---

### Task 46: Update Documentation

**Files:**
- Modify: `README.md` or create `docs/hierarchy.md`

- [ ] **Step 1: Document new hierarchy**

Explain the new Subject → Grade → Unit → Topic → Question hierarchy.

- [ ] **Step 2: Document API endpoints**

List cascading endpoints: /Grade/BySubject/{id}, /Unit/ByGrade/{id}, /Topic/ByUnit/{id}

- [ ] **Step 3: Commit**

```bash
git add README.md docs/hierarchy.md
git commit -m "docs: document new hierarchical structure"
```

---

### Task 47: Final Commit and Push

**Files:**
- None (final)

- [ ] **Step 1: Review all changes**

Run:
```bash
git status
git log --oneline -20
```

- [ ] **Step 2: Push to remote**

Run:
```bash
git push
```

Expected: All commits pushed to origin/main

---

## Summary

After completing this implementation plan:
1. ✅ Clean hierarchy: Subject → Grade → Unit → Topic → Question
2. ✅ Unique constraints enforced via EF Core and FluentValidation
3. ✅ Cascading dropdowns with Select2
4. ✅ Order fields on Unit and Topic
5. ✅ Questions linked only to Topic
6. ✅ Exam question ordering option
7. ✅ All forms updated with new hierarchy

**Estimated completion time:** 4-6 hours for a skilled developer
