using Space.Domain.Entities;

namespace Space.Infrastructure.Domain.Entities;

public class Geolocation : IBaseEntity
{
    public int Id { get; set; }
    public int? GeolocationTypeId { get; set; }
    public virtual GeolocationType? GeolocationType { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int MeteoriteId { get; set; }
    public virtual Meteorite Meteorite { get; set; } = null!;
}
