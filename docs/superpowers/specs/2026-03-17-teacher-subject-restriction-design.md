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
```

### 2. UnitService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

Constructor güncelleme - ITeacherProfileService ekle:
```csharp
private readonly IUnitRepository _unitRepository;
private readonly ITeacherProfileService _teacherProfileService;

public UnitService(IUnitRepository unitRepository, ITeacherProfileService teacherProfileService)
{
    _unitRepository = unitRepository;
    _teacherProfileService = teacherProfileService;
}
```

Teacher filtreleme implementasyonu:
```csharp
public async Task<IEnumerable<UnitViewModel>> GetByTeacherSubjectsAsync(string userId)
{
    // 1. Teacher'ın derslerini al
    var teacherProfile = await _teacherProfileService.GetWithSubjectsAsync(userId);
    if (teacherProfile == null || !teacherProfile.Subjects.Any())
        return Enumerable.Empty<UnitViewModel>();

    // 2. SubjectId'leri al (null kontrolü ile)
    var subjectIds = teacherProfile.Subjects
        .Where(ts => !string.IsNullOrEmpty(ts.SubjectId))
        .Select(ts => ts.SubjectId!)
        .ToList();

    // 3. Tüm üniteleri getir
    var allUnits = await _unitRepository.GetAllWithIncludesAsync();

    // 4. Teacher'ın derslerine ait üniteleri filtrele
    return allUnits
        .Where(u => !string.IsNullOrEmpty(u.SubjectId) && subjectIds.Contains(u.SubjectId))
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

Constructor güncelleme:
```csharp
private readonly IUnitService _unitService;
private readonly ISubjectService _subjectService;
private readonly ITeacherProfileService _teacherProfileService;  // YENİ

public UnitController(
    IUnitService unitService,
    ISubjectService subjectService,
    ITeacherProfileService teacherProfileService)  // YENİ
{
    _unitService = unitService;
    _subjectService = subjectService;
    _teacherProfileService = teacherProfileService;  // YENİ
}
```

Authorize güncelleme:
```csharp
// ÖNCE: [Authorize(Roles = UserRoles.Admin)]
// SONRA:
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
public class UnitController : Controller
```

Index action güncelleme:
```csharp
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
    else  // Teacher
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        viewModels = await _unitService.GetByTeacherSubjectsAsync(userId!);
    }

    return View(viewModels);
}
```

Create GET action güncelleme:
```csharp
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
    else  // Teacher
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        subjects = await _teacherProfileService.GetTeacherSubjectsAsync(userId!);
    }

    ViewBag.Subjects = subjects;
    return View();
}
```

Create POST action güncelleme:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(UnitViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        // Validasyon hatasında da ViewBag'i doğru doldur
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
        return View(viewModel);
    }

    await _unitService.CreateViewModelAsync(viewModel);
    TempData["Success"] = "Ünite başarıyla eklendi";
    return RedirectToAction("Index");
}
```

Edit GET action güncelleme:
```csharp
public async Task<IActionResult> Edit(string id)
{
    ViewData["ActivePage"] = ManageNavPages.Units;
    ViewData["Title"] = "Ünite Düzenle";
    ViewData["PageDescription"] = "Ünite bilgilerini düzenleyin";

    var viewModel = await _unitService.GetViewModelByIdAsync(id);
    if (viewModel == null)
        return NotFound();

    // Teacher için: Kendi dersine ait mi kontrol et
    if (!User.IsInRole(UserRoles.Admin))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasSubject = await _teacherProfileService.HasSubjectAsync(userId!, viewModel.SubjectId!);
        if (!hasSubject)
            return Forbid();
    }

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
    return View(viewModel);
}
```

Edit POST action güncelleme:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(UnitViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        // Validasyon hatasında da ViewBag'i doğru doldur
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
        return View(viewModel);
    }

    // Teacher için: Kendi dersine ait mi kontrol et
    if (!User.IsInRole(UserRoles.Admin))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasSubject = await _teacherProfileService.HasSubjectAsync(userId!, viewModel.SubjectId!);
        if (!hasSubject)
            return Forbid();
    }

    await _unitService.UpdateViewModelAsync(viewModel);
    TempData["Success"] = "Ünite başarıyla güncellendi";
    return RedirectToAction("Index");
}
```

