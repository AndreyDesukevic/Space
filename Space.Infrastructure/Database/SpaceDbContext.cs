using Microsoft.EntityFrameworkCore;
using Space.Infrastructure.Domain.Entities;

namespace Space.Infrastructure.Database;

public class SpaceDbContext : DbContext
{
    public DbSet<Meteorite> Meteorites { get; set; }
    public DbSet<Geolocation> Geolocations { get; set; }
    public DbSet<NameType> NameTypes { get; set; }
    public DbSet<RecClass> RecClasses { get; set; }
    public DbSet<GeolocationType> GeolocationTypes { get; set; }

    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Meteorite>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(e => e.GeoLocation)
                  .WithOne(g => g.Meteorite)
                  .HasForeignKey<Geolocation>(g => g.MeteoriteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.NameType)
                  .WithMany(n => n.Meteorites)
                  .HasForeignKey(e => e.NameTypeId);

            entity.HasOne(e => e.RecClass)
                  .WithMany(r => r.Meteorites)
                  .HasForeignKey(e => e.RecClassId);

            entity.HasIndex(e => e.Year);

            entity.HasIndex(e => e.RecClassId);

            entity.HasIndex(e => new { e.Year, e.RecClassId });
        });

        modelBuilder.Entity<Geolocation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Latitude).HasPrecision(18, 6);
            entity.Property(e => e.Longitude).HasPrecision(18, 6);

            entity.HasOne(e => e.GeolocationType)
                  .WithMany(g => g.GeoLocations)
                  .HasForeignKey(e => e.GeolocationTypeId);

            entity.HasOne(e => e.Meteorite)
                  .WithOne(m => m.GeoLocation)
                  .HasForeignKey<Geolocation>(g => g.MeteoriteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NameType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<RecClass>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<GeolocationType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}
