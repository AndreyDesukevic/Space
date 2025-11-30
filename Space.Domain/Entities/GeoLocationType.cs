using Space.Domain.Entities;

namespace Space.Infrastructure.Domain.Entities;

public class GeolocationType : IBaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<Geolocation>? GeoLocations { get; set; }
}
