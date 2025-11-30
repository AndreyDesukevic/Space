using Space.Domain.Entities;

namespace Space.Infrastructure.Domain.Entities;

public class NameType : IBaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<Meteorite>? Meteorites { get; set; }
}
