---
name: warn-large-sql
enabled: true
event: file
action: warn
conditions:
  - field: file_path
    operator: regex_match
    pattern: '\.(cs|sql)$'
  - field: new_text
    operator: regex_match
    pattern: '(?is)(select\s+.+from|with\s+\w+\s+as\s*\().{2500,}'
---

⚠️ **Büyük / karmaşık SQL bloğu tespit edildi.**

Bu değişiklik uzun SQL içeriyor olabilir.

**Öneri:**
- Mümkünse sorguyu ayrı bir `.sql` dosyasında veya yönetilebilir bir yapıda tut
- Parametre kullanımını kontrol et
- Tekrar eden parçaları böl
- Performans ve bakım maliyetini düşün
- SQL injection riskini tekrar gözden geçir