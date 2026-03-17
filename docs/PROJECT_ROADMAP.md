# Öğrenci Seviye Tespit ve İlerleme Sistemi - Proje Yol Haritası

## Proje Durumu: 🟡 İNŞA AŞAMASINDA

**Oluşturma Tarihi:** 2026-03-16
**Son Güncelleme:** 2026-03-16

---

## 📊 MEVCUT DURUM ANALİZİ

### ✅ Tamamlanan Bölümler
- [x] ASP.NET Core mimari (Core, Service, Repository, WebUI katmanları)
- [x] Identity tabanlı kimlik doğrulama (Admin, Teacher, Student rolleri)
- [x] Temel entity yapısı (Grade, Subject, Unit, Topic, Question, Exam, StudentExam)
- [x] Admin paneli (Grade, Subject, Unit, Topic CRUD işlemleri)
- [x] Öğretmen soru oluşturma (TeacherQuestionController)
- [x] Öğrenci sınav girme (StudentExamController)
- [x] Temel repository ve service katmanları

### ❌ EKSİKLİKLER VE SORUNLAR

---

## 🔴 KRİTİK EKSİKLİKLER (ÖNCELİK 1)

### 1. Sınıf-Ünite-Konu Hiyerarşisi Sorunu
**Soru:** Her sınıfın aynı ünite adını tekrar oluşturmak zorunda kalınıyor.
**Şu anki durum:** `Unit` entity'si hem `SubjectId` hem de `GradeId` gerektiriyor.

**Çözüm:** Ünite yapısı yeniden tasarlanmalı:
- Üniter sınıftan bağımsız olmalı (ders altında hiyerarşik)
- Konular da benzer şekilde sınıftan bağımsız olmalı
- Sınav/soru oluştururken sınıf+ders kombinasyonuna göre ünite filtrelenmeli

**Test Metodu:**
```
1. Admin: Matematik dersi için "1. Ünite - Sayılar" ünitesi oluştur (sınıf seçmeden)
2. Admin: Aynı üniteye 7. sınıf için "Tam Sayılar" konusu ekle
3. Admin: Aynı üniteye 8. sınıf için "Rasyonel Sayılar" konusu ekle
4. Doğrula: Her iki konu da aynı ünite altında görülmeli
```

**Checkpoint 1:** [x] Ünite-Konu hiyerarşisi düzeltildi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-unit-topic-hierarchy-fix-design.md`

---

### 2. Öğretmen Branş Kısıtlaması
**Soru:** Öğretmenler sadece kendi branşlarına (SubjectId) ünite ve konu ekleyebilmeli.

**Şu anki durum:**
- `UnitController` ve `TopicController` sadece Admin rolüne açık
- Öğretmenler için ayrı controller yok

**Çözüm:**
- `TeacherUnitController` ve `TeacherTopicController` oluşturulmalı
- Sadece öğretmenin `SubjectId` sine ait verileri görmeli ve ekleyebilmeli
- Admin tüm dersleri görmeye devam etmeli

**Test Metodu:**
```
1. Matematik öğretmeni ile giriş yap
2. Sadece Matematik dersi üniteleri görülmeli
3. Matematik için yeni ünite/konu eklenebilmeli
4. Fizik dersi seçilememeli
```

**Checkpoint 2:** [ ] Öğretmen branş kısıtlaması eklendi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-teacher-subject-restriction-design.md`

---

### 3. Soru Oluşturma Hatası
**Soru:** Kullanıcı soru oluştururken hata alıyor.

**Olası Nedenler:**
- `Question.Id` `string` ama `Unit.Id` ve `Topic.Id` `int` - type mismatch
- Foreign key ilişkilerinde tutarsızlık
- ViewModel'de Id type uyumsuzluğu

**Çözüm:**
- Tüm entity Id'leri tutarlı tipe çevrilmeli (öneri: hepsi `string`)
- ViewModel'lerde Id type'ları kontrol edilmeli
- Foreign key ilişkileri düzeltilmeli

**Test Metodu:**
```
1. Öğretmen ile giriş yap
2. Yeni soru oluştur (tüm alanları doldur)
3. Kaydet ve başarı mesajı doğrula
4. Soru listesinde göründüğünü doğrula
```

**Checkpoint 3:** [ ] Soru oluşturma hatası düzeltildi
**Devam Skill Prompt:** `/superpowers:systematic-debugging` kullanarak hatayı tekrar üret ve düzelt

---

## 🟡 ÖNEMLİ EKSİKLİKLER (ÖNCELİK 2)

### 4. Soru Zorluk Seviyesi (1-5)
**Soru:** Soru havuzu 1-5 arası derecelendirmeli olmalı (1=en zor, 5=en kolay).

**Şu anki durum:** `Question.Difficulty` 1-3 arasında (Kolay, Orta, Zor)

**Çözüm:**
- `Question.Difficulty` 1-5 arasına çıkarılacak
- UI'da 1-5 arası seçim sunulacak
- Açıklama: "1. Seviye (En Zor)" - "5. Seviye (En Kolay)"

**Test Metodu:**
```
1. Soru oluştururken 5 seviye seçeneği görülmeli
2. Her seviyeden soru oluşturulabilmeli
3. Veritabanında doğru kaydedildiği doğrulanmalı
```

