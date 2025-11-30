using Space.Domain.DTO;

namespace Space.Application.Interfaces;

public interface IRecClassService
{
    Task<List<RecClassDto>> GetRecClassesAsync(CancellationToken cancellationToken = default);
}
