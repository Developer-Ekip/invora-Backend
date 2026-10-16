# Invora FieldOps – Teknik Proje Notları

Bu dosyada projede kullandığım temel yapıları ve API'nin nasıl çalıştığını özetledim. Kod incelemesi sırasında hem genel sistemi hem de neden bu yöntemleri kullandığımı anlatmak için hazırladığım notlar.

## 1. Projenin amacı

Bu projede şirketlerin sahalarını ve bu sahalarda gerçekleştirilen ziyaretleri yönetebileceğimiz bir backend API geliştirdim.

Sistemde beş temel yapı bulunuyor:

- **Company:** Müşteri firması
- **Site:** Firmaya bağlı çalışma sahası
- **Visit:** Bir sahada planlanan veya gerçekleştirilen ziyaret
- **FieldTask:** Ziyaret sırasında yapılacak görev
- **Issue:** Ziyarette tespit edilen sorun veya arıza

Bir Company'nin birden fazla Site'ı olabilir. Her Visit bir Site'a bağlıdır. Görevler ve sorunlar da ilgili Visit üzerinden tutulur.

Ayrıca ziyaretleri belirli bir FieldUser'a atayabiliyoruz.

## 2. API'de bir istek nasıl işleniyor?

Örneğin `POST /api/visits` isteğiyle yeni bir ziyaret oluşturmak istediğimizi düşünelim.

Öncelikle sistem kullanıcının JWT token'ını kontrol ediyor. Token geçerliyse bu işlemi yapmaya yetkisi olup olmadığına bakılıyor. Yeni ziyaret oluşturma işlemi yalnızca Admin'e açık.

Sonrasında gönderilen bilgiler DTO üzerinden doğrulanıyor. Örneğin zorunlu bir alan eksikse sistem `400 Bad Request` döndürebiliyor.

İstek `VisitsController` tarafından alınıyor ve `IVisitService` üzerinden `VisitService`'e aktarılıyor. Asıl kontroller burada yapılıyor. Sahanın ve varsa atanacak kullanıcının geçerli olup olmadığı kontrol edildikten sonra EF Core aracılığıyla kayıt PostgreSQL'e yazılıyor.

İşlem başarılıysa API yeni ziyaretin bilgilerini `201 Created` yanıtıyla geri gönderiyor.

Burada Controller ile Service'i ayırmamızın nedeni, HTTP işlemleriyle iş mantığını birbirinden ayrı tutmak.

## 3. DTO ve Service yapısı

Projede Entity ve DTO'ları ayrı kullandım.

**Entity**, veritabanında hangi bilgilerin tutulacağını tanımlıyor. **DTO** ise API üzerinden hangi bilgilerin alınacağını veya kullanıcıya gönderileceğini belirliyor.

Bu sayede veritabanı modelini doğrudan dışarı açmamış oluyoruz. Örneğin kullanıcı bilgileri dönerken `PasswordHash` gibi alanların response içinde yer almasını istemiyoruz.

DTO dönüşümleri `Responses.cs` içindeki `ToDto` metotlarıyla yapılıyor.

Service yapısını ise veri tabanı sorgularını ve iş kurallarını Controller'ın dışında tutmak için kullandım.

`Dependency Injection` sayesinde servisleri Controller içinde `new` ile oluşturmam gerekmiyor. İhtiyacım olan servis constructor üzerinden otomatik veriliyor.

## 4. Kullanıcı girişi ve yetkilendirme

Projede JWT tabanlı bir giriş sistemi var.

Kullanıcı kayıt olurken şifresi düz metin olarak değil, `PasswordHasher` ile hash'lenerek veritabanına kaydediliyor.

Login başarılı olduğunda `JwtTokenService` bir saat geçerli JWT oluşturuyor. Bu token kullanıcı ID'si, kullanıcı adı ve rol bilgilerini içeriyor.

Burada iki kavramı ayırıyoruz:

- **Authentication:** Kullanıcının kim olduğunu doğrulamak.
- **Authorization:** Kullanıcının hangi işlemleri yapabileceğini belirlemek.

Sistemde `Admin` ve `FieldUser` olmak üzere iki rol bulunuyor.

Admin şirketleri, sahaları ve ziyaretleri yönetebiliyor. Ziyaretleri kullanıcılara atayabiliyor.

FieldUser ise yalnızca kendisine atanmış ziyaretleri görebiliyor. Ayrıca bu ziyaretlerin durumunu `PATCH` isteğiyle güncelleyebiliyor.

## 5. Kullanıcıların sadece kendi kayıtlarını görmesi

Burası güvenlik açısından önemli.

Sadece `[Authorize]` kullanmak yeterli değil. Çünkü giriş yapmış bir kullanıcının başka kullanıcıların ziyaretlerine erişmesini de engellememiz gerekiyor.

Bunun için `CurrentUser` üzerinden JWT içindeki kullanıcı ID'sini alıyoruz.

`VisitService.Visible` sorgusu kullanıcı rolüne göre farklı çalışıyor:

- Admin bütün ziyaretleri görebiliyor.
- FieldUser yalnızca `AssignedUserId` değeri kendi ID'siyle eşleşen ziyaretleri görebiliyor.

Aynı mantık görev ve sorun kayıtları için de ilgili Visit üzerinden kontrol ediliyor.

Böylece bir kullanıcı başka bir ziyaretin ID'sini bilse bile o kayda doğrudan erişemiyor.

## 6. Entity Framework Core ve PostgreSQL