**Checkpoint 4:** [ ] Soru zorluk seviyesi 1-5'e çıkarıldı
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-difficulty-levels-1-5-design.md`

---

### 5. Seviyeye Göre Sınav Oluşturma
**Soru:** Sınav oluştururken seçilen zorluk seviyelerine göre sorular gelmeli.

**Şu anki durum:** `ExamService.CreateAsync` tamamen rastgele soru seçiyor

**Çözüm:**
- `CreateExamViewModel`a seviye seçimi eklenmeli
- `ExamService` seçilen seviyelerdeki soruları filtrelemeli
- Örnek: 2 soru 1. seviye, 3 soru 2. seviye, 5 soru 3. seviye...

**Test Metodu:**
```
1. Sınav oluştur: 10 soru, 2'si 1. seviye, 3'ü 2. seviye, 5'i 3. seviye
2. Sınavı oluştur ve soruları görüntüle
3. Her seviyeden doğru sayıda soru geldiğini doğrula
```

**Checkpoint 5:** [ ] Seviyeye göre sınav oluşturma eklendi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-level-based-exam-design.md`

---

## 🟢 İŞLEVSEL EKSİKLİKLER (ÖNCELİK 3)

### 6. Konu Bazlı Sonuç Grafikleri
**Soru:** Sınav sonuçları konu bazlı grafiklerle gösterilmeli.

**İstenen:** Çubuk grafikte her konu için doğru/yanlış gösterimi

**Çözüm:**
- `IReportService` genişletilmeli
- Konu bazlı istatistik DTO'su oluşturulmalı
- Frontend'de Chart.js veya benzeri kütüphane ile görselleştirme
- Öğretmen ve öğrenci sonuç ekranında grafik gösterilmeli

**Test Metodu:**
```
1. 10 konudan oluşan bir sınav oluştur
2. Öğrenci sınavı çözsün
3. Sonuç ekranında 10 çubuk görülmeli
4. Her çubukta o konudaki doğru/yanlış sayısı görülmeli
```

**Checkpoint 6:** [ ] Konu bazlı sonuç grafikleri eklendi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-topic-result-chart-design.md`

---

### 7. Öğrenci Seviye Takibi
**Soru:** Öğrencinin her konudaki seviyesi takip edilmeli.

**Çözüm:**
- `StudentTopicLevel` entity'si oluşturulmalı
- Her sınav sonrasında konu bazlı performans güncellenmeli
- Öğretmen öğrencinin seviyesini görebilmeli

**Test Metodu:**
```
1. Öğrenci sınavı çözsün
2. Sonuçlara göre öğrencinin seviyesi güncellenmeli
3. Öğretmen rapor ekranında öğrencinin her konudaki seviyesini görebilmeli
```

**Checkpoint 7:** [ ] Öğrenci seviye takibi eklendi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-student-level-tracking-design.md`

---

## 🔵 VERİ YAPISI DÜZENLEMELERİ (ÖNCELİK 4)

### 8. ID Type Uyumsuzluğu
**Soru:** Entity ID'leri tutarsız (int ve string karışık)

**Şu anki durum:**
- `Grade.Id`, `Subject.Id`, `Unit.Id`, `Topic.Id` → `int`
- `Question.Id`, `Exam.Id`, `AppUser.Id` → `string`

**Çözüm:** Tüm ID'ler `string` (GUID) olmalı

**Checkpoint 8:** [ ] ID type uyumsuzluğu düzeltildi
**Devam Skill Prompt:** `/superpowers:executing-plans` ile `docs/superpowers/specs/YYYY-MM-DD-id-type-consistency-design.md`

---

## 📋 UYGULAMA SIRASI

### Aşama 1: Kritik Düzeltmeler (Checkpoint 1-3)
1. Ünite-Konu hiyerarşisi düzeltme
2. Öğretmen branş kısıtlaması
3. Soru oluşturma hatası düzeltme

### Aşama 2: Seviye Sistemi (Checkpoint 4-5)
4. Soru zorluk seviyesi 1-5'e çıkarma
5. Seviyeye göre sınav oluşturma

### Aşama 3: Raporlama (Checkpoint 6-7)
6. Konu bazlı sonuç grafikleri
7. Öğrenci seviye takibi

### Aşama 4: Temizlik (Checkpoint 8)
8. ID type uyumsuzluğu düzeltme

---

## 🔧 TEST STRATEJİSİ

### End-to-End Test Akışı
```
1. ADMIN: Ders oluştur (Matematik)
2. ADMIN: Sınıf oluştur (7. Sınıf, 8. Sınıf)
3. ADMIN: Ünite oluştur (1. Ünite - Sayılar) - sınıf seçmeden
4. ADMIN: Konular oluştur:
   - 7. Sınıf için: Tam Sayılar
   - 8. Sınıf için: Rasyonel Sayılar
5. ÖĞRETMEN (Matematik): Her konudan 5 seviyede soru ekle (20 soru)
6. ÖĞRETMEN: Seviyeye göre sınav oluştur (10 soru)
7. ÖĞRENCİ: Sınava gir
8. DOĞRULA: Sonuç ekranında konu bazlı grafik görüntülensin
9. DOĞRULA: Öğrencinin her konudaki seviyesi kaydedilsin
```

---

## 📝 NOTLAR

- Her checkpoint sonrasında backend testleri çalıştırılmalı
- Frontend değişiklikleri için Metronic tema yapısı korunmalı
- Yeni entity eklerken migration oluşturulmalı
- Kullanıcı deneyimi test edilmeli

---

## 💬 SON DURUM

**Tamamlanan Checkpointler:** 1/8
**Mevcut Durum:** Checkpoint 1 tamamlandı
**Sonraki Adım:** Checkpoint 2 - Öğretmen branş kısıtlaması

---

*Doküman otomatik olarak oluşturuldu ve güncellenmektedir.*
