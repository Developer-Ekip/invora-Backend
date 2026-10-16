//EF Core üzerinden veritabanı işlemlerini yönetir
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    //EF Core´a veritabaninda tablo olarak yönetme:
    public DbSet<Company> Companies { get; set; } //companies tablosu

    public DbSet<Site> Sites { get; set; } //site tablosu

    public DbSet<Visit> Visits { get; set; }

    public DbSet<FieldTask> FieldTasks { get; set; }
    public DbSet<Issue> Issues { get; set; }

    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Visit>().HasOne(v => v.AssignedUser)
            .WithMany().HasForeignKey(v => v.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Sites) //bir company -> birden cok site
            .WithOne(s => s.Company) //her site bir comp a bagli
            .HasForeignKey(s => s.CompanyId); //baglanti comp id den kurulur
    }
}
