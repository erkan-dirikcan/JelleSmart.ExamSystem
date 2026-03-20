---
name: block-dangerous-dotnet-shell
enabled: true
event: bash
action: block
pattern: (rm\s+-rf\s+/|del\s+/f\s+/s\s+/q|rmdir\s+/s\s+/q|drop\s+database|truncate\s+table|git\s+push\s+--force(?!-with-lease)|chmod\s+777|sudo\s+rm)
---

🚫 **Riskli komut tespit edildi.**

Bu komut veri kaybına, geri döndürülemez silmeye veya tehlikeli sistem değişikliklerine yol açabilir.

**İzlenecek yol:**
- Önce komutun etkisini açıkla
- Daha güvenli alternatifi öner
- Gerekirse dry-run veya hedefli komut kullan
- `git push --force` yerine tercihen `--force-with-lease` düşün