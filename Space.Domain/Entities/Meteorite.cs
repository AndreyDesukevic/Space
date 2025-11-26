namespace Space.Infrastructure.Domain.Entities;

public class Meteorite
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int? NameTypeId { get; set; }
    public virtual NameType? NameType { get; set; }

    public int? RecClassId { get; set; }
    public virtual RecClass? RecClass { get; set; }

    public decimal? Mass { get; set; }

    public bool Fall { get; set; }

    public DateTime? Year { get; set; }

    public virtual GeoLocation? GeoLocation { get; set; }

    public string? RegionDistrictRaw { get; set; }
    public string? RegionGeoZoneRaw { get; set; }
}
