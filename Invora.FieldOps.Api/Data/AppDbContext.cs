using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; } //companies tablosu

    public DbSet<Site> Sites { get; set; } //site tablosu

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Sites) //bir company -> birden cok site
            .WithOne(s => s.Company) //her site bir comp a bagli
            .HasForeignKey(s => s.CompanyId); //baglanti comp id den kurulur
    }
}
