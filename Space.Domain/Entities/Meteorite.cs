using Space.Domain.Entities;

namespace Space.Infrastructure.Domain.Entities;

public class Meteorite : IBaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int? NameTypeId { get; set; }
    public virtual NameType? NameType { get; set; }

    public int? RecClassId { get; set; }
    public virtual RecClass? RecClass { get; set; }

    public double? Mass { get; set; }

    public bool Fall { get; set; }

    public int? Year { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public virtual Geolocation? GeoLocation { get; set; }

    public string? RegionDistrictRaw { get; set; }
    public string? RegionGeoZoneRaw { get; set; }
}
