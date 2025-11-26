using Microsoft.EntityFrameworkCore;
using Space.Infrastructure.Domain.Entities;

namespace Space.Infrastructure.Database;

public class SpaceDbContext : DbContext
{
    public DbSet<Meteorite> Meteorites { get; set; }
    public DbSet<GeoLocation> GeoLocations { get; set; }
    public DbSet<NameType> NameTypes { get; set; }
    public DbSet<RecClass> RecClasses { get; set; }
    public DbSet<GeoType> GeoTypes { get; set; }

    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Meteorite>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(e => e.GeoLocation)
                  .WithOne(g => g.Meteorite)
                  .HasForeignKey<GeoLocation>(g => g.MeteoriteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.NameType)
                  .WithMany(n => n.Meteorites)
                  .HasForeignKey(e => e.NameTypeId);

            entity.HasOne(e => e.RecClass)
                  .WithMany(r => r.Meteorites)
                  .HasForeignKey(e => e.RecClassId);
        });

        modelBuilder.Entity<GeoLocation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Latitude).HasPrecision(18, 6);
            entity.Property(e => e.Longitude).HasPrecision(18, 6);

            entity.HasOne(e => e.GeoType)
                  .WithMany(g => g.GeoLocations)
                  .HasForeignKey(e => e.GeoTypeId);

            entity.HasOne(e => e.Meteorite)
                  .WithOne(m => m.GeoLocation)
                  .HasForeignKey<GeoLocation>(g => g.MeteoriteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NameType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RecClass>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<GeoType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50);
        });
    }
}
