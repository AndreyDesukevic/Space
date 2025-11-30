using AutoMapper;
using Space.Domain.DTO;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Mapping;

public class RecClassProfile : Profile
{
    public RecClassProfile()
    {
        CreateMap<RecClass, RecClassDto>();
    }
}
