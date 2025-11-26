namespace Space.Domain.Models;

public class GeoLocationDto
{
    public string Type { get; set; }

    public decimal[]? Coordinates { get; set; }
}
