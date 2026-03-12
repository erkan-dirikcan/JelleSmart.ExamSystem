# JelloSmart Exam System - Proje Durumu

Tarih: 2026-03-07

---

## 1. PROJENİN MEVCUT MİMARİSİ

### Katman Yapısı (Clean Architecture)
```
JelleSmart.ExamSystem.Core          (Entity, DTO, Enum, Interface)
├── Entities/
│   ├── BaseEntity
│   ├── Identity/ (AppUser, AppRole)
│   └── [Domain Entities: Grade, Subject, Unit, Topic, Question, Exam, etc.]
├── DTOs/
├── Enums/ (UserRoles, ParentType)
└── Interfaces/ (IRepository<T>, IService contracts)

JelleSmart.ExamSystem.Repository     (Data Access)
├── Data/
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── Repositories/
│   └── [Repository<T> implementations]

JelleSmart.ExamSystem.Service         (Business Logic)
├── Services/
│   └── [Service implementations]

JelleSmart.ExamSystem.WebUI          (Presentation)
├── Controllers/ (Account, UserManagement, Home)
├── Views/
├── Areas/
│   ├── Student/ (Controllers/Views)
│   └── Teacher/ (Controllers/Views)
└── wwwroot/
```

### Veritabanı
- SQL Server (uzak sunucu: 76.13.29.62)
- Connection String: `MatTerapistExam`
- ORM: Entity Framework Core 10.0.3

---

## 2. IDENTITY/CORE FAZ 1'DE YAPILANLAR

### 2.1 Yeni Entity'ler
| Entity | Açıklama |
|--------|----------|
| `TeacherProfile` | Öğretmen profili (UserId, Title, Department, HireDate, Subjects) |
| `StudentProfile` | Öğrenci profili (UserId, GradeId, StudentNumber, EnrollmentDate, Parents) |
| `StudentParent` | Veli bilgileri (StudentProfileId, ParentType, FirstName, LastName, PhoneNumber, Email) |
| `TeacherSubject` | Öğretmen-ders çok-çok ilişkisi (TeacherProfileId + SubjectId composite PK) |
| `ParentType (Enum)` | First, Second |

### 2.2 AppUser Güncellemeleri
```csharp
public class AppUser : IdentityUser
{
    // Mevcut alanlar (korunuyor)
    public int? SubjectId { get; set; }  // Öğretmen için tek ders (eski sistem)
    public int? GradeId { get; set; }    // Öğrenci için sınıf (eski sistem)

    // Yeni profil ilişkileri
    public TeacherProfile? TeacherProfile { get; set; }
    public StudentProfile? StudentProfile { get; set; }
}
```

### 2.3 DbContext Configurations
- **Yeni DbSets:** TeacherProfiles, StudentProfiles, StudentParents, TeacherSubjects
- **Unique Index'ler:**
  - `IX_TeacherProfiles_UserId`
  - `IX_StudentProfiles_UserId`
  - `IX_StudentParents_StudentProfileId_ParentType`
  - `PK_TeacherSubjects` (composite: TeacherProfileId + SubjectId)

### 2.4 Repository Layer (Yeni)
```
IAppUserRepository          (IdentityUser BaseEntity'den türemediği için bağımsız)
ITeacherProfileRepository   (TeacherProfile CRUD + Subject işlemleri)
IStudentProfileRepository   (StudentProfile CRUD + Parent işlemleri)
```

### 2.5 Service Layer (Yeni)
```
IEmailService               (Placeholder, logger ile çalışıyor)
IAppUserService            (Password reset backend)
ITeacherProfileService     (Teacher profili + Subject yönetimi)
IStudentProfileService     (Student profili + Parent yönetimi)
```

---

## 3. MIGRATION ADI

**`Phase1_ProfileEntities`**
Dosya: `Repository/Migrations/20260307111331_Phase1_ProfileEntities.cs`

### Oluşan Tablolar:
- StudentProfiles
- TeacherProfiles
- StudentParents
- TeacherSubjects

### Migration Çalıştırma Komutu:
```bash
dotnet ef database update --project JelleSmart.ExamSystem.Repository --startup-project JelleSmart.ExamSystem.WebUI
```

