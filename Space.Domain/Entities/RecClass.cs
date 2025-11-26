namespace Space.Infrastructure.Domain.Entities;

public class RecClass
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<Meteorite>? Meteorites { get; set; }
}
