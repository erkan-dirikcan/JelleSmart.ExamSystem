# Ünite-Konu Hiyerarşisi Düzeltme - Implementasyon Planı

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ünite yapısından GradeId (Sınıf) özelliğini kaldırarak, her sınıfın aynı ünite adını tekrar oluşturmak zorunda kalma sorununu çözmek.

**Architecture:** Unit entity'sinden GradeId özelliğini ve Grade navigation property'sini kaldıracağız. Üniteler sadece SubjectId ile ilişkilendirilecek. Soru oluşturma sırasında Grade+Subject kombinasyonuna göre ünite listeleme mantığı eklenecek.

**Tech Stack:** ASP.NET Core 8, Entity Framework Core, Metronic 7 Theme

---

## Chunk 1: Entity Değişikliği ve Migration

### Task 1: Unit Entity'den GradeId Kaldırma

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Entities/Unit.cs`

- [ ] **Step 1: Mevcut Unit entity'sini incele**

Dosya şu an şu yapıda:
```csharp
public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Foreign keys
    public string? SubjectId { get; set; }
    public string? GradeId { get; set; }  // BU KALDIRILACAK

    // Navigation properties
    public Subject? Subject { get; set; }
    public Grade? Grade { get; set; }  // BU KALDIRILACAK
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
```

- [ ] **Step 2: GradeId ve Grade navigation property'sini sil**

Dosyayı güncelle:
```csharp
namespace JelleSmart.ExamSystem.Core.Entities
{
    /// <summary>
    /// Ünite (Dersin alt başlıkları)
    /// Sınıftan bağımsız olarak sadece derse bağlı üniteler
    /// </summary>
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Foreign keys
        public string? SubjectId { get; set; }

        // Navigation properties
        public Subject? Subject { get; set; }
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Entities/Unit.cs
git commit -m "feat: Remove GradeId from Unit entity - make units class-independent"
```

---

### Task 2: Migration Oluşturma ve Uygulama

**Files:**
- Create: `JelleSmart.ExamSystem.Repository/Migrations/YYYYMMDDHHMMSS_RemoveGradeIdFromUnit.cs`

- [ ] **Step 1: Migration oluştur**

Run: `dotnet ef migrations add RemoveGradeIdFromUnit --project JelleSmart.ExamSystem.Repository --startup-project JelleSmart.ExamSystem.WebUI`
Expected: Yeni migration dosyası oluşturulur

- [ ] **Step 2: Migration dosyasını incele**

Oluşturulan migration dosyasını kontrol et - şunları içermeli:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropForeignKey(name: "FK_Units_Grades_GradeId", table: "Units");
    migrationBuilder.DropIndex(name: "IX_Units_GradeId", table: "Units");
    migrationBuilder.DropColumn(name: "GradeId", table: "Units");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<string>(name: "GradeId", table: "Units", type: "nvarchar(450)", nullable: true);
    migrationBuilder.CreateIndex(name: "IX_Units_GradeId", table: "Units", column: "GradeId");
    migrationBuilder.AddForeignKey(name: "FK_Units_Grades_GradeId", table: "Units", column: "GradeId", principalTable: "Grades", principalColumn: "Id");
}
```

- [ ] **Step 3: Migration'u uygula**

Run: `dotnet ef database update --project JelleSmart.ExamSystem.Repository --startup-project JelleSmart.ExamSystem.WebUI`
Expected: Database güncellenir, GradeId column'u silinir

- [ ] **Step 4: Migration dosyalarını commit et**

```bash
git add JelleSmart.ExamSystem.Repository/Migrations/
git commit -m "chore: Add RemoveGradeIdFromUnit migration and apply to database"
```

---

## Chunk 2: Repository Interface Değişikliği

### Task 3: IUnitRepository'den GetByGradeAsync'ı Kaldırma

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/Interfaces/Repositories/IUnitRepository.cs`

- [ ] **Step 1: GetByGradeAsync metodunu kaldır**

Dosyayı güncelle:
```csharp
using JelleSmart.ExamSystem.Core.Entities;

namespace JelleSmart.ExamSystem.Core.Interfaces.Repositories
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit?> GetWithTopicsAsync(string id);
        Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId);
        // GetByGradeAsync kaldırıldı - artık GradeId yok
        Task<Unit?> GetByIdWithIncludesAsync(string id);
        Task<IEnumerable<Unit>> GetAllWithIncludesAsync();
    }
}
```

- [ ] **Step 2: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Core/JelleSmart.ExamSystem.Core.csproj`
Expected: Build başarılı

