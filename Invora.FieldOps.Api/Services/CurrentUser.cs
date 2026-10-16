using System.Security.Claims;
namespace Invora.FieldOps.Api.Services;

// JWT içindeki kullanıcı bilgisi; her istek için ayrı oluşturulur.
public class CurrentUser(IHttpContextAccessor accessor)
{
    public bool IsAdmin => accessor.HttpContext!.User.IsInRole("Admin");
    public int Id => int.Parse(accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
