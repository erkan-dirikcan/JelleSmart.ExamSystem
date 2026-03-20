---
name: block-wrong-layer-deps
enabled: true
event: file
action: block
conditions:
  - field: file_path
    operator: regex_match
    pattern: '(WebUI|WebApi|Service|Repository|Core).*\.(cs|csproj)$'
  - field: new_text
    operator: regex_match
    pattern: '(using\s+.*\.Repository\b)|(using\s+.*\.Service\b)|(ProjectReference.+(Repository|Service|WebUI|WebApi|Core))'
---

🧱 **Katman bağımlılığı kontrolü tetiklendi.**

Bu değişiklik mimari bağımlılık yönünü bozuyor olabilir.

**Beklenen yön:**
- WebUI -> Service
- WebApi -> Service
- Service -> Repository
- Repository -> Core

**Yasak örnekler:**
- WebUI -> Repository
- WebApi -> Repository
- Core -> Service / Repository / WebUI / WebApi

Dosyayı kaydetmeden önce bağımlılık yönünü tekrar doğrula.