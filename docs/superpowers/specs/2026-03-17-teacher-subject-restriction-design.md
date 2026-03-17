# Öğretmen Branş Kısıtlaması Tasarımı

## Tarih
2026-03-17

## Amaç
Öğretmenlerin sadece atanmış oldukları dersler (Subject) için Unit ve Topic yönetebilmesini sağlamak. Admin tüm dersleri görmeye devam edecek.

## Mevcut Durum

### Yetkilendirme Yapısı
```csharp
// UnitController - Sadece Admin
[Authorize(Roles = UserRoles.Admin)]
public class UnitController : Controller

// TopicController - Sadece Admin
[Authorize(Roles = UserRoles.Admin)]
public class TopicController : Controller
```

### Öğretmen-Ders İlişkisi
```csharp
// TeacherProfile - Birden fazla ders
public class TeacherProfile : BaseEntity
{
    public string UserId { get; set; }
    public ICollection<TeacherSubject> Subjects { get; set; }
}

// TeacherSubject - Junction table
public class TeacherSubject
{
    public string? TeacherProfileId { get; set; }
    public string? SubjectId { get; set; }
}
```

## Hedeflenen Yapı

### Yetkilendirme
- Admin: Tüm dersleri görebilir ve yönetebilir (mevcut davranış)
- Teacher: Sadece atanmış olduğu dersleri görebilir ve yönetebilir

### Filtreleme Yeri
Service katmanında filtreleme yapılacak - repository değişikliği minimal olacak.

## Değişiklik Listesi

### 1. IUnitService Arayüzü Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/IUnitService.cs`

Teacher için yeni metodlar:
```csharp
// Admin için - mevcut metod
Task<IEnumerable<UnitViewModel>> GetAllViewModelAsync();

// Teacher için - yeni
Task<IEnumerable<UnitViewModel>> GetByTeacherSubjectsAsync(string userId);
Task<IEnumerable<UnitViewModel>> GetBySubjectIdAsync(string subjectId);
```

### 2. UnitService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

Teacher filtreleme implementasyonu:
```csharp
public async Task<IEnumerable<UnitViewModel>> GetByTeacherSubjectsAsync(string userId)
{
    // 1. Teacher'ın derslerini al
    var teacherProfile = await _teacherProfileService.GetWithSubjectsAsync(userId);
    if (teacherProfile == null || !teacherProfile.Subjects.Any())
        return Enumerable.Empty<UnitViewModel>();

    var subjectIds = teacherProfile.Subjects.Select(ts => ts.SubjectId).ToList();

    // 2. Tüm üniteleri getir
    var allUnits = await _unitRepository.GetAllWithIncludesAsync();

    // 3. Teacher'ın derslerine ait üniteleri filtrele
    return allUnits
        .Where(u => subjectIds.Contains(u.SubjectId))
        .Select(e => new UnitViewModel
        {
            Id = e.Id,
            Name = e.Name,
            SubjectId = e.SubjectId,
            Description = e.Description,
            SubjectName = e.Subject?.Name
        }).ToList();
}
```

### 3. UnitController Güncelleme

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

```csharp
// Admin OR Teacher
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
public class UnitController : Controller
{
    private readonly IUnitService _unitService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherProfileService _teacherProfileService;

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = ManageNavPages.Units;
        ViewData["Title"] = "Üniteler";
        ViewData["PageDescription"] = "Sistem ünitelerini yönetin";

        IEnumerable<UnitViewModel> viewModels;

        // Admin ise tüm üniteler, Teacher ise sadece kendi derslerinin üniteleri
        if (User.IsInRole(UserRoles.Admin))
        {
            viewModels = await _unitService.GetAllViewModelAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            viewModels = await _unitService.GetByTeacherSubjectsAsync(userId!);
        }

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["ActivePage"] = ManageNavPages.Units;
        ViewData["Title"] = "Yeni Ünite";
        ViewData["PageDescription"] = "Yeni ünite ekleyin";

        IEnumerable<SubjectViewModel> subjects;

        if (User.IsInRole(UserRoles.Admin))
        {
            subjects = await _subjectService.GetAllViewModelAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            subjects = await _teacherProfileService.GetTeacherSubjectsAsync(userId!);
        }

        ViewBag.Subjects = subjects;
        return View();
    }
}
```

### 4. ITopicService Arayüzü Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/ITopicService.cs`

```csharp
// Teacher için - yeni
Task<IEnumerable<TopicViewModel>> GetByTeacherSubjectsAsync(string userId);
Task<IEnumerable<TopicViewModel>> GetBySubjectIdAsync(string subjectId);
```

### 5. TopicService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Service/Services/TopicService.cs`