Veritabanı tarafında PostgreSQL kullandım. C# koduyla veritabanı arasındaki iletişimi EF Core sağlıyor.

`AppDbContext`, tablolarımızı ve aralarındaki ilişkileri yönetiyor.

Veritabanı sorgularında LINQ kullandım. Örneğin bir kullanıcının ziyaretlerini filtrelerken yazdığım LINQ sorgusu EF Core tarafından SQL'e dönüştürülüyor.

Asenkron metotları (`async/await`) veritabanı işlemlerini beklerken uygulamanın iş parçacığını gereksiz yere meşgul etmemek için kullandım.

Sadece veri okuduğumuz bazı sorgularda `AsNoTracking()` bulunuyor. Böylece EF Core'un değişiklik takibi yapmasına gerek kalmıyor.

### Migration

Model üzerinde yaptığımız değişikliklerin veritabanına uygulanması için migration kullanıyoruz.

Son değişikliklerde ziyaretlere `AssignedUserId` alanı eklendi. Böylece bir ziyaretin hangi kullanıcıya atandığını tutabiliyoruz.

Ayrıca kullanıcı adlarının tekrar etmesini engellemek için `Users.Username` alanına unique index eklendi.

## 7. Hata yönetimi

API'de farklı durumlara göre uygun HTTP status code'ları dönmeye çalıştım.

| HTTP kodu | Anlamı |
|---|---|
| `200 OK` | İstek başarılı |
| `201 Created` | Yeni kayıt oluşturuldu |
| `204 No Content` | İşlem başarılı, yanıt gövdesi yok |
| `400 Bad Request` | Gönderilen veriler geçersiz |
| `401 Unauthorized` | Geçerli giriş bilgisi yok |
| `403 Forbidden` | Kullanıcının işlem için yetkisi yok |
| `404 Not Found` | Kayıt bulunamadı veya kullanıcıya görünür değil |
| `409 Conflict` | Veritabanı kısıtlamasıyla çakışma |
| `500 Internal Server Error` | Beklenmeyen sunucu hatası |

Beklenmeyen hatalar `ApiExceptionHandler` tarafından ele alınıyor. İç hata detayları doğrudan kullanıcıya gösterilmiyor.

Loglar üzerinden hangi isteğin yapıldığını, sonucunu ve varsa hata bilgisini takip edebiliyoruz.

## 8. Projeyi nasıl göstereceğim?

Kod incelemesinde önce API'nin genel yapısını, ardından Swagger üzerinden birkaç örnek işlemi göstermek istiyorum.

1. API'yi çalıştırıp Swagger'ı açmak.
2. Admin hesabıyla giriş yapıp şirket, saha ve ziyaret kayıtlarını göstermek.
3. Bir ziyaretin FieldUser'a atanmasını göstermek.
4. FieldUser hesabıyla giriş yapıp yalnızca atanmış ziyaretlerin listelendiğini göstermek.
5. Ziyaret durumunu `PATCH` ile güncellemek.
6. Başka bir FieldUser'ın aynı ziyareti göremediğini göstermek.
7. FieldUser'ın şirket oluşturma yetkisi olmadığını `403 Forbidden` yanıtıyla göstermek.

Demo kullanıcı bilgileri yerel `.local/GIRIS_BILGILERI.txt` dosyasında tutuluyor. Bu dosya GitHub'a yüklenmiyor.

## 9. Kod incelemesinde açıklayabilmem gereken noktalar

**Controller ve Service neden ayrı?**

Controller HTTP isteğini karşılıyor ve yanıtı döndürüyor. Service ise veritabanı sorgularını ve iş mantığını yönetiyor. Bu ayrım kodun bakımını ve test edilmesini kolaylaştırıyor.

**Neden kullanıcı ID'sini request içinden almıyoruz?**

Çünkü kullanıcı kendi isteğine başka bir kullanıcının ID'sini yazabilir. Bunun yerine doğrulanmış JWT içindeki ID'yi kullanıyoruz.

**Neden JWT için Logout endpoint'i yok?**

Bu sürümde token bir saat geçerli. Kullanıcı çıkış yaptığında istemci tarafında token siliniyor. Refresh token veya token iptal sistemi henüz eklenmedi.

**Bir şirket silinirse ne olur?**

Mevcut EF Core ilişki ayarlarına göre şirkete bağlı sahalar ve bunlarla ilişkili ziyaret, görev ve sorun kayıtları da silinebiliyor. Bu yüzden silme işlemleri yalnızca Admin'e açık.

**Migration sırasında unique index hatası neden çıkabilir?**

Veritabanında aynı kullanıcı adına sahip birden fazla kayıt varsa unique index oluşturulamaz. Böyle bir durumda tekrar eden kayıtların düzeltilmesi gerekir.

---

## Kısa özet

Bu projede ASP.NET Core Web API ile bir saha operasyonları yönetim sisteminin backend tarafını geliştirdim. Veritabanı için PostgreSQL, veri erişimi için EF Core, kullanıcı girişi için JWT kullandım.

Projede özellikle Controller–Service ayrımını, DTO kullanımını, Dependency Injection mantığını ve kullanıcıların yalnızca yetkili oldukları kayıtlara erişmesini sağlayan kontrolleri uyguladım.

Temel amacım sadece CRUD endpoint'leri oluşturmak değil, bunları daha düzenli, anlaşılır ve güvenli bir backend yapısı içinde birleştirmekti.
