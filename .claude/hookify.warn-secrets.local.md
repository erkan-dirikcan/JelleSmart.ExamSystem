---
name: warn-secrets
enabled: true
event: file
action: warn
conditions:
  - field: new_text
    operator: regex_match
    pattern: '(?i)(password\s*=|pwd\s*=|api[_-]?key\s*=|client[_-]?secret\s*=|connectionstring\s*=|server=.*;database=.*;user id=.*;password=.*;)'
---

⚠️ **Olası gizli bilgi / secret tespit edildi.**

Eklenen içerikte parola, API key, client secret veya connection string benzeri bir veri olabilir.

**Kontrol et:**
- Bu bilgi gerçekten source code içinde mi olmalı?
- `appsettings.*` yerine secret store / environment variable kullanılabilir mi?
- Dosya `.gitignore` ile korunuyor mu?
- Gerekirse örnek değer bırak, gerçek değeri commit etme