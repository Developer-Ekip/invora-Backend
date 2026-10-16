using Microsoft.OpenApi;
using Invora.FieldOps.Api.Middleware;

using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Invora.FieldOps.Api.Services;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Logging.AddJsonConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Invora FieldOps API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddScoped<IVisitService, VisitService>(); //.netin otom. visitservice nesnesini olusturup ona verir
builder.Services.AddScoped<IFieldTaskService, FieldTaskService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<JwtTokenService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("JWT key must be at least 32 bytes.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql( //UseNpgsql: PostgreSQL kullanacağımızı belirtir
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Learn more about configuring OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler();
app.Use(async (context, next) =>
{
    await next(context);
    app.Logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}",
        context.Request.Method, context.Request.Path, context.Response.StatusCode);
});
app.UseStatusCodePages();

// Admin hesabı yalnızca açıkça yapılandırıldığında oluşturulur.
if (builder.Configuration["BootstrapAdmin:Username"] is string adminName &&
    builder.Configuration["BootstrapAdmin:Password"] is string adminPassword)
{
    if (adminPassword.Length < 12) throw new InvalidOperationException("Bootstrap admin password must be at least 12 characters.");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
    {
        if (await db.Users.AnyAsync(u => u.Username == adminName))
            throw new InvalidOperationException("Bootstrap username is already in use. Choose another username.");
        var admin = new User { Username = adminName, Role = "Admin" };
        admin.PasswordHash = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>().HashPassword(admin, adminPassword);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
