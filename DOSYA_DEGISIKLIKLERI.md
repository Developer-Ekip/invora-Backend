# Eklenen ve değiştirilen dosyalar

Asıl proje klasörü: `/Users/muniseaydogdu/Desktop/Invora.FieldOps.Training`. VS Code içinde bu klasörü açmalısın. Önce paylaştığın HotelListing klasörü bu çalışma için değiştirilmedi.

Aşağıdaki liste, senin başlamış olduğun JWT çalışmasını koruyan `ccda1f9` commit'inden sonraki tamamlama değişikliklerini gösterir. AuthController, AuthService, IAuthService, JwtTokenService, User, LoginDto, RegisterDto ve AddUsers migration dosyaları geldiğimde zaten vardı; ilk commit ile bu çalışmanı GitHub'a kaydettim. Bunları sıfırdan benim yazdığım dosyalar olarak saymıyorum.

## Yeni oluşturduğum dosyalar

- `Invora.FieldOps.Api/Controllers/UsersController.cs`
- `Invora.FieldOps.Api/DTO/Responses.cs`
- `Invora.FieldOps.Api/DTO/SaveCompanyDto.cs`
- `Invora.FieldOps.Api/DTO/SaveSiteDto.cs`
- `Invora.FieldOps.Api/DTO/UpdateVisitStatusDto.cs`
- `Invora.FieldOps.Api/Middleware/ApiExceptionHandler.cs`
- `Invora.FieldOps.Api/Migrations/20261016105254_AddVisitAssignmentAndUniqueUsername.Designer.cs`
- `Invora.FieldOps.Api/Migrations/20261016105254_AddVisitAssignmentAndUniqueUsername.cs`
- `Invora.FieldOps.Api/Services/CompanyService.cs`
- `Invora.FieldOps.Api/Services/CurrentUser.cs`
- `Invora.FieldOps.Api/Services/ICompanyService.cs`
- `Invora.FieldOps.Api/Services/ISiteService.cs`
- `Invora.FieldOps.Api/Services/SiteService.cs`
- `README.md`
- `dotnet-tools.json`
- `tests/smoke.py`
- `TEKNIK_ANLATIM.md` — kod akışı ve inceleme hazırlığı
- `TESLIM_KONTROLU.md` — doğrulama sonuçları
- `DOSYA_DEGISIKLIKLERI.md` — bu liste

## Güncellediğim dosyalar

- `Invora.FieldOps.Api/Controllers/AuthController.cs`
- `Invora.FieldOps.Api/Controllers/CompaniesController.cs`
- `Invora.FieldOps.Api/Controllers/FieldTasksController.cs`
- `Invora.FieldOps.Api/Controllers/IssuesController.cs`
- `Invora.FieldOps.Api/Controllers/SitesController.cs`
- `Invora.FieldOps.Api/Controllers/VisitsController.cs`
- `Invora.FieldOps.Api/DTO/CreateFieldTaskDto.cs`
- `Invora.FieldOps.Api/DTO/CreateIssueDto.cs`
- `Invora.FieldOps.Api/DTO/CreateVisitDto.cs`
- `Invora.FieldOps.Api/DTO/LoginDto.cs`
- `Invora.FieldOps.Api/DTO/UpdateFieldTaskDto.cs`
- `Invora.FieldOps.Api/DTO/UpdateIssueDto.cs`
- `Invora.FieldOps.Api/DTO/UpdateVisitDto.cs`
- `Invora.FieldOps.Api/Data/AppDbContext.cs`
- `Invora.FieldOps.Api/Invora.FieldOps.Api.csproj`
- `Invora.FieldOps.Api/Migrations/AppDbContextModelSnapshot.cs`
- `Invora.FieldOps.Api/Models/FieldTask.cs`
- `Invora.FieldOps.Api/Models/Site.cs`
- `Invora.FieldOps.Api/Models/Visit.cs`
- `Invora.FieldOps.Api/Program.cs`
- `Invora.FieldOps.Api/Services/FieldTaskService.cs`
- `Invora.FieldOps.Api/Services/IVisitService.cs`
- `Invora.FieldOps.Api/Services/IssueService.cs`
- `Invora.FieldOps.Api/Services/VisitService.cs`
- `.gitignore` — yerel giriş bilgilerini Git dışında tutar

## Kaldırdığım başlangıç şablonu dosyaları

- `Invora.FieldOps.Api/Controllers/WeatherForecastController.cs`
- `Invora.FieldOps.Api/WeatherForecast.cs`

## Yerel ve GitHub dışında kalan dosya

- `.local/GIRIS_BILGILERI.txt`: Admin ve demo FieldUser giriş bilgileri, örnek kayıt ID'leri. Dosya izinleri yalnızca kullanıcı hesabına açıktır. GitHub'a gönderilmez.

## Önce hangi dosyaları okumalıyım

Önce `README.md`, ardından `TEKNIK_ANLATIM.md`. Kod içinde `Program.cs` → `Controllers/VisitsController.cs` → `Services/VisitService.cs` → `Data/AppDbContext.cs` sırasını izle. Veri erişimi sınırını `Services/CurrentUser.cs`, request alanlarını `DTO/CreateVisitDto.cs`, response alanlarını `DTO/Responses.cs` içinde görebilirsin.
