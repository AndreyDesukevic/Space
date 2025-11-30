namespace Space.Domain.Models;

public class GeolocationDto
{
    public string Type { get; set; }

    public double[]? Coordinates { get; set; }
}
