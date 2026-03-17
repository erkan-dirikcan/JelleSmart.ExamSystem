# Öğretmen Branş Kısıtlaması - Implementasyon Planı

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Öğretmenlerin sadece atanmış oldukları dersler için Unit ve Topic yönetebilmesini sağlamak.

**Architecture:** Mevcut UnitController ve TopicController genişletilecek. Service katmanında teacher-specific filtering eklenecek. Admin tüm verileri görmeye devam edecek.

**Tech Stack:** ASP.NET Core 8, Entity Framework Core, ASP.NET Core Identity

---

## Chunk 1: Service Katmanı - Unit

### Task 1: IUnitService Arayüzüne Teacher Metodu Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Services/IUnitService.cs`

- [ ] **Step 1: Mevcut interface'i incele**

Dosyayı oku ve mevcut metodları gör.

- [ ] **Step 2: GetByTeacherSubjectsAsync metodunu ekle**

Dosyaya şu metodu ekle:
```csharp
Task<IEnumerable<UnitViewModel>> GetByTeacherSubjectsAsync(string userId);
```

- [ ] **Step 3: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj`
Expected: Build başarılı

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Services/IUnitService.cs
git commit -m "feat: Add GetByTeacherSubjectsAsync to IUnitService interface"
```

---

### Task 2: UnitService'e Teacher Filtreleme Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

- [ ] **Step 1: Mevcut service'i incele**

Constructor ve mevcut metodları gör.

- [ ] **Step 2: ITeacherProfileService dependency'sini ekle**

Constructor'ı güncelle:
```csharp
private readonly IUnitRepository _unitRepository;
private readonly ITeacherProfileService _teacherProfileService;

public UnitService(IUnitRepository unitRepository, ITeacherProfileService teacherProfileService)
{
    _unitRepository = unitRepository;
    _teacherProfileService = teacherProfileService;
}
```

- [ ] **Step 3: GetByTeacherSubjectsAsync metodunu implemente et**

Dosyaya şu metodu ekle:
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

- [ ] **Step 4: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj`
Expected: Build başarılı

- [ ] **Step 5: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/UnitService.cs
git commit -m "feat: Add GetByTeacherSubjectsAsync implementation to UnitService"
```

---

### Task 3: ITeacherProfileService Arayüzüne Metod Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Services/ITeacherProfileService.cs`

- [ ] **Step 1: Mevcut interface'i incele**

Dosyayı oku ve mevcut metodları gör.

- [ ] **Step 2: GetTeacherSubjectsAsync metodunu ekle**

Dosyaya şu metodu ekle:
```csharp
Task<IEnumerable<SubjectViewModel>> GetTeacherSubjectsAsync(string userId);
```

- [ ] **Step 3: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj`
Expected: Build başarılı

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Services/ITeacherProfileService.cs
git commit -m "feat: Add GetTeacherSubjectsAsync to ITeacherProfileService interface"
```

---

### Task 4: TeacherProfileService'e GetTeacherSubjectsAsync Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/TeacherProfileService.cs`

- [ ] **Step 1: Mevcut service'i incele**

Dosyayı oku.

- [ ] **Step 2: GetTeacherSubjectsAsync metodunu implemente et**

Dosyaya şu metodu ekle:
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

- [ ] **Step 3: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj`
Expected: Build başarılı

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/TeacherProfileService.cs
git commit -m "feat: Add GetTeacherSubjectsAsync implementation to TeacherProfileService"
```

---

## Chunk 2: Service Katmanı - Topic

### Task 5: ITopicService Arayüzüne Teacher Metodu Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Services/ITopicService.cs`

- [ ] **Step 1: Mevcut interface'i incele**

Dosyayı oku.

- [ ] **Step 2: GetByTeacherSubjectsAsync metodunu ekle**

Dosyaya şu metodu ekle:
```csharp
Task<IEnumerable<TopicViewModel>> GetByTeacherSubjectsAsync(string userId);
```

- [ ] **Step 3: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj`
Expected: Build başarılı

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Services/ITopicService.cs
git commit -m "feat: Add GetByTeacherSubjectsAsync to ITopicService interface"
```

---

### Task 6: TopicService'e Teacher Filtreleme Ekleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/TopicService.cs`

- [ ] **Step 1: Mevcut service'i incele**

Constructor'ı gör.

- [ ] **Step 2: ITeacherProfileService dependency'sini ekle**

