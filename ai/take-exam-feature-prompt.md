# Öğrenci Sınav Çözme Ekranı Geliştirme (Student Test Environment)

Bu belge, "JelleSmart.ExamSystem" projesindeki Sınav Çözme (Take Exam) ekranının iyileştirilmesi ve UI kurallarına göre düzenlenmesi için hazırlanan bir **görev belgesidir**. AI'a bu dosyayı okumasını söylediğinizde, aşağıdaki plan doğrultusunda geliştirmeye başlaması gerekir.

## Hedefler
1. **Ara Yüz (UI) Kuralları Onarımı (Örn: Areas Kaldırma)**
   - Mevcut `Areas/Teacher` ve `Areas/Student` içerisindeki Controller ve View'lar projeden "Areas" yapısı kaldırılarak ana `Controllers` ve `Views` klasörlerine taşınmalıdır.
   - Örn: `Student/ExamController.cs` -> `StudentExamController.cs` adıyla ana `Controllers` altına taşınmalı ve route'lar güncellenmelidir. `Program.cs` içerisindeki `areas` route'u kaldırılmalıdır. (Eğer bu işlem başka bir görevde yapıldıysa, bu adım atlanabilir, ancak *Areas kesinlikle kullanılmamalıdır*).

2. **Öğrenci Sınav Çözme Ekranı ("Take.cshtml") İyileştirmeleri**
   - **Her Sayfada Bir Soru:** Tüm sorular alt alta değil, her seferinde ekranda sadece **1 soru** görünecek şekilde tasarlanmalıdır.
   - **Geri/İleri (Önceki/Sonraki Soru) Navigasyonu:** Öğrenci süresi yettiği müddetçe önceki sorulara dönebilmelidir.
   - **Kalan Süre (Timer) Gösterimi:** Sunucudan gelen `RemainingTime` üzerinden JavaScript ile geriye sayan bir sayaç yapılmalıdır. Süre bittiğinde sınav otomatik olarak tamamlanmalıdır (Auto-Submit).
   - **Seçilen Cevabın Anında Kaydedilmesi:** Öğrenci bir şıkkı (A, B, C, D) seçtiğinde arka planda AJAX ile `SubmitAnswer` endpoint'ine veri gönderilmeli, böylece tarayıcı kapansa bile cevaplar kaybolmamalıdır.
   - **Puan ve Durum Gösterimi:** Sınavın toplam puanı ve genel ilerleme durumu (örneğin "Soru 3 / Toplam 10") ekranda "joystick / kontrol paneli" mantığıyla *sol alt köşeye* sabitlenebilecek modern bir widget aracılığıyla gösterilmelidir.
   - **Uyarı:** Sınavı tamamlamadan ("Bitir" butonuna basıldığında) önce boş bırakılan sorular varsa öğrenci uyarılmalıdır. Öğrenci onaylarsa `Complete` endpoint'ine yönlendirilmeli ve sınav sonlandırılmalıdır.

3. **Öğretmen Soru Havuzu ve Sınav Atama (Özet Ön Şart)**
   - Sınav sistemi; öğretmenlerin Soru havuzu üzerinden konulara, filtrelere göre sınav hazırlayıp öğrencilere ataması mantığında çalışır. Bu yapı `TeacherQuestionController` ve `TeacherExamController` içinde mevcuttur (Areas'dan taşındıktan sonra). Bu kısımların çalıştığından emin olunmalıdır.

4. **JavaScript (Kurallara Kesin Uyum)**
   - "Take.cshtml" içerisinde `<script>` tag'i ile sayfa bazlı JavaScript yazılması **kesinlikle yasaktır**.
   - Tüm timer, navigasyon, AJAX cevap kaydetme ve form submit mantığı `wwwroot/js/custom/student-exam-take.js` (veya benzer bir isimde) özel JavaScript dosyasında yazılmalı ve `Take.cshtml` içinde sadece referans olarak eklenmelidir.
   - Sınav ekranındaki sayaç, soru geçişleri ve sol alt widget kontrolü bu JS dosyasından yönetilmelidir.

## Dosya Değişiklik Listesi (Beklenen)
- `JelleSmart.ExamSystem.WebUI/Views/StudentExam/Take.cshtml` (veya `Areas/Student/Views/Exam/Take.cshtml` - Areas kontrolüne göre) yapısal HTML değişiklikleri (Bir soru/sayfa yapısı, sol alt süre/puan widget'ı).
- `JelleSmart.ExamSystem.WebUI/wwwroot/js/custom/student-exam-take.js` (Yeni veya güncellenen JS dosyası eklenecek).
- `JelleSmart.ExamSystem.WebUI/Controllers/StudentExamController.cs` (AJAX için gerekirse küçük route DTO uyarlamaları).

## Talimat
Bu işlemi başlattığınızda; öncelikle ilgili View (`Take.cshtml`) içeriğini inceleyin, ardından modern bir "Sol Alt Panel" (süre, puan, soru durumu) ile "Tek Soru" görünümünü entegre edip Javascript dosyasını oluşturarak görevi tamamlayın. İşlemleri yaparken uygulamanın Metronic 7 temasına sadık kalın.
