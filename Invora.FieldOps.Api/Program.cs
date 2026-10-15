using Invora.FieldOps.Api.Services;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IVisitService, VisitService>(); //.netin otom. visitservice nesnesini olusturup ona verir

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql( //UseNpgsql: PostgreSQL kullanacağımızı belirtir
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Learn more about configuring OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
