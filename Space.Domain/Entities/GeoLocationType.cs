namespace Space.Infrastructure.Domain.Entities;

public class GeoType
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<GeoLocation>? GeoLocations { get; set; }
}