---

## 4. SEEDER MANTIĞI (Bootstrap Admin Yaklaşımı)

### DbSeeder.cs - SeedAdminUserAsync

**Kural:**
```
1. Herhangi bir Admin rolüne sahip kullanıcı var mı kontrol et (GetUsersInRoleAsync)
2. En az bir Admin varsa → Bootstrap admin oluşturma (return)
3. Hiç Admin yoksa → info@jellosmart.com kullanıcısını oluştur
```

**Bootstrap Admin:**
- Email: `info@jellosmart.com`
- Şifre: `Orko123!`
- Ad: Jello, Soyad: Smart
- Rol: Admin

**Idempotent:**
- Var olan kullanıcı bilgileri korunur
- Şifre resetlenmez
- Mevcut adminler değiştirilmez
- Her çalışmada aynı sonuç verir

---

## 5. HANGİ DOSYALAR EKLENDİ

### Core/Entities (4 yeni)
- `TeacherProfile.cs`
- `StudentProfile.cs`
- `StudentParent.cs`
- `TeacherSubject.cs`

### Core/Enums (1 yeni)
- `ParentType.cs`

### Core/DTOs (6 yeni)
- `AppUserDto.cs`
- `CreateAppUserDto.cs`
- `ForgotPasswordDto.cs`
- `ResetPasswordDto.cs`
- `TeacherProfileDto.cs`
- `StudentProfileDto.cs`

### Core/Interfaces/Repositories (3 yeni)
- `IAppUserRepository.cs`
- `ITeacherProfileRepository.cs`
- `IStudentProfileRepository.cs`

### Core/Interfaces/Services (4 yeni)
- `IEmailService.cs`
- `IAppUserService.cs`
- `ITeacherProfileService.cs`
- `IStudentProfileService.cs`

### Repository/Repositories (3 yeni)
- `AppUserRepository.cs`
- `TeacherProfileRepository.cs`
- `StudentProfileRepository.cs`

### Service/Services (4 yeni)
- `EmailService.cs`
- `AppUserService.cs`
- `TeacherProfileService.cs`
- `StudentProfileService.cs`

### Repository/Data (1 yeni)
- `DbSeeder.cs`

### WebUI/Models (1 güncelleme)
- `AccountViewModels.cs` (ForgotPasswordViewModel eklendi)

---

## 6. HANGİ KURALLAR KRİTİK

### Mimari Kurallar
1. **Area kullanılmayacak** - Mevcut Areas taşıncaya kadar geçici olarak korundu
2. **Repository pattern korunacak** - IRepository<T> base class
3. **Role-based identity bozulmayacak** - IdentityUser + RoleManager
4. **Metronic 7 yapısı korunacak** - _Layout ve partial'lar korundu
5. **JavaScript cshtml içine yazılmayacak** - wwwroot/js/custom altında
6. **İlgisiz dosyalar değiştirilmeyecek**

### Kod Kalitesi
1. **TODO comment kalmayacak** - Faz 1'de temizlendi
2. **Nullable reference warning'ler temiz** - Mevcut kodla ilgili
3. **Build hatasız olacak** - 0 Error

### İş Kuralları
1. **Register olmayacak** - Sadece Admin kullanıcı ekleyebilecek
2. **Veli bilgileri opsiyonel** - Max 2 veli, ParentType unique
3. **Sınıf bazlı filtre** - StudentProfile.GradeId zorunlu
4. **Password reset backend hazır** - Token generation + Email placeholder

---

## 7. HANGİ ŞEYLERE DOKUNULMAMASI GEREKİYOR

### KORUNMASI GEREKEN (Faz 2'ye kadar)
- [x] `Program.cs` Areas route (satır 122-124)
- [x] `AppUser.SubjectId` ve `AppUser.GradeId` - Eski sistem için korundu
- [x] `Areas/` klasörü ve içindeki tüm dosyalar
- [x] `_Layout.cshtml` ve shared partial'lar
- [x] Mevcut Controller/View yapısı