Constructor'ı güncelle:
```csharp
private readonly ITopicRepository _topicRepository;
private readonly ITeacherProfileService _teacherProfileService;

public TopicService(ITopicRepository topicRepository, ITeacherProfileService teacherProfileService)
{
    _topicRepository = topicRepository;
    _teacherProfileService = teacherProfileService;
}
```

- [ ] **Step 3: GetByTeacherSubjectsAsync metodunu implemente et**

Dosyaya şu metodu ekle:
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

- [ ] **Step 4: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Service/JelleSmart.ExamSystem.Service.csproj`
Expected: Build başarılı

- [ ] **Step 5: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/TopicService.cs
git commit -m "feat: Add GetByTeacherSubjectsAsync implementation to TopicService"
```

---

## Chunk 3: Controller Katmanı - UnitController

### Task 7: UnitController Authorize ve Constructor Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Mevcut controller'ı incele**

Dosyayı oku.

- [ ] **Step 2: System.Security.Claims using'ini ekle**

Dosyanın en üstündeki using'lere ekle:
```csharp
using System.Security.Claims;
```

- [ ] **Step 3: ITeacherProfileService field'ını ekle**

Class level'da ekle:
```csharp
private readonly ITeacherProfileService _teacherProfileService;
```

- [ ] **Step 4: Constructor'ı güncelle**

Constructor'ı güncelle:
```csharp
public UnitController(
    IUnitService unitService,
    ISubjectService subjectService,
    ITeacherProfileService teacherProfileService)
{
    _unitService = unitService;
    _subjectService = subjectService;
    _teacherProfileService = teacherProfileService;
}
```

- [ ] **Step 5: Authorize attribute'ını güncelle**

Class attribute'ını güncelle:
```csharp
// ÖNCE: [Authorize(Roles = UserRoles.Admin)]
// SONRA:
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
```

- [ ] **Step 5: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.WebUI/JelleSmart.ExamSystem.WebUI.csproj`
Expected: Build başarılı

- [ ] **Step 6: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "refactor: Add Teacher role and ITeacherProfileService to UnitController"
```

---

### Task 8: UnitController Index Action Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Index action'ı güncelle**

Index metodunu şu şekilde güncelle:
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

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "feat: Add role-based filtering to UnitController Index"
```

---

### Task 9: UnitController Create Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Create GET action'ı güncelle**

Create GET metodunu güncelle:
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

- [ ] **Step 2: Create POST action'ı güncelle**

Create POST metodunu güncelle - ModelState validasyon hatası kısmını da güncelle:
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

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "feat: Add role-based filtering to UnitController Create actions"
```

---

### Task 10: UnitController Edit Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Edit GET action'ı güncelle**

Edit GET metodunu güncelle:
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

- [ ] **Step 2: Edit POST action'ı güncelle**

Edit POST metodunu güncelle:
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

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "feat: Add authorization and role-based filtering to UnitController Edit"
```

---

### Task 11: UnitController Delete Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: Delete GET action'ı güncelle**

Delete GET metodunu güncelle:
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
```

- [ ] **Step 2: DeleteConfirmed POST action'ı güncelle**

DeleteConfirmed POST metodunu güncelle:
```csharp
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

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "feat: Add authorization check to UnitController Delete actions"
```

---

## Chunk 4: Controller Katmanı - TopicController

### Task 12: TopicController Authorize ve Constructor Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Mevcut controller'ı incele**

Dosyayı oku.

- [ ] **Step 2: System.Security.Claims using'ini ekle**

Dosyanın en üstündeki using'lere ekle:
```csharp
using System.Security.Claims;
```

- [ ] **Step 3: ITeacherProfileService field'ını ekle**

Class level'da ekle:
```csharp
private readonly ITeacherProfileService _teacherProfileService;
```

- [ ] **Step 4: Constructor'ı güncelle**

Constructor'ı güncelle:
```csharp
public TopicController(
    ITopicService topicService,
    IUnitService unitService,
    ITeacherProfileService teacherProfileService)
{
    _topicService = topicService;
    _unitService = unitService;
    _teacherProfileService = teacherProfileService;
}
```

- [ ] **Step 5: Authorize attribute'ını güncelle**

Class attribute'ını güncelle:
```csharp
// ÖNCE: [Authorize(Roles = UserRoles.Admin)]
// SONRA:
[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Teacher)]
```

- [ ] **Step 6: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.WebUI/JelleSmart.ExamSystem.WebUI.csproj`
Expected: Build başarılı

- [ ] **Step 7: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "refactor: Add Teacher role and ITeacherProfileService to TopicController"
```

---

### Task 13: TopicController Index Action Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Index action'ı güncelle**

Index metodunu güncelle:
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

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "feat: Add role-based filtering to TopicController Index"
```

---

### Task 14: TopicController Create Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Create GET action'ı güncelle**

Create GET metodunu güncelle:
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

- [ ] **Step 2: Create POST action'ı güncelle**

Create POST metodunu güncelle:
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

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "feat: Add role-based filtering to TopicController Create actions"
```

---

### Task 15: TopicController Edit Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Edit GET action'ı güncelle**

Edit GET metodunu güncelle:
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

- [ ] **Step 2: Edit POST action'ı güncelle**

Edit POST metodunu güncelle:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(TopicViewModel viewModel)
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

    await _topicService.UpdateViewModelAsync(viewModel);
    TempData["Success"] = "Konu başarıyla güncellendi";
    return RedirectToAction("Index");
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "feat: Add authorization and role-based filtering to TopicController Edit"
```

---

### Task 16: TopicController Delete Actions Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

- [ ] **Step 1: Delete GET action'ı güncelle**

Delete GET metodunu güncelle:
```csharp
public async Task<IActionResult> Delete(string id)
{
    ViewData["ActivePage"] = ManageNavPages.Topics;
    ViewData["Title"] = "Konu Sil";
    ViewData["PageDescription"] = "Konu silme onayı";

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

    return View(viewModel);
}
```

- [ ] **Step 2: DeleteConfirmed POST action'ı güncelle**

DeleteConfirmed POST metodunu güncelle:
```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(string id)
{
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

    await _topicService.DeleteAsync(id);
    TempData["Success"] = "Konu başarıyla silindi";
    return RedirectToAction("Index");
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs
git commit -m "feat: Add authorization check to TopicController Delete actions"
```

---

## Chunk 5: Test ve Doğrulama

### Task 17: End-to-End Test

**Files:**
- Test: Manual testing through browser

- [ ] **Step 1: Uygulamayı başlat**

Run: `dotnet run --project JelleSmart.ExamSystem.WebUI`
Expected: Uygulama başarılı şekilde başlar

- [ ] **Step 2: Admin olarak test et**

1. Admin olarak giriş yap
2. Unit Index -> Tüm üniteler görünmeli
3. Topic Index -> Tüm konular görünmeli
4. Yeni unit/topic oluştur -> Tüm dersler seçilebilir

- [ ] **Step 3: Teacher olarak test et (Tek Ders)**

1. Matematik öğretmeni olarak giriş yap
2. Unit Index -> Sadece Matematik üniteleri görünmeli
3. Yeni unit ekle -> Sadece Matematik seçilebilir
4. Topic Index -> Sadece Matematik konuları görünmeli
5. Yeni topic ekle -> Sadece Matematik üniteleri seçilebilir

- [ ] **Step 4: Teacher olarak test et (Birden Fazla Ders)**

1. Matematik + Fizik öğretmeni olarak giriş yap
2. Unit Index -> Her iki dersin üniteleri görünmeli
3. Yeni unit ekle -> Her iki ders de seçilebilir

- [ ] **Step 5: Yetki kontrolü test et**

1. Matematik öğretmeni olarak giriş yap
2. Fizik dersine ait bir Unit'in Edit URL'sine manuel git
3. 403 Forbidden dönmeli

- [ ] **Step 6: DataTable kontrolü**

1. Unit ve Topic sayfalarında DataTable düzgün çalışmalı
2. Sıralama, arama, sayfalama çalışmalı

- [ ] **Step 7: Doğrulama başarılıysa commit**

```bash
git add -A
git commit -m "test: Verify teacher subject restriction working correctly"
```

---

## Bitiş Kontrol Listesi

- [ ] IUnitService'e GetByTeacherSubjectsAsync eklendi
- [ ] UnitService'e GetByTeacherSubjectsAsync implemente edildi
- [ ] ITopicService'e GetByTeacherSubjectsAsync eklendi
- [ ] TopicService'e GetByTeacherSubjectsAsync implemente edildi
- [ ] ITeacherProfileService'e GetTeacherSubjectsAsync eklendi
- [ ] TeacherProfileService'e GetTeacherSubjectsAsync implemente edildi
- [ ] UnitController güncellendi (Teacher role + filtreleme)
- [ ] TopicController güncellendi (Teacher role + filtreleme)
- [ ] Test başarılı
- [ ] Tüm değişiklikler commit edildi

---

## Sonraki Adım

Bu checkpoint tamamlandıktan sonra, PROJECT_ROADMAP.md dosyasında Checkpoint 2'yi işaretleyin ve sonraki checkpoint'e geçin.
