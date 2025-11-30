using AutoMapper;
using Space.Domain.Models;
using Space.Infrastructure.Domain.Entities;

namespace Space.Application.Mapping;

public class MeteoriteProfile : Profile
{
    public MeteoriteProfile()
    {
        CreateMap<MeteoriteDto, Meteorite>()
            .ForMember(d => d.GeoLocation, opt => opt.Ignore())

            .ForMember(d => d.Fall,
                opt => opt.MapFrom(s =>
                    s.Fall != null && s.Fall.Equals("Fell", StringComparison.OrdinalIgnoreCase)))
            .ForMember(d => d.Mass,
                opt => opt.MapFrom(s => ParseDouble(s.Mass)))
            .ForMember(d => d.Year,
                opt => opt.MapFrom(s => ParseYear(s.Year)))
            .ForMember(d => d.Latitude,
                opt => opt.MapFrom(s => ParseDouble(s.RecLat)))
            .ForMember(d => d.Longitude,
                opt => opt.MapFrom(s => ParseDouble(s.RecLong)))
            .ForMember(d => d.RegionDistrictRaw,
                opt => opt.MapFrom(s => s.RegionDistrictRaw))
            .ForMember(d => d.RegionGeoZoneRaw,
                opt => opt.MapFrom(s => s.RegionGeoZoneRaw))
            .ForMember(d => d.GeoLocation, opt => opt.Ignore())
            .ForMember(d => d.NameType, opt => opt.Ignore())
            .ForMember(d => d.RecClass, opt => opt.Ignore());
    }

    private static double? ParseDouble(string? value)
    {
        if (double.TryParse(value, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double result))
            return result;

        return null;
    }

    private int? ParseYear(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParse(value, out var dt))
            return dt.Year;

        return null;
    }
}