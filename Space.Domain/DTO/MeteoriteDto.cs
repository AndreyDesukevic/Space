using System.Text.Json.Serialization;

namespace Space.Domain.Models;

public class MeteoriteDto
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? NameType { get; set; }

    public string? RecClass { get; set; }

    public string? Mass { get; set; }

    public string? Fall { get; set; }

    public string? Year { get; set; }

    public string? RecLat { get; set; }

    public string? RecLong { get; set; }

    public GeolocationDto? Geolocation { get; set; }

    [JsonPropertyName(":@computed_region_cbhk_fwbd")]
    public string? RegionDistrictRaw { get; set; }

    [JsonPropertyName(":@computed_region_nnqa_25f4")]
    public string? RegionGeoZoneRaw { get; set; }
}
