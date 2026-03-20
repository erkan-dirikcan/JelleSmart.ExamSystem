# Hiyerarşik Ünite-Konu-Soru Düzeltmesi - Tasarım Belgesi

## Tarih
2026-03-20

## Problem
Mevcut sistemde hiyerarşi eksik ve tutarsız:
- **Unit (Ünite)**: Sadece Subject (Ders) bağlı, Grade (Sınıf) bağı yok
- **Topic (Konu)**: Sadece Unit bağlı, Subject/Grade contexti formda görünmüyor
- **Question (Soru)**: Subject ve Grade seçimleri birbirinden bağımsız, hiyerarşik değil
- **Select Listler**: Select2 kullanılmıyor, basic HTML select

## Hedef
Temiz hiyerarşik yapı: **Ders → Sınıf → Ünite → Konu → Sorular**

## Çözüm Yaklaşımı
**Tam Hiyerarşik Refactor** - Test verisi olduğu için database silinip baştan oluşturulabilir.

---

## 1. Veri Modeli

### Yeni Entity: SubjectGrade (Many-to-Many)
```csharp
public class SubjectGrade : BaseEntity
{
    public string SubjectId { get; set; }
    public string GradeId { get; set; }

    public Subject Subject { get; set; }
    public Grade Grade { get; set; }
}
```

### Güncellenen Entity'ler

#### Subject
```csharp
public class Subject : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? IconClass { get; set; }

    public ICollection<SubjectGrade> SubjectGrades { get; set; }
    public ICollection<Unit> Units { get; set; }
}
```

#### Grade
```csharp
public class Grade : BaseEntity
{
    public int Level { get; set; }  // 1, 2, 3, 4
    public string Name { get; set; }

    public ICollection<SubjectGrade> SubjectGrades { get; set; }
    public ICollection<Unit> Units { get; set; }
}
```

#### Unit
```csharp
public class Unit : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }  // YENİ: Sıra numarası

    public string GradeId { get; set; }  // REQUIRED (SubjectId kaldırıldı)

    public Grade Grade { get; set; }
    public ICollection<Topic> Topics { get; set; }
}
```

#### Topic
```csharp
public class Topic : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public int Order { get; set; }  // YENİ: Sıra numarası

    public string UnitId { get; set; }     // REQUIRED
    public string GradeId { get; set; }    // YENİ: REQUIRED

    public Unit Unit { get; set; }
    public Grade Grade { get; set; }
    public ICollection<Question> Questions { get; set; }
}
```

#### Question
```csharp
public class Question : BaseEntity
{
    public string Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? Explanation { get; set; }
    public int Difficulty { get; set; } = 1;

    public string TopicId { get; set; }  // REQUIRED (SubjectId, UnitId, GradeId kaldırıldı)

    public Topic Topic { get; set; }
    public string CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; }
    public ICollection<Choice> Choices { get; set; }
    public ICollection<ExamQuestion> ExamQuestions { get; set; }
}
```

#### Exam (Güncellenme)
```csharp
public class Exam : BaseEntity
{
    // ... mevcut field'lar ...

    public bool OrderQuestionsByTopicSequence { get; set; } = false;  // YENİ
}
```

---

## 2. Unique Constraints (EF Core)

```csharp
// Subject - Name (case-insensitive)
builder.HasIndex(s => s.Name)
    .IsUnique()
    .HasFilter("LOWER(Name) = LOWER(Name)");

// Grade - Level
builder.HasIndex(g => g.Level)
    .IsUnique();

// SubjectGrade - Composite
builder.HasIndex(sg => new { sg.SubjectId, sg.GradeId })
    .IsUnique();

// Unit - (GradeId, Order)
builder.HasIndex(u => new { u.GradeId, u.Order })
    .IsUnique();

// Topic - (UnitId, Order)
builder.HasIndex(t => new { t.UnitId, t.Order })
    .IsUnique();
```

---

## 3. API Endpoints (Cascading Dropdowns)

### GradeController
```
POST /Grade/BySubject/{subjectId}
→ SubjectGrade tablosundan Grade'ları getir
```

### UnitController
```
POST /Unit/ByGrade/{gradeId}
→ Unit'leri Order ile sıralı getir
→ Format: "{Order}. {Name}"
```

### TopicController
```
POST /Topic/ByUnit/{unitId}
→ Topic'leri Order ile sıralı getir
→ Format: "{Code} - {Name}" veya "{Order}. {Name}"
```

