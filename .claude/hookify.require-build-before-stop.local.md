---
name: require-build-before-stop
enabled: true
event: stop
action: warn
pattern: .*
---

🛠️ **İşi tamamlamadan önce doğrulama yap.**

Bu oturumu “bitti” diye kapatmadan önce mümkünse şunları kontrol et:

- `dotnet restore`
- `dotnet build`
- uygunsa `dotnet test`

Ayrıca:
- derlenmeyen projeyi tamamlandı sayma
- build kırıkken commit/push önerme
- yaptığın değişiklikleri kısa ve net özetle