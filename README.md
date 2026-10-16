# Invora FieldOps Training

ASP.NET Core 10, EF Core ve PostgreSQL ile hazırlanmış basit saha operasyon API'si.
Akış: Company → Site → Visit → FieldTask / Issue.
Controller isteği alır, service iş kurallarını ve erişimi kontrol eder, AppDbContext veritabanına erişir.
Ek repository, CQRS veya karmaşık mimari kullanılmaz.

## Gereksinimler

- .NET SDK 10
- PostgreSQL (yerel kurulum veya Docker)
- EF aracı: `dotnet tool restore` (sürüm repository içinde sabitlenmiştir)

Komutları solution klasöründen çalıştırın.

## Yerel kurulum

1. PostgreSQL'de `invora_fieldops` adlı veritabanını oluşturun.
2. Yerel ayarları User Secrets ile tanımlayın. Aşağıdaki örnek değerleri kendiniz değiştirin; gerçek şifreleri Git'e eklemeyin.

```sh
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' 'Host=localhost;Port=5432;Database=invora_fieldops;Username=postgres;Password=YOUR_DATABASE_PASSWORD' --project Invora.FieldOps.Api
dotnet user-secrets set 'Jwt:Key' 'YOUR_RANDOM_SECRET_AT_LEAST_32_BYTES_LONG' --project Invora.FieldOps.Api
dotnet user-secrets set 'Jwt:Issuer' 'Invora.FieldOps.Api' --project Invora.FieldOps.Api
dotnet user-secrets set 'Jwt:Audience' 'Invora.FieldOps.Client' --project Invora.FieldOps.Api
dotnet user-secrets set 'BootstrapAdmin:Username' 'admin' --project Invora.FieldOps.Api
dotnet user-secrets set 'BootstrapAdmin:Password' 'YOUR_ADMIN_PASSWORD_AT_LEAST_12_CHARS' --project Invora.FieldOps.Api
```

3. Migration'ları uygulayıp API'yi başlatın:

```sh
dotnet tool restore
dotnet restore
dotnet build
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project Invora.FieldOps.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Invora.FieldOps.Api --no-launch-profile --urls http://localhost:5198
```

Swagger: http://localhost:5198/swagger

Migration otomatik çalışmaz; şema değişikliklerinde `dotnet ef database update` komutunu tekrar çalıştırın. Mevcut kayıtlar silinmez. Eski ziyaretler atama yapılana kadar yalnızca Admin'e görünür.
Admin hesabı, migration sonrasında ilk açılışta ve yalnızca henüz Admin yoksa oluşturulur. Public register endpoint'i her zaman FieldUser oluşturur. Admin oluştuktan sonra BootstrapAdmin secret'larını kaldırabilirsiniz.

## Swagger ile örnek kullanım

1. `POST /api/auth/login` ile yapılandırdığınız Admin bilgileriyle giriş yapın.
2. Dönen `token` değerini Swagger'daki **Authorize** alanına yapıştırın (başına Bearer yazmayın).
3. `POST /api/companies`: `{"name":"ABC Endüstri A.Ş."}`
4. `POST /api/sites`: `{"name":"Ankara Fabrikası","companyId":1}`
5. `POST /api/auth/register`: `{"username":"saha1","password":"YourPassword123!"}`
6. `GET /api/users` ile FieldUser ID'sini bulun.
7. `POST /api/visits`:

```json
{
  "siteId": 1,
  "assignedUserId": 2,
  "visitDate": "2026-10-20T09:00:00Z",
  "status": "Planned"
}
```

Örnekteki ID'leri endpointlerden dönen gerçek ID'lerle değiştirin. Tarihleri UTC (`Z`) veya saat dilimi bilgisiyle gönderin.

8. FieldUser ile login olup Authorize token'ını değiştirin. Yalnızca kendisine atanmış ziyaretleri ve bunlara bağlı firma, saha, görev ve sorunları görür.
9. `PATCH /api/visits/1/status`: `{"status":"InProgress"}`
10. `POST /api/fieldtasks`: `{"visitId":1,"title":"Ekipman kontrolü","description":"Kontrol listesini tamamla","status":"Pending"}`
11. `POST /api/issues`: `{"visitId":1,"title":"Eksik ekipman","description":"Koruyucu ekipman eksik","status":"Open"}`

