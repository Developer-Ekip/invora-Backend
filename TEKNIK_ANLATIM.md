# Projeyi açıklama rehberi

Bu rehber kod incelemesinde akışı kendi cümlelerinle anlatabilmen için hazırlanmıştır. Kodun çalışması teknik olarak test edilmiştir; işverenin yapacağı görüşme ve değerlendirme ayrı bir süreçtir.

## Projenin amacı

Bir müşteri firmasının sahalarını, bu sahalara planlanan ziyaretleri ve ziyaretteki görev/sorun kayıtlarını yönetiyoruz. Company birden fazla Site içerir. Visit bir Site'a bağlıdır; FieldTask ve Issue bir Visit'e bağlıdır. Visit ayrıca bir FieldUser'a atanabilir.

## Bir istek nasıl çalışır

Örnek: `POST /api/visits`.

1. Authentication middleware JWT'nin imzasını, süresini, issuer ve audience bilgisini doğrular.
2. Controller üzerindeki Authorize, ziyaret oluşturma için Admin rolünü kontrol eder.
3. ApiController, CreateVisitDto üzerindeki doğrulama kurallarını kontrol eder. Hatalı giriş controller çalışmadan 400 dönebilir.
4. VisitsController, IVisitService üzerinden VisitService'i çağırır.
5. VisitService, Site'ın ve atanacak FieldUser'ın varlığını kontrol eder.
6. AppDbContext ve EF Core yeni kaydı PostgreSQL'e yazar.
7. Controller response DTO'sunu ve kaydın adresiyle birlikte 201 Created yanıtını döndürür.

Dependency injection, constructor'da istenen servisi otomatik sağlar. Scoped servis her HTTP isteği için ayrı oluşturulur. AppDbContext de bu yaşam süresini kullanır.

## DTO neden var

Entity veritabanı modelidir. DTO API'nin alacağı ve döndüreceği alanları belirler. İstemci EF ilişkilerini göndermek zorunda kalmaz; response navigation property veya parola hash'i içermez. Eşleme Responses.cs içindeki kısa ToDto metotlarıyla yapılır.

## Authentication ve authorization

Authentication kim olduğunu doğrular. Login doğruysa JwtTokenService kullanıcı ID'si, kullanıcı adı ve rolü içeren, bir saat geçerli JWT üretir. Parola token'a yazılmaz. Parola veritabanında PasswordHasher ile hash'lenir.

Authorization ne yapabileceğini belirler. Admin firma/saha/ziyaret oluşturur ve ziyaret atar. Register her zaman FieldUser oluşturur; istemci kendisine Admin rolü veremez.

CurrentUser, doğrulanmış token içindeki kullanıcı ID'sini ve rolünü okur. VisitService.Visible sorgusu Admin'e tüm ziyaretleri, FieldUser'a AssignedUserId kendi ID'si olan ziyaretleri döndürür. FieldTaskService ve IssueService aynı kontrolü bağlı Visit üzerinden yapar. Böylece başka bir kaydın ID'sini tahmin etmek yetki vermez.

FieldUser yalnızca ziyaretin status alanını PATCH ile değiştirebilir. Site ve atanan kullanıcıyı değiştiren PUT yalnızca Admin'e açıktır.

## Async LINQ ve migration

LINQ sorguları filtreleri SQL'e dönüştürür; async çağrılar veritabanı yanıtını beklerken uygulama iş parçacığını bloklamaz. Liste sorgularında AsNoTracking, yalnızca okunan entity'lerin değişiklik takibini kapatır.

Migration, modelin veritabanı şemasına uygulanacak değişikliklerini tutar. Son migration Visits tablosuna AssignedUserId ekler, Users.Username için unique index oluşturur ve yabancı anahtar kurar. Eski ziyaretler null atamayla korunur; Admin onları atayabilir.

## Hatalar ve loglar

Doğrulama hataları 400, giriş hatası 401, rol yetersizliği 403, bulunamayan/erişilemeyen kayıt 404 döner. Görev/sorun PUT işlemlerinde bulunamayan kayıt veya geçersiz/erişilemeyen Visit için ortak 400 yanıtı kullanılır. Unique veya yabancı anahtar çakışması 409 olur. Beklenmedik hatalar ApiExceptionHandler tarafından 500 ProblemDetails'e çevrilir; iç hata detayları istemciye gönderilmez. JSON loglarında HTTP metodu, yol, durum kodu ve hata bilgisi bulunur.

## Canlı gösterim sırası

1. API'yi README komutuyla çalıştır ve Swagger'ı aç.
2. Admin ile login ol, token'ı Authorize alanına yapıştır.
3. Örnek firma, saha ve ziyareti listele. Yerel giriş dosyasında örnek kayıt ID'leri var.
4. İlk demo FieldUser ile giriş yapıp atanmış ziyareti göster.
5. Ziyaretin durumunu PATCH ile değiştir, görev ve sorun kayıtlarını listele.
6. İkinci demo FieldUser ile aynı ziyaret ID'sini iste; 404 dönmeli.
7. FieldUser ile firma oluşturmayı dene; 403 dönmeli.
8. Admin'e dönüp ziyaretin kullanıcı atamasını değiştir ve görünürlüğün değiştiğini göster.

Yerel hesapların bilgileri `.local/GIRIS_BILGILERI.txt` dosyasında tutulur ve Git'e eklenmez.

## İnceleme soruları

- Controller neden doğrudan her işlemi yapmıyor? İş kuralları ve sorgular service içinde tutuluyor; controller HTTP isteği ve yanıtıyla ilgileniyor.
- Kullanıcı ID'si neden request'ten okunmuyor? Kimlik doğrulanmış JWT claim'inden alınıyor; istemcinin başka bir kullanıcı ID'si göndermesi yetki sağlamıyor.
- JWT logout endpoint'i neden yok? Bu basit sürümde token bir saat sonra geçersiz olur; istemci çıkışta token'ı siler. Refresh token ve iptal listesi bu eğitim kapsamına eklenmedi.
- Firma silinirse ne olur? EF ilişkilerinin cascade davranışı nedeniyle bağlı saha, ziyaret, görev ve sorunlar da silinir.
- Migration tekil kullanıcı adı indeksinde hata verirse? Mevcut veritabanındaki tekrar eden kullanıcı adları düzeltilmelidir; unique kuralı kaldırılmamalıdır.
