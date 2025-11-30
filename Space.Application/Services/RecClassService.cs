using AutoMapper;
using Space.Application.Interfaces;
using Space.Domain.DTO;
using Space.Infrastructure.Application;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Services;

public class RecClassService : IRecClassService
{
    private readonly IBaseRepository<RecClass> _recClassRepository;
    private readonly IMapper _mapper;

    public RecClassService(IBaseRepository<RecClass> recClassRepository, IMapper mapper)
    {
        _recClassRepository = recClassRepository;
        _mapper = mapper;
    }

    public async Task<List<RecClassDto>> GetRecClassesAsync(CancellationToken cancellationToken = default)
    {
        var entities = (await _recClassRepository.GetAllAsync(cancellationToken)).ToList();

        return _mapper.Map<List<RecClassDto>>(entities);
    }
}
