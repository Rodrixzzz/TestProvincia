using Microsoft.EntityFrameworkCore;
using TestProvincia.Domain.Entities;

namespace TestProvincia.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Desc).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Active).IsRequired();

            entity.HasData(
                new DocumentType { Id = 1, Desc = "DNI", Active = true },
                new DocumentType { Id = 2, Desc = "Pasaporte", Active = true }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DocumentNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(e => e.DocumentType)
                  .WithMany()
                  .HasForeignKey(e => e.DocumentTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.DocumentNumber, e.DocumentTypeId }).IsUnique();
        });
    }
}