## Yetkiler ve endpointler

| Kaynak | Admin | FieldUser |
| --- | --- | --- |
| Companies / Sites | Listele, getir, oluştur, güncelle, sil | Kendi ziyaretlerine bağlı kayıtları oku |
| Visits | Tüm CRUD ve kullanıcı atama | Kendi ziyaretlerini oku ve durum güncelle |
| FieldTasks / Issues | Tüm CRUD | Kendi ziyaretleri altında CRUD |
| Users | FieldUser listesini oku | Erişim yok |
| Auth register / login | Herkese açık | Herkese açık |

CRUD yolları: `GET /api/{resource}`, `GET /api/{resource}/{id}`, `POST /api/{resource}`, `PUT /api/{resource}/{id}`, `DELETE /api/{resource}/{id}`.
Companies ve Sites listelerinde `?search=Ankara` ile ada göre filtreleme yapılabilir.
Atamayı değiştirmek için Admin olarak `PUT /api/visits/{id}` kullanın. FieldUser PATCH ile sadece durumu değiştirebilir; başka kullanıcıya atama yapamaz.

Geçerli durumlar:

- Visit: `Planned`, `InProgress`, `Completed`, `Cancelled`
- FieldTask: `Pending`, `InProgress`, `Completed`
- Issue: `Open`, `InProgress`, `Resolved`

Başarılı oluşturma 201, güncelleme/silme 204, doğrulama hatası 400, geçersiz giriş 401, rol yetersizliği 403, bulunamayan/erişilemeyen kayıt 404, veri çakışması 409 döner.
Beklenmedik hatalar merkezi handler ile ProblemDetails olarak döner; ayrıntılar JSON loglarına yazılır. Response DTO'ları navigation property ve parola hash'i içermez.
Firma silmek altındaki sahaları ve ziyaretleri; ziyaret silmek bağlı görev ve sorunları siler. Silme işlemlerini buna göre kullanın.

## Doğrulama

```sh
dotnet build
dotnet ef migrations has-pending-model-changes --project Invora.FieldOps.Api
```

Uçtan uca test için API'yi **ayrı, boş bir test veritabanında** çalıştırın. Script iki test FieldUser hesabı oluşturur; firma/saha/ziyaret/görev/sorun kayıtlarını test sonunda siler. Kullanıcı hesapları test veritabanında kalır.

```sh
FIELDOPS_TEST_URL=http://localhost:5198 \
FIELDOPS_ADMIN_USERNAME=admin \
FIELDOPS_ADMIN_PASSWORD='YOUR_TEST_ADMIN_PASSWORD' \
python3 tests/smoke.py
```

Testler JWT, Admin/FieldUser rolleri, iki farklı kullanıcının veri izolasyonu, ziyaret atama/değiştirme, CRUD, doğrulama ve Swagger'ı kontrol eder.

## Teslim ve kodu anlama

- `TEKNIK_ANLATIM.md`: Kod akışı, örnek gösterim ve teknik değerlendirme hazırlığı.
- `TESLIM_KONTROLU.md`: Yapılan teknik kontroller ve sonuçları.
- `DOSYA_DEGISIKLIKLERI.md`: Yeni ve değiştirilen dosyaların tam listesi.
- Bu bilgisayardaki geliştirme hesapları: `.local/GIRIS_BILGILERI.txt` (GitHub dışında).

## Git akışı

Önceki çalışmalar ayrı `feature/*` branch'lerinde korunmuştur. JWT başlangıcı `feature/jwt-auth`, tamamlanan çalışma `feature/complete-fieldops` branch'indedir. Teslim PR'ı: https://github.com/Developer-Ekip/Demo/pull/1. Birleştirme durumu PR sayfasında görülebilir.

```sh
git switch feature/complete-fieldops
git pull --ff-only origin feature/complete-fieldops
```

Yeni çalışma için feature branch açın; değişikliği test edip anlamlı commit oluşturun ve push edin. Review için bu branch'ten pull request açılabilir. Önceki branch'ler otomatik birleştirilmez.
