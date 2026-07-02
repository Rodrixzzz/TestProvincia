using Microsoft.EntityFrameworkCore;
using TestProvincia.Domain.Entities;

namespace TestProvincia.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DocumentType).IsRequired();
            entity.Property(e => e.DocumentNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });
    }
}
