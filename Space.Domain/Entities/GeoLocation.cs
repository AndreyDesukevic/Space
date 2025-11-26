namespace Space.Infrastructure.Domain.Entities;

public class GeoLocation
{
    public int Id { get; set; }

    public int? GeoTypeId { get; set; }
    public virtual GeoType? GeoType { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int MeteoriteId { get; set; }
    public virtual Meteorite Meteorite { get; set; } = null!;
}