Delete action güncelleme:
```csharp
public async Task<IActionResult> Delete(string id)
{
    ViewData["ActivePage"] = ManageNavPages.Units;
    ViewData["Title"] = "Ünite Sil";
    ViewData["PageDescription"] = "Ünite silme onayı";

    var viewModel = await _unitService.GetViewModelByIdAsync(id);
    if (viewModel == null)
        return NotFound();

    // Teacher için: Kendi dersine ait mi kontrol et
    if (!User.IsInRole(UserRoles.Admin))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasSubject = await _teacherProfileService.HasSubjectAsync(userId!, viewModel.SubjectId!);
        if (!hasSubject)
            return Forbid();
    }

    return View(viewModel);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(string id)
{
    var viewModel = await _unitService.GetViewModelByIdAsync(id);
    if (viewModel == null)
        return NotFound();

    // Teacher için: Kendi dersine ait mi kontrol et
    if (!User.IsInRole(UserRoles.Admin))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasSubject = await _teacherProfileService.HasSubjectAsync(userId!, viewModel.SubjectId!);
        if (!hasSubject)
            return Forbid();
    }

    await _unitService.DeleteAsync(id);
    TempData["Success"] = "Ünite başarıyla silindi";
    return RedirectToAction("Index");
}
```

### 4. ITopicService Arayüzü Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/ITopicService.cs`

```csharp
// Admin için - mevcut metod
Task<IEnumerable<TopicViewModel>> GetAllViewModelAsync();

// Teacher için - yeni
Task<IEnumerable<TopicViewModel>> GetByTeacherSubjectsAsync(string userId);
```

### 5. TopicService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Service/Services/TopicService.cs`

Constructor güncelleme:
```csharp
private readonly ITopicRepository _topicRepository;
private readonly ITeacherProfileService _teacherProfileService;  // YENİ

public TopicService(ITopicRepository topicRepository, ITeacherProfileService teacherProfileService)
{
    _topicRepository = topicRepository;
    _teacherProfileService = teacherProfileService;  // YENİ
}
```

Teacher filtreleme implementasyonu:
```csharp
public async Task<IEnumerable<TopicViewModel>> GetByTeacherSubjectsAsync(string userId)
{
    // 1. Teacher'ın derslerini al
    var teacherProfile = await _teacherProfileService.GetWithSubjectsAsync(userId);
    if (teacherProfile == null || !teacherProfile.Subjects.Any())
        return Enumerable.Empty<TopicViewModel>();

    // 2. SubjectId'leri al (null kontrolü ile)
    var subjectIds = teacherProfile.Subjects
        .Where(ts => !string.IsNullOrEmpty(ts.SubjectId))
        .Select(ts => ts.SubjectId!)
        .ToList();

    // 3. Tüm konuları getir
    var allTopics = await _topicRepository.GetAllWithIncludesAsync();

    // 4. Teacher'ın derslerine ait konuları filtrele
    return allTopics
        .Where(t => t.Unit != null && !string.IsNullOrEmpty(t.Unit.SubjectId) && subjectIds.Contains(t.Unit.SubjectId))
        .Select(e => new TopicViewModel
        {
            Id = e.Id,
            Name = e.Name,
            UnitId = e.UnitId,
            Code = e.Code,
            Description = e.Description,
            UnitName = e.Unit?.Name
        }).ToList();
}
```

### 6. TopicController Güncelleme

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

Constructor güncelleme:
```csharp
private readonly ITopicService _topicService;
private readonly IUnitService _unitService;
private readonly ITeacherProfileService _teacherProfileService;  // YENİ

public TopicController(
    ITopicService topicService,
    IUnitService unitService,
    ITeacherProfileService teacherProfileService)  // YENİ
{
    _topicService = topicService;
    _unitService = unitService;
    _teacherProfileService = teacherProfileService;  // YENİ
}
```