```csharp
public async Task<IEnumerable<TopicViewModel>> GetByTeacherSubjectsAsync(string userId)
{
    // 1. Teacher'ın derslerini al
    var teacherProfile = await _teacherProfileService.GetWithSubjectsAsync(userId);
    if (teacherProfile == null || !teacherProfile.Subjects.Any())
        return Enumerable.Empty<TopicViewModel>();

    var subjectIds = teacherProfile.Subjects.Select(ts => ts.SubjectId).ToList();

    // 2. Tüm konuları getir
    var allTopics = await _topicRepository.GetAllWithIncludesAsync();

    // 3. Teacher'ın derslerine ait konuları filtrele
    return allTopics
        .Where(t => t.Unit != null && subjectIds.Contains(t.Unit.SubjectId))
        .Select(e => new TopicViewModel
        {
            Id = e.Id,
            Name = e.Name,
            UnitId = e.UnitId,
            Description = e.Description,
            UnitName = e.Unit?.Name
        }).ToList();
}

public async Task<IEnumerable<TopicViewModel>> GetBySubjectIdAsync(string subjectId)
{
    var allTopics = await _topicRepository.GetAllWithIncludesAsync();
    return allTopics
        .Where(t => t.Unit != null && t.Unit.SubjectId == subjectId)
        .Select(e => new TopicViewModel
        {
            Id = e.Id,
            Name = e.Name,
            UnitId = e.UnitId,
            Description = e.Description,
            UnitName = e.Unit?.Name
        }).ToList();
}
```

### 6. TopicController Güncelleme

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

```csharp
// Admin OR Teacher
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
public class TopicController : Controller
{
    private readonly ITopicService _topicService;
    private readonly IUnitService _unitService;
    private readonly ITeacherProfileService _teacherProfileService;

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = ManageNavPages.Topics;
        ViewData["Title"] = "Konular";
        ViewData["PageDescription"] = "Ünite konularını yönetin";

        IEnumerable<TopicViewModel> viewModels;

        if (User.IsInRole(UserRoles.Admin))
        {
            viewModels = await _topicService.GetAllViewModelAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            viewModels = await _topicService.GetByTeacherSubjectsAsync(userId!);
        }

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["ActivePage"] = ManageNavPages.Topics;
        ViewData["Title"] = "Yeni Konu";
        ViewData["PageDescription"] = "Yeni konu ekleyin";

        IEnumerable<UnitViewModel> units;

        if (User.IsInRole(UserRoles.Admin))
        {
            units = await _unitService.GetAllViewModelAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Teacher'ın derslerine ait üniteler
            var teacherUnits = await _unitService.GetByTeacherSubjectsAsync(userId!);
            units = teacherUnits;
        }

        ViewBag.Units = units;
        return View();
    }
}
```

### 7. ITeacherProfileService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/ITeacherProfileService.cs`

```csharp
Task<IEnumerable<SubjectViewModel>> GetTeacherSubjectsAsync(string userId);
```

### 8. TeacherProfileService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Service/Services/TeacherProfileService.cs`

```csharp
public async Task<IEnumerable<SubjectViewModel>> GetTeacherSubjectsAsync(string userId)
{
    var profile = await _teacherProfileRepository.GetWithSubjectsAsync(userId);
    if (profile == null || !profile.Subjects.Any())
        return Enumerable.Empty<SubjectViewModel>();

    return profile.Subjects
        .Select(ts => ts.Subject)
        .Where(s => s != null)
        .Select(s => new SubjectViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code
        }).ToList();
}
```

## Test Senaryoları

### Test 1: Matematik Öğretmeni Unit Yönetimi
```
1. Matematik öğretmeni olarak giriş yap
2. Unit Index sayfasına git
3. Sadece Matematik dersi üniteleri görünmeli
4. Fizik dersi üniteleri GÖRÜNMEMELİ
5. Yeni ünite ekle -> Sadece Matematik dropdown'ta olmalı
```

### Test 2: Admin Unit Yönetimi
```
1. Admin olarak giriş yap
2. Unit Index sayfasına git
3. Tüm derslerin üniteleri görünmeli (değişiklik yok)
4. Yeni ünite ekle -> Tüm dersler seçilebilir
```

### Test 3: Birden Fazla Dersli Öğretmen
```
1. Matematik + Fizik öğretmeni olarak giriş yap
2. Unit Index -> Hem Matematik hem Fizik üniteleri görünmeli
3. Yeni ünite ekle -> Her iki ders de seçilebilir
```

### Test 4: Topic Yönetimi
```
1. Öğretmen olarak giriş yap
2. Topic Index -> Sadece kendi derslerinin konuları görünmeli
3. Yeni konu ekle -> Sadece kendi derslerinin üniteleri seçilebilir
```

## Güvenlik

### Authorize Kontrolü
Controller seviyesinde `[Authorize]` kontrolü mevcut. Ek güvenlik önlemi gerekmiyor.

### Validasyon
Service katmanında teacher'ın ders IDs'i kontrol ediliyor, yanlış ID ile istek gönderse bile boş sonuç dönüyor.

## Riskler

1. **Performance:** Tüm üniteleri/konuları çekip filtreleme - Veri çok büyürse repository katmanında filtreleme yapılmalı
2. **TeacherProfile yoksa:** Boş liste dönülüyor, hata fırlatılmıyor
3. **Role name hardcoding:** UserRoles.Admin ve UserRoles.Teacher string olarak kullanılıyor

## Sonraki Adımlar

1. IUnitService ve UnitService güncelle
2. ITopicService ve TopicService güncelle
3. ITeacherProfileService ve TeacherProfileService güncelle
4. UnitController güncelle
5. TopicController güncelle
6. Test
7. Commit
