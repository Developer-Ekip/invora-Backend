# Teknik teslim kontrolü

Kontrol tarihi: 16 Ekim 2026. Yerel bilgisayarda .NET 10 ve PostgreSQL ile doğrulanmıştır.

| Kontrol | Sonuç |
| --- | --- |
| dotnet build | Başarılı, 0 uyarı ve 0 hata |
| EF model ile migration uyumu | Bekleyen model değişikliği yok |
| Sıfırdan ayrı PostgreSQL veritabanına migration | Başarılı |
| Mevcut yerel veritabanına son migration | Başarılı, mevcut kayıtlar korundu |
| Ayrı veritabanında uçtan uca test | 57 HTTP kontrolü geçti |
| Yerel gerçek geliştirme ayarlarıyla uçtan uca test | 57 HTTP kontrolü geçti |
| Yerel Admin oluşturma ve login | Başarılı |
| Örnek firma, saha ve atanmış ziyaret oluşturma | Başarılı |
| FieldUser ile ziyaret durumu, görev ve sorun işlemleri | Başarılı |
| Başka FieldUser'ın ziyarete erişimi | 404, erişim engellendi |
| FieldUser'ın Admin işlemine erişimi | 403, erişim engellendi |
| Swagger UI ve JWT tanımı | HTTP kontrolü ve tarayıcıda açılış başarılı |
| Kod akışının teknik incelemesi | Rol sınırları, sorgu filtreleri, DTO'lar, parola hash'i ve migration kontrol edildi |

Yerelde gösterim için örnek firma, saha, ziyaret, görev ve sorun kaydı bırakılmıştır. İki demo FieldUser hesabı ve test script'inin oluşturduğu test hesapları yerel geliştirme veritabanındadır. Giriş bilgileri yalnızca `.local/GIRIS_BILGILERI.txt` içindedir; bu klasör Git tarafından yok sayılır.

Bu dosya yapılan teknik kontrolleri kaydeder. İnsan reviewer onayı veya işverenin yetkinlik değerlendirmesi yerine geçmez. GitHub PR'ın güncel birleştirme durumu GitHub üzerinden görülebilir.