- [ ] **Step 3: Commit**

```bash
git add JelleSmart.ExamSystem.Core/Interfaces/Repositories/IUnitRepository.cs
git commit -m "refactor: Remove GetByGradeAsync from IUnitRepository interface"
```

---

### Task 4: UnitRepository Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Repository/Repositories/UnitRepository.cs`

- [ ] **Step 1: Mevcut repository'i incele**

`GetByIdWithIncludesAsync` ve `GetAllWithIncludesAsync` Grade include ediyor. `GetByGradeAsync` metodu var.

- [ ] **Step 2: Grade include'larını ve GetByGradeAsync'ı kaldır**

Dosyayı güncelle:
```csharp
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Repositories
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Unit?> GetWithTopicsAsync(string id)
        {
            return await _dbSet
                .Include(u => u.Topics.Where(t => !t.IsDeleted))
                .Include(u => u.Subject)
                // Grade include kaldırıldı
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId)
        {
            return await _dbSet
                .Include(u => u.Subject)
                // Grade include kaldırıldı
                .Where(u => u.SubjectId == subjectId && !u.IsDeleted)
                .ToListAsync();
        }

        // GetByGradeAsync metodu interface'den kaldırıldı, buradan da kaldırılıyor

        public async Task<Unit?> GetByIdWithIncludesAsync(string id)
        {
            return await _dbSet
                .Include(u => u.Subject)
                // Grade include kaldırıldı
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<IEnumerable<Unit>> GetAllWithIncludesAsync()
        {
            return await _dbSet
                .Include(u => u.Subject)
                // Grade include kaldırıldı
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }
    }
}
```

- [ ] **Step 3: Build kontrolü**

Run: `dotnet build JelleSmart.ExamSystem.Repository/JelleSmart.ExamSystem.Repository.csproj`
Expected: Build başarılı

- [ ] **Step 4: Commit**

```bash
git add JelleSmart.ExamSystem.Repository/Repositories/UnitRepository.cs
git commit -m "refactor: Remove Grade includes and GetByGradeAsync from UnitRepository"
```

---

## Chunk 3: ViewModel ve Service Değişiklikleri

### Task 5: UnitViewModel Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Core/ViewModels/UnitViewModel.cs`

- [ ] **Step 1: GradeId özelliğini kaldır**