### QuestionController
```
POST /Question/ByTopic/{topicId}
→ Topic'e ait soruları getir
```

---

## 4. Sınav Soru Sıralama Mantığı

```csharp
// ExamService
if (exam.OrderQuestionsByTopicSequence)
{
    questions = questions
        .Include(q => q.Topic)
            .ThenInclude(t => t.Unit)
        .OrderBy(q => q.Topic.Unit.Order)
        .ThenBy(q => q.Topic.Order)
        .ToList();
}
else
{
    questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
}
```

**Şıklar her sınavda karışık gelir** - doğru cevap A iken başka sınavda C olabilir.

---

## 5. ViewModels

### UnitViewModel
```csharp
public class UnitViewModel
{
    public string? Id { get; set; }
    [Required] public string Name { get; set; }
    public string? Description { get; set; }
    [Required] public string GradeId { get; set; }  // YENİ: Required
    [Required] public int Order { get; set; }       // YENİ
    public string? SubjectId { get; set; }          // Form için (hidden)
}
```

### TopicViewModel
```csharp
public class TopicViewModel
{
    public string? Id { get; set; }
    [Required] public string Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    [Required] public string UnitId { get; set; }
    [Required] public string GradeId { get; set; }  // YENİ: Required
    [Required] public int Order { get; set; }       // YENİ
    public string? SubjectId { get; set; }          // Form için (hidden)
}
```

### QuestionViewModel
```csharp
public class QuestionViewModel
{
    public string? Id { get; set; }
    [Required] public string Text { get; set; }
    public string? ImageUrl { get; set; }
    public int Difficulty { get; set; } = 1;
    [Required] public string TopicId { get; set; }  // Sadece bu kaldı

    // Form cascade için helper field'lar (submit edilmez)
    public string? SubjectId { get; set; }   // UI için
    public string? GradeId { get; set; }     // UI için
    public string? UnitId { get; set; }      // UI için

    public List<ChoiceViewModel> Choices { get; set; }
}
```

---

## 6. Validation (FluentValidation)

### SubjectValidator
- Name not empty, max 200 chars
- Name unique (case-insensitive)

### GradeValidator
- Level between 1-12
- Level unique

### SubjectGradeValidator
- (SubjectId, GradeId) composite unique

### UnitValidator
- Name not empty, max 200 chars
- Order > 0
- (GradeId, Order) unique
- GradeId required

### TopicValidator
- Name not empty, max 200 chars
- Order > 0
- (UnitId, Order) unique
- UnitId required
- GradeId required

---

## 7. UI - HierarchySelect.js

Yeni JavaScript modülü: `wwwroot/js/hierarchy-select.js`

Select2 ile cascading dropdown:
- Subject değişince → Grade'lar yüklenir
- Grade değişince → Unit'ler yüklenir
- Unit değişince → Topic'ler yüklenir (sadece Question formu)

Tüm select listeler Select2 kullanır.

---

## 8. Forms

