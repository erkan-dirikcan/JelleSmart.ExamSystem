# Ünite-Konu Hiyerarşisi Düzeltme Tasarımı

## Tarih
2026-03-16

## Amaç
Ünite ve konu yapısını sınıftan bağımsız hale getirerek, her sınıfın aynı ünite adını tekrar oluşturmak zorunda kalma sorununu çözmek.

## Mevcut Durum

### Entity Yapısı
```csharp
public class Unit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int SubjectId { get; set; }      // ZORUNLU
    public int GradeId { get; set; }        // ZORUNLU - SORUN KAYNAĞI
    public Subject Subject { get; set; }
    public Grade Grade { get; set; }
    public ICollection<Topic> Topics { get; set; }
}

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UnitId { get; set; }         // ZORUNLU
    public Unit Unit { get; set; }
    public ICollection<Question> Questions { get; set; }
}
```

### Sorun
- "1. Ünite - Sayılar" hem 7. sınıf hem 8. sınıf için ayrı oluşturulmak zorunda
- Veri tekrarı ve yönetim zorluğu

## Hedeflenen Yapı

### Yeni Entity Yapısı
```csharp
public class Unit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int SubjectId { get; set; }      // ZORUNLU
    // GradeId KALDIRILDI
    public Subject Subject { get; set; }
    public ICollection<Topic> Topics { get; set; }
}

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UnitId { get; set; }         // ZORUNLU
    public Unit Unit { get; set; }
    public ICollection<Question> Questions { get; set; }
}
```

### Filtreleme Mantığı
- **Ünite listeleme:** Ders seçilir → o dersteki tüm üniteler gelir
- **Konu listeleme:** Ünite seçilir → o üniteye ait konular + sınıf filtresi uygulanır
- **Soru oluşturma:** Sınıf seçilir → o sınıfın dersine ait üniteler listelenir

## Değişiklik Listesi

### 1. Entity Katmanı

**Dosya:** `JelleSmart.ExamSystem.Core/Entities/Unit.cs`

```csharp
// GradeId özelliği ve navigation property kaldırılacak
// [Required]
// public int GradeId { get; set; }
//
// [ForeignKey(nameof(GradeId))]
// public virtual Grade? Grade { get; set; }
```

### 2. Repository Katmanı

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces Repositories/IUnitRepository.cs`

Yeni metod eklenmesi:
```csharp
Task<IEnumerable<Unit>> GetBySubjectAndGradeAsync(int subjectId, int gradeId);
```

**Dosya:** `JelleSmart.ExamSystem.Repository/Repositories/UnitRepository.cs`

Implementasyon:
```csharp
public async Task<IEnumerable<Unit>> GetBySubjectAndGradeAsync(int subjectId, int gradeId)
{
    // Sadece o dersin ünitelerini getir (sınıf filtresi yok)
    return await _dbSet
        .Include(u => u.Subject)
        .Where(u => u.SubjectId == subjectId && !u.IsDeleted)
        .ToListAsync();
}
```

### 3. Service Katmanı

**Dosya:** `JelleSmart.ExamSystem.Core/Interfaces/Services/IUnitService.cs`

```csharp
Task<IEnumerable<UnitViewModel>> GetBySubjectAndGradeAsync(int subjectId, int gradeId);
```

**Dosya:** `JelleSmart.ExamSystem.Service/Services/UnitService.cs`

ViewModel dönüşlü implementasyon.

### 4. Controller Katmanı

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/UnitController.cs`

Create action güncellemesi:
- GradeId seçimini kaldır
- Sadece SubjectId seçimi kalsın

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/TopicController.cs`

Create action'da GradeId filtreleme:
- Topic yaratılırken sınıf bilgisine göre ünite listelenecek

**Dosya:** `JelleSmart.ExamSystem.WebUI/Controllers/TeacherQuestionController.cs`

Soru oluşturma sırası:
1. Sınıf seç (zorunlu)
2. Ders seç (zorunlu, sınıfın dersinden)
3. Ünite seç (sınıf+ders kombinasyonuna göre)
4. Konu seç (seçilen üniteye göre)

### 5. View Katmanı

**Dosya:** `JelleSmart.ExamSystem.WebUI/Views/Unit/Create.cshtml`

Grade dropdown kaldırılacak.

**Dosya:** `JelleSmart.ExamSystem.WebUI/Views/Unit/Edit.cshtml`

Grade dropdown kaldırılacak.

**Dosya:** `JelleSmart.ExamSystem.WebUI/Views/Topic/Create.cshtml`

Unit dropdown güncellenecek - sınıf bilgisiyle filtreleme.

**Dosya:** `JelleSmart.ExamSystem.WebUI/Views/TeacherQuestion/Create.cshtml`

Sıralama değişecek:
- Önce Sınıf
- Sonra Ders (sınıfın dersleri)
- Sonra Ünite (seçilen sınıf+ders için)
- Sonra Konu

### 6. Migration

**Yeni migration:** `RemoveGradeIdFromUnit`

SQL değişiklikleri:
```sql
-- GradeId constraint'ini kaldır
ALTER TABLE [Units] DROP CONSTRAINT [FK_Units_Grades_GradeId];

-- GradeId column'unu kaldır
ALTER TABLE [Units] DROP COLUMN [GradeId];

-- Index'leri temizle (varsa)
DROP INDEX IF EXISTS [IX_Units_GradeId] ON [Units];
```

## Test Senaryoları

### Test 1: Sınıfsız Ünite Oluşturma
```
1. Admin olarak giriş yap
2. Ünite oluştur sayfasına git
3. Ders: Matematik seç
4. Ünite adı: "1. Ünite - Sayılar" gir
5. Kaydet
6. Başarılı olduğu doğrula
```

### Test 2: Farklı Sınıflar Aynı Ünite
```
1. Admin olarak giriş yap
2. 7. Sınıf için "Tam Sayılar" konusu oluştur (1. Ünite altında)
3. 8. Sınıf için "Rasyonel Sayılar" konusu oluştur (aynı 1. Ünite altında)
4. Her iki konu da aynı ünite altında görüldüğünü doğrula
```

### Test 3: Soru Oluşturma Sıralı
```
1. Öğretmen olarak giriş yap
2. Yeni soru oluştur
3. Sınıf: 7. Sınıf seç
4. Ders: Matematik seç
5. Ünite: 1. Ünite - Sayılar seç (sadece 7. sınıf Matematik üniteleri)
6. Konu: Tam Sayılar seç
7. Soruyu kaydet ve doğrula
```

### Test 4: Mevcut Veri Bütünlüğü
```
1. Migration öncesi mevcut ünite sayısını not al
2. Migration çalıştır
3. Verilerin korunduğunu doğrala
4. GradeId null olan kayıtlar düzgün çalışmalı
```

## Geriye Uyumluluk

- Mevcut Unit kayıtlarının GradeId'si null olarak ayarlanacak
- Soru ve Sınav kayıtları etkilenmeyecek (onlar zaten GradeId ile filtreleniyor)
- Öğretmen ve Öğrenci view'larında değişiklik gerekmiyor

## Riskler

1. **Mevcut veriler:** Migration sırasında GradeId bilgisi kaybolacak - ama bu istenen durum
2. **Kullanıcı alışkanlığı:** Admin'ler ünite eklerken sınıf seçemeyecek - dokümantasyon gerekli
3. **Test kapsamı:** Tüm view'ların test edilmesi gerekiyor

## Sonraki Adımlar

1. Entity değişikliği
2. Migration oluştur ve çalıştır
3. Repository güncelle
4. Service güncelle
5. Controller güncelle
6. View güncelle
7. Test
8. Commit