Dosyayı güncelle:
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

        [Required(ErrorMessage = "Ders seçimi gereklidir")]
        public string? SubjectId { get; set; }

        // GradeId KALDIRILDI - Üniteler sınıftan bağımsız

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }

        // For display purposes
        public string? SubjectName { get; set; }
        // GradeName display property de kaldırıldı
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Core/ViewModels/UnitViewModel.cs
git commit -m "refactor: Remove GradeId from UnitViewModel"
```

### Task 6: UnitService Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

- [ ] **Step 1: GradeId atamalarını kaldır**

Dosyayı güncelle:
```csharp
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _unitRepository.GetAllAsync();
        }

        public async Task<Unit?> GetByIdAsync(string id)
        {
            return await _unitRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Unit>> GetBySubjectAsync(string subjectId)
        {
            return await _unitRepository.GetBySubjectAsync(subjectId);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            return await _unitRepository.CreateAsync(unit);
        }

        public async Task UpdateAsync(Unit unit)
        {
            await _unitRepository.UpdateAsync(unit);
        }

        public async Task DeleteAsync(string id)
        {
            await _unitRepository.DeleteAsync(id);
        }

        // ViewModel methods for WebUI
        public async Task<IEnumerable<UnitViewModel>> GetAllViewModelAsync()
        {
            var entities = await _unitRepository.GetAllWithIncludesAsync();
            return entities.Select(e => new UnitViewModel
            {
                Id = e.Id,
                Name = e.Name,
                SubjectId = e.SubjectId,
                // GradeId ataması kaldırıldı
                Description = e.Description,
                SubjectName = e.Subject?.Name
                // GradeName ataması kaldırıldı
            }).ToList();
        }

        public async Task<UnitViewModel?> GetViewModelByIdAsync(string id)
        {
            var entity = await _unitRepository.GetByIdWithIncludesAsync(id);
            if (entity == null)
                return null;

            return new UnitViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                SubjectId = entity.SubjectId,
                // GradeId ataması kaldırıldı
                Description = entity.Description,
                SubjectName = entity.Subject?.Name
                // GradeName ataması kaldırıldı
            };
        }

        public async Task<string> CreateViewModelAsync(UnitViewModel viewModel)
        {
            var entity = new Unit
            {
                Name = viewModel.Name,
                SubjectId = viewModel.SubjectId,
                // GradeId ataması kaldırıldı
                Description = viewModel.Description
            };
            var result = await _unitRepository.CreateAsync(entity);
            return result.Id!;
        }

        public async Task UpdateViewModelAsync(UnitViewModel viewModel)
        {
            var entity = await _unitRepository.GetByIdAsync(viewModel.Id!);
            if (entity == null)
                throw new Exception("Unit not found");

            entity.Name = viewModel.Name;
            entity.SubjectId = viewModel.SubjectId;
            // GradeId ataması kaldırıldı
            entity.Description = viewModel.Description;

            await _unitRepository.UpdateAsync(entity);
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.Service/Services/UnitService.cs
git commit -m "refactor: Remove GradeId assignments from UnitService"
```

---

## Chunk 4: Controller Değişiklikleri

### Task 7: UnitController Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

- [ ] **Step 1: IGradeService inject'ini ve ViewBag.Grades'i kaldır**

Dosyayı güncelle:
```csharp
using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.Interfaces.Services;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.WebUI.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JelleSmart.ExamSystem.WebUI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class UnitController : Controller
    {
        private readonly IUnitService _unitService;
        private readonly ISubjectService _subjectService;
        // IGradeService kaldırıldı

        public UnitController(IUnitService unitService, ISubjectService subjectService)
        {
            _unitService = unitService;
            _subjectService = subjectService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Üniteler";
            ViewData["PageDescription"] = "Sistem ünitelerini yönetin";
            var viewModels = await _unitService.GetAllViewModelAsync();
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Yeni Ünite";
            ViewData["PageDescription"] = "Yeni ünite ekleyin";
            ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
            // ViewBag.Grades kaldırıldı
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
                // ViewBag.Grades kaldırıldı
                return View(viewModel);
            }

            await _unitService.CreateViewModelAsync(viewModel);
            TempData["Success"] = "Ünite başarıyla eklendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Düzenle";
            ViewData["PageDescription"] = "Ünite bilgilerini düzenleyin";

            var viewModel = await _unitService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
            // ViewBag.Grades kaldırıldı
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Subjects = await _subjectService.GetAllViewModelAsync();
                // ViewBag.Grades kaldırıldı
                return View(viewModel);
            }

            await _unitService.UpdateViewModelAsync(viewModel);
            TempData["Success"] = "Ünite başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewData["ActivePage"] = ManageNavPages.Units;
            ViewData["Title"] = "Ünite Sil";
            ViewData["PageDescription"] = "Ünite silme onayı";

            var viewModel = await _unitService.GetViewModelByIdAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _unitService.DeleteAsync(id);
            TempData["Success"] = "Ünite başarıyla silindi";
            return RedirectToAction("Index");
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs
git commit -m "refactor: Remove GradeService and Grades from UnitController"
```

---

## Chunk 5: View Değişiklikleri

### Task 8: Unit Index View Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Unit/Index.cshtml`

- [ ] **Step 1: Sınıf sütununu tablodan kaldır**

Dosyayı güncelle - Sınıf sütunu header'ı (satır 37) ve body'deki GradeName display'ini (satır 61-63) kaldır:
```csharp
@model IEnumerable<JelleSmart.ExamSystem.Core.ViewModels.UnitViewModel>

@{
    ViewData["Title"] = "Üniteler";
}

<div class="d-flex flex-column-fluid">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12">
                <div class="card card-xl-stretch mb-xl-8">
                    <div class="card-header border-0 pt-6">
                        <div class="card-title">
                            <div class="d-flex align-items-center position-relative my-1">
                                <span class="svg-icon svg-icon-1 position-absolute ms-6">
                                    <i class="ki-outline ki-search fs-2"></i>
                                </span>
                                <input type="text" data-kt-unit-table-filter="search" class="form-control form-control-solid w-250px ps-15" placeholder="Ünite ara..." />
                            </div>
                        </div>
                        <div class="card-toolbar">
                            <div class="d-flex justify-content-end" data-kt-unit-table-toolbar="base">
                                <a asp-action="Create" class="btn btn-primary">
                                    <i class="ki-outline ki-plus fs-2"></i>
                                    Yeni Ünite Ekle
                                </a>
                            </div>
                        </div>
                    </div>
                    <div class="card-body py-4">
                        <div class="table-responsive">
                            <table class="table align-middle table-row-dashed fs-6 gy-5" id="unitsTable">
                                <thead>
                                    <tr class="text-start text-muted fw-bold fs-7 text-uppercase gs-0">
                                        <th class="min-w-150px">Ünite Adı</th>
                                        <th class="min-w-125px">Ders</th>
                                        @* Sınıf sütunu kaldırıldı *@
                                        <th class="min-w-200px">Açıklama</th>
                                        <th class="text-end min-w-100px">İşlemler</th>
                                    </tr>
                                </thead>
                                <tbody class="text-gray-600 fw-semibold">
                                    @foreach (var item in Model)
                                    {
                                        <tr>
                                            <td>
                                                <div class="d-flex align-items-center">
                                                    <div class="symbol symbol-40px me-3 bg-light-info">
                                                        <span class="symbol-label text-info fw-bold">
                                                            @(item.Name?.Substring(0, 1).ToUpper())
                                                        </span>
                                                    </div>
                                                    <div class="d-flex flex-column">
                                                        <span class="text-gray-800 fw-bold">@item.Name</span>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                <span class="badge badge-light-primary">@item.SubjectName</span>
                                            </td>
                                            @* Sınıf sütunu kaldırıldı *@
                                            <td>
                                                <span class="text-muted text-truncate d-block" style="max-width: 200px;">
                                                    @(item.Description?.Length > 40 ? item.Description.Substring(0, 40) + "..." : item.Description ?? "-")
                                                </span>
                                            </td>
                                            <td class="text-end">
                                                <div class="d-flex justify-content-end flex-shrink-0">
                                                    <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm me-1">
                                                        <i class="ki-outline ki-pencil fs-5"></i>
                                                    </a>
                                                    <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-icon btn-bg-light btn-active-color-danger btn-sm"
                                                       onclick="return confirm('Bu üniteyi silmek istediğinizden emin misiniz?')">
                                                        <i class="ki-outline ki-trash fs-5"></i>
                                                    </a>
                                                </div>
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/assets/js/custom/units.js"></script>
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Unit/Index.cshtml
git commit -m "refactor: Remove Grade column from Unit Index view"
```

### Task 9: Unit DataTable JavaScript Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/wwwroot/assets/js/custom/units.js`

- [ ] **Step 1: Column sayısını 5'ten 4'e düşür**

Dosyayı güncelle:
```javascript
"use strict";

var UnitsTable = function () {
    var initTable = function () {
        var table = $('#unitsTable');

        // Begin first table
        table.DataTable({
            responsive: true,
            // DOM layout
            dom: `<'row'<'col-sm-12'tr>>
                  <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>`,
            // Order settings
            order: [[0, 'asc']],
            // Column definitions - 4 sütun var artık (Sınıf sütunu kaldırıldı)
            columnDefs: [
                {
                    targets: 0,
                    orderable: true,
                },
                {
                    targets: 1,
                    orderable: true,
                },
                {
                    targets: 2,
                    orderable: true,
                },
                {
                    targets: 3,
                    orderable: false,
                    className: 'text-end'
                }
            ],
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json'
            }
        });
    };

    return {
        init: function () {
            initTable();
        }
    };
}();

// Initialize on document ready
jQuery(document).ready(function () {
    UnitsTable.init();
});
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/wwwroot/assets/js/custom/units.js
git commit -m "refactor: Update units.js for 4 columns instead of 5"
```

### Task 10: Unit Create View Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Unit/Create.cshtml`

- [ ] **Step 1: Grade dropdown'unu ve ViewBag.Grades kontrolünü kaldır**

Dosyayı güncelle:
```csharp
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

                            <div class="row">
                                <div class="col-md-6 mb-5">
                                    <label asp-for="Name" class="form-label">Ünite Adı</label>
                                    <input asp-for="Name" class="form-control form-control-solid" placeholder="Ünite adını girin" />
                                    <span asp-validation-for="Name" class="text-danger fs-7"></span>
                                </div>

                                <div class="col-md-6 mb-5">
                                    <label asp-for="SubjectId" class="form-label">Ders</label>
                                    <select asp-for="SubjectId" class="form-select form-select-solid" asp-items="@(new SelectList(subjects ?? new List<JelleSmart.ExamSystem.Core.ViewModels.SubjectViewModel>(), "Id", "Name"))">
                                        <option value="">Seçiniz...</option>
                                    </select>
                                    <span asp-validation-for="SubjectId" class="text-danger fs-7"></span>
                                </div>
                            </div>

                            @* Grade dropdown kaldırıldı *@

                            <div class="mb-5">
                                <label asp-for="Description" class="form-label">Açıklama</label>
                                <textarea asp-for="Description" class="form-control form-control-solid" rows="3" placeholder="Ünite açıklamasını girin"></textarea>
                                <span asp-validation-for="Description" class="text-danger fs-7"></span>
                            </div>

                            <div class="d-flex justify-content-end">
                                <a asp-action="Index" class="btn btn-light me-3">İptal</a>
                                <button type="submit" class="btn btn-primary">
                                    <span class="indicator-label">Kaydet</span>
                                    <span class="indicator-progress">
                                        Lütfen bekleyin...
                                        <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
                                    </span>
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
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Unit/Create.cshtml
git commit -m "refactor: Remove Grade dropdown from Unit Create view"
```

### Task 11: Unit Edit View Güncelleme

**Files:**
- Modify: `JelleSmart.ExamSystem.WebUI/Views/Unit/Edit.cshtml`

- [ ] **Step 1: Grade dropdown'unu kaldır**

Dosyayı güncelle - Grade dropdown bloğunu sil (satır 39-45 arası):
```csharp
@model JelleSmart.ExamSystem.Core.ViewModels.UnitViewModel

@{
    ViewData["Title"] = "Ünite Düzenle";
}

<div class="d-flex flex-column-fluid">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12">
                <div class="card card-xl-stretch mb-5 mb-xl-8">
                    <div class="card-header border-0 pt-5">
                        <h3 class="card-title align-items-start flex-column">
                            <span class="card-label fw-bold fs-3 mb-1">Ünite Düzenle</span>
                            <span class="text-muted fw-semibold fs-7">Ünite bilgilerini güncelleyin</span>
                        </h3>
                    </div>
                    <div class="card-body py-3">
                        <form asp-action="Edit" method="post" id="unitForm">
                            <div asp-validation-summary="ModelOnly" class="alert alert-danger d-none" role="alert"></div>
                            <input type="hidden" asp-for="Id" />

                            <div class="row">
                                <div class="col-md-6 mb-5">
                                    <label asp-for="Name" class="form-label">Ünite Adı</label>
                                    <input asp-for="Name" class="form-control form-control-solid" placeholder="Ünite adını girin" />
                                    <span asp-validation-for="Name" class="text-danger fs-7"></span>
                                </div>

                                <div class="col-md-6 mb-5">
                                    <label asp-for="SubjectId" class="form-label">Ders</label>
                                    <select asp-for="SubjectId" class="form-select form-select-solid" asp-items="@(new SelectList(ViewBag.Subjects, "Id", "Name"))">
                                        <option value="">Seçiniz...</option>
                                    </select>
                                    <span asp-validation-for="SubjectId" class="text-danger fs-7"></span>
                                </div>
                            </div>

                            @* Grade dropdown kaldırıldı *@

                            <div class="mb-5">
                                <label asp-for="Description" class="form-label">Açıklama</label>
                                <textarea asp-for="Description" class="form-control form-control-solid" rows="3" placeholder="Ünite açıklamasını girin"></textarea>
                                <span asp-validation-for="Description" class="text-danger fs-7"></span>
                            </div>

                            <div class="d-flex justify-content-end">
                                <a asp-action="Index" class="btn btn-light me-3">İptal</a>
                                <button type="submit" class="btn btn-primary">
                                    <span class="indicator-label">Güncelle</span>
                                    <span class="indicator-progress">
                                        Lütfen bekleyin...
                                        <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
                                    </span>
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
    <script src="~/assets/js/custom/units-form.js"></script>
}
```

- [ ] **Step 2: Commit**

```bash
git add JelleSmart.ExamSystem.WebUI/Views/Unit/Edit.cshtml
git commit -m "refactor: Remove Grade dropdown from Unit Edit view"
```

---

## Chunk 6: Test ve Doğrulama

### Task 12: End-to-End Test

**Files:**
- Test: Manual testing through browser

- [ ] **Step 1: Uygulamayı başlat**

Run: `dotnet run --project JelleSmart.ExamSystem.WebUI`
Expected: Uygulama başarılı şekilde başlar

- [ ] **Step 2: Admin giriş yap**

1. Admin olarak giriş yap
2. Unit Index sayfasına git
3. Tüm üniteler listelenmeli (hata vermemeli)
4. Sınıf sütunu OLMAMALI

- [ ] **Step 3: Sınıfsız Ünite Oluşturma**

1. Create Unit sayfasına git
2. Grade dropdown OLMAMALI
3. Sadece Subject seçimi olmalı
4. Name: "1. Ünite - Sayılar"
5. Subject: "Matematik" seç
6. Kaydet
7. Başarılı olduğu doğrula

- [ ] **Step 4: Ünite Düzenleme**

1. Oluşturulan üniteyi düzenle
2. Grade dropdown OLMAMALI
3. Düzenleme başarılı olmalı

- [ ] **Step 5: Topic oluşturma**

1. Topic Create sayfasına git
2. Unit seçiminde önceki üniteler görülmeli
3. Topic oluştur: "Tam Sayılar" (Unit: "1. Ünite - Sayılar")

- [ ] **Step 6: Soru oluşturma**

1. Teacher ile giriş yap
2. Question Create sayfasına git
3. Grade seç: 7. Sınıf
4. Subject seç: Matematik
5. Unit seç: Sadece Matematik üniteleri gelmeli
6. Topic seç: Seçilen ünitenin konuları gelmeli
7. Soruyu kaydet

- [ ] **Step 7: DataTable kontrolü**

1. Unit Index sayfasında DataTable düzgün çalışmalı
2. Sıralama çalışmalı
3. Arama çalışmalı
4. Sayfalama çalışmalı

- [ ] **Step 8: Doğrulama başarılıysa commit**

```bash
git add -A
git commit -m "test: Verify Unit-Topic hierarchy fix working correctly"
```

---

## Bitiş Kontrol Listesi

- [ ] Entity GradeId kaldırıldı
- [ ] Migration oluşturulup uygulandı
- [ ] Repository interface'den GetByGradeAsync kaldırıldı
- [ ] Repository'den Grade include'ları ve GetByGradeAsync kaldırıldı
- [ ] ViewModel'den GradeId kaldırıldı
- [ ] Service'ten GradeId atamaları kaldırıldı
- [ ] Controller'dan GradeService ve Grades kaldırıldı
- [ ] Index view'den Grade sütunu kaldırıldı
- [ ] units.js 4 sütun için güncellendi
- [ ] Create view'den Grade dropdown kaldırıldı
- [ ] Edit view'den Grade dropdown kaldırıldı
- [ ] Test başarılı
- [ ] Tüm değişiklikler commit edildi

---

## Sonraki Adım

Bu checkpoint tamamlandıktan sonra, PROJECT_ROADMAP.md dosyasında Checkpoint 1'i işaretleyin ve sonraki checkpoint'e geçin.