### FAKLARDA DOKUNULMAYACAK
- Register.cshtml ve Register action - Faz 2'de kaldırılacak
- AppUser.SubjectId/GradeId - Migration sonrası değerlendirilecek
- EmailService TODO'ları - Placeholder olarak bilerek bırakıldı

---

## 8. ŞU ANKİ GELİŞTİRME AŞAMASI

### Faz 1: Identity/Core Altyapısı ✓ TAMAMLANDI
- [x] Profile entity'leri
- [x] Repository ve Service layer
- [x] Email service abstraction (placeholder)
- [x] Seed data (Bootstrap admin)
- [x] Password reset backend
- [x] Migration hazır (henüz çalıştırılmadı)
- [x] Build başarılı

### Faz 2A: Account Akışı (SONRAKİ)
- [ ] Register kaldırma
- [ ] ForgotPassword view/controller
- [ ] ResetPassword view/controller
- [ ] Profile view/controller
- [ ] Area kaldırma / taşıma

### Faz 2B: User Management (SONRAKİ)
- [ ] Admin kullanıcı ekleme (Create)
- [ ] Kullanıcı listesi (Index - güncelleme)
- [ ] Kullanıcı düzenleme (Edit)
- [ ] Role-based authorization

### Faz 2C: UI/JS Entegrasyonu (SONRAKİ)
- [ ] View render'lar
- [ ] JavaScript dosyaları (wwwroot/js/custom)
- [ ] Form validasyonları
- [ ] AJAX çağrıları

---

## 9. BİR SONRAKİ YAPILACAK İŞ (FAZ 2A: ACCOUNT AKIŞI)

### Kapsam
1. **Register Kaldırma**
   - Views/Account/Register.cshtml sil
   - AccountController.Register action sil
   - Route'tan register kaldır

2. **ForgotPassword**
   - Views/Account/ForgotPassword.cshtml oluştur
   - AccountController.ForgotPassword (GET/POST) ekle
   - wwwroot/js/custom/account-forgot-password.js oluştur

3. **ResetPassword**
   - Views/Account/ResetPassword.cshtml oluştur
   - AccountController.ResetPassword (GET/POST) ekle
   - wwwroot/js/custom/account-reset-password.js oluştur

4. **Profile**
   - Views/Account/Profile.cshtml oluştur
   - AccountController.Profile (GET/POST) ekle
   - wwwroot/js/custom/account-profile.js oluştur
   - Şifre değiştirme, profil güncelleme

5. **Email Service**
   - IEmailService metotları zaten hazır
   - Faz 2'de sadece view tarafını bağlamak yeterli

### Kurallar (Faz 2A için)
- Area kullanılmayacak
- Repository pattern korunacak
- Metronic 7 yapısı korunacak
- JavaScript cshtml içine yazılmayacak
- İlgisiz dosyalar değiştirilmeyecek

---

## 10. ÖNEMLİ NOTLAR

### Connection String
- Dosya: `WebUI/appsettings.json`
- Key: `"DefaultConnection"`
- Uzak sunucu: 76.13.29.62

### Build
```bash
dotnet build --no-restore
```

### Migration
```bash
dotnet ef migrations add [Name] --project JelleSmart.ExamSystem.Repository --startup-project JelleSmart.ExamSystem.WebUI
dotnet ef database update --project JelleSmart.ExamSystem.Repository --startup-project JelleSmart.ExamSystem.WebUI
```

### Test Kullanıcısı (Seed sonrası)
- Email: info@jellosmart.com
- Şifre: Orko123!
- Rol: Admin

### Rolleri
- UserRoles.Admin
- UserRoles.Teacher
- UserRoles.Student

---

## 11. KALAN RİSKLER

| Risk | Seviy | Mitigasyon |
|------|-------|------------|
| Migration çalıştırılmadı | Orta | Veritabanı güncellemesi yapılacak |
| EmailService placeholder | Düşük | Production için SMTP/SendGrid entegrasyonu |
| AppUser.SubjectId/GradeId çakışması | Düşük | Migration sonrası temizlenecek |
| Areas taşıma planı | Düşük | Faz 2'de yapılacak |

---

**Son Güncelleme:** 2026-03-07
**Durum:** Faz 1 Tamamlandı, Faz 2A Bekliyor