### Unit Create/Edit
- Subject select (Select2)
- Grade select (Select2, Subject'den cascade)
- Order input (number)
- Name input
- Description textarea

### Topic Create/Edit
- Subject select (Select2, disabled - cascade için)
- Grade select (Select2, Subject'den cascade)
- Unit select (Select2, Grade'den cascade, "{Order}. {Name}" format)
- Order input (number)
- Name input
- Code input
- Description textarea

### Question Create/Edit
- Subject select (Select2)
- Grade select (Select2, Subject'den cascade)
- Unit select (Select2, Grade'den cascade)
- Topic select (Select2, Unit'den cascade, "{Code} - {Name}" format)
- Text textarea
- Image file input
- Difficulty select
- Choices (A, B, C, D, E)

---

## 9. Database Migration

### Adımlar
1. Mevcut database'i sil (test verisi olduğu için)
2. Tüm migration'ları sil
3. Yeni initial migration oluştur
4. Update-Database

### Silinecek Entity'ler/Field'lar
- `Unit.SubjectId`
- `Question.SubjectId`
- `Question.UnitId`
- `Question.GradeId`

### Yeni Entity'ler/Field'lar
- `SubjectGrade` (yeni tablo)
- `Unit.GradeId` (required)
- `Unit.Order`
- `Topic.GradeId` (required)
- `Topic.Order`
- `Exam.OrderQuestionsByTopicSequence`

---

## 10. Kullanıcı Rolleri

### Admin
- Subject, Grade, SubjectGrade CRUD
- Unit CRUD (tüm ders/sınıf için)
- Topic CRUD (tüm ünite için)

### Teacher
- Unit CRUD (sadece atanmış dersler için)
- Topic CRUD (sadece atanmış derslerin üniteleri için)
- Question CRUD (sadece atanmış dersler için)

---

## Test Senaryoları

1. **Ders Ekleme**: Aynı isimde ders eklenememeli
2. **Sınıf Ekleme**: Aynı Level'da sınıf eklenememeli
3. **Ders-Sınıf İlişkisi**: Aynı ilişki tekrar eklenememeli
4. **Ünite Ekleme**: Aynı sınıfta aynı Order numarası eklenememeli
5. **Konu Ekleme**: Aynı ünitede aynı Order numarası eklenememeli
6. **Soru Ekleme**: Konu seçilmeden soru eklenememeli
7. **Cascading**: Subject seçilince Grade'lar gelmeli
8. **Sınav**: OrderQuestionsByTopicSequence=true ise sıralı gelmeli

---

## Dosya Listesi (Değişecek/Eklenecek)

### Entity'ler
- `Core/Entities/SubjectGrade.cs` (YENİ)
- `Core/Entities/Subject.cs` (GÜNCELLE)
- `Core/Entities/Grade.cs` (GÜNCELLE)
- `Core/Entities/Unit.cs` (GÜNCELLE)
- `Core/Entities/Topic.cs` (GÜNCELLE)
- `Core/Entities/Question.cs` (GÜNCELLE)
- `Core/Entities/Exam.cs` (GÜNCELLE)

### Repository'ler
- `Core/Interfaces/Repositories/ISubjectGradeRepository.cs` (YENİ)
- `Repository/Repositories/SubjectGradeRepository.cs` (YENİ)
- `Repository/Repositories/UnitRepository.cs` (GÜNCELLE)
- `Repository/Repositories/TopicRepository.cs` (GÜNCELLE)
- `Repository/Repositories/QuestionRepository.cs` (GÜNCELLE)

### Service'ler
- `Service/Services/UnitService.cs` (GÜNCELLE)
- `Service/Services/TopicService.cs` (GÜNCELLE)
- `Service/Services/QuestionService.cs` (GÜNCELLE)

### Controllers
- `Controllers/GradeController.cs` (GÜNCELLE - BySubject endpoint)
- `Controllers/UnitController.cs` (GÜNCELLE - ByGrade endpoint)
- `Controllers/TopicController.cs` (GÜNCELLE - ByUnit endpoint)
- `Controllers/Teacher/QuestionController.cs` (GÜNCELLE)

### ViewModels
- `Core/ViewModels/UnitViewModel.cs` (GÜNCELLE)
- `Core/ViewModels/TopicViewModel.cs` (GÜNCELLE)
- `Core/ViewModels/QuestionViewModel.cs` (GÜNCELLE)
- `Core/ViewModels/SubjectGradeViewModel.cs` (YENİ)

### Views
- `Views/Admin/Unit/Create.cshtml` (GÜNCELLE)
- `Views/Admin/Unit/Edit.cshtml` (GÜNCELLE)
- `Views/Admin/Topic/Create.cshtml` (GÜNCELLE)
- `Views/Admin/Topic/Edit.cshtml` (GÜNCELLE)
- `Views/Teacher/Question/Create.cshtml` (GÜNCELLE)
- `Views/Teacher/Question/Edit.cshtml` (GÜNCELLE)

### JavaScript
- `wwwroot/js/hierarchy-select.js` (YENİ)

### Configurations
- `Repository/Configurations/SubjectGradeConfiguration.cs` (YENİ)
- `Repository/Configurations/SubjectConfiguration.cs` (GÜNCELLE - unique constraint)
- `Repository/Configurations/GradeConfiguration.cs` (GÜNCELLE - unique constraint)
- `Repository/Configurations/UnitConfiguration.cs` (GÜNCELLE - unique constraint)
- `Repository/Configurations/TopicConfiguration.cs` (GÜNCELLE - unique constraint)

### Validators
- `Validators/SubjectValidator.cs` (YENİ)
- `Validators/GradeValidator.cs` (YENİ)
- `Validators/SubjectGradeValidator.cs` (YENİ)
- `Validators/UnitValidator.cs` (YENİ)
- `Validators/TopicValidator.cs` (YENİ)

---

## Sonraki Adım
Bu tasarım onaylandıktan sonra implementation planı (superpowers:writing-plans) oluşturulacak.