Authorize güncelleme:
```csharp
// ÖNCE: [Authorize(Roles = UserRoles.Admin)]
// SONRA:
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
public class TopicController : Controller
```

Index action:
```csharp
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
    else  // Teacher
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        viewModels = await _topicService.GetByTeacherSubjectsAsync(userId!);
    }

    return View(viewModels);
}
```

Create GET action:
```csharp
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
    else  // Teacher
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        units = await _unitService.GetByTeacherSubjectsAsync(userId!);
    }

    ViewBag.Units = units;
    return View();
}
```

Create POST action:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(TopicViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        IEnumerable<UnitViewModel> units;

        if (User.IsInRole(UserRoles.Admin))
        {
            units = await _unitService.GetAllViewModelAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            units = await _unitService.GetByTeacherSubjectsAsync(userId!);
        }

        ViewBag.Units = units;
        return View(viewModel);
    }

    await _topicService.CreateViewModelAsync(viewModel);
    TempData["Success"] = "Konu başarıyla eklendi";
    return RedirectToAction("Index");
}
```

Edit GET action:
```csharp
public async Task<IActionResult> Edit(string id)
{
    ViewData["ActivePage"] = ManageNavPages.Topics;
    ViewData["Title"] = "Konu Düzenle";
    ViewData["PageDescription"] = "Konu bilgilerini düzenleyin";

    var viewModel = await _topicService.GetViewModelByIdAsync(id);
    if (viewModel == null)
        return NotFound();

    // Teacher için: Kendi dersine ait mi kontrol et
    if (!User.IsInRole(UserRoles.Admin))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var unit = await _unitService.GetByIdAsync(viewModel.UnitId!);
        if (unit != null)
        {
            var hasSubject = await _teacherProfileService.HasSubjectAsync(userId!, unit.SubjectId!);
            if (!hasSubject)
                return Forbid();
        }
    }

    IEnumerable<UnitViewModel> units;

    if (User.IsInRole(UserRoles.Admin))
    {
        units = await _unitService.GetAllViewModelAsync();
    }
    else
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        units = await _unitService.GetByTeacherSubjectsAsync(userId!);
    }

    ViewBag.Units = units;
    return View(viewModel);
}
```

Delete actions - UnitController ile aynı mantıkta yetki kontrolü eklenmeli.

### 7. ITeacherProfileService Güncelleme

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/ITeacherProfileService.cs`

```csharp
Task<IEnumerable<SubjectViewModel>> GetTeacherSubjectsAsync(string userId);
```

Not: `HasSubjectAsync(string userId, string subjectId)` zaten mevcut.

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
            Description = s.Description,
            IconClass = s.IconClass
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

### Test 5: Yetki Kontrolü (Delete/Edit)
```
1. Matematik öğretmeni olarak giriş yap
2. Fizik dersine ait bir Unit'in Edit URL'sine doğrudan git
3. 403 Forbidden dönmeli
```

## Güvenlik

### Authorize Kontrolü
Controller seviyesinde `[Authorize]` kontrolü mevcut.

### Ek Güvenlik
- Edit/Delete action'larında teacher için kaynak yetki kontrolü yapılıyor
- Doğrudan URL erişimi engelleniyor (`Forbid()`)

## Riskler

1. **Performance:** Tüm üniteleri/konuları çekip filtreleme - Veri çok büyürse repository katmanında filtreleme yapılmalı
2. **TeacherProfile yoksa:** Boş liste dönülüyor, hata fırlatılmıyor
3. **Role name hardcoding:** UserRoles.Admin ve UserRoles.Teacher string olarak kullanılıyor

## Sonraki Adımlar

1. IUnitService ve UnitService güncelle
2. ITopicService ve TopicService güncelle
3. UnitController ve TopicController güncelle
4. Test
5. Commit
