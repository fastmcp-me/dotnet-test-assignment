using AutoMapper;

namespace Application.Common.AutoMapperProfiles;

public class WeatherMappingProfile : Profile
{
    public WeatherMappingProfile()
    {
        //CreateMap<Weather, WeatherDto>();

        //CreateMap<WeatherCreateDto, Weather>();

        //CreateMap<WeatherUpdateDto, Weather>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        //    .ForMember(dest => dest.IsActive, o => o.Ignore())
        //    .ForMember(dest => dest.CreatedAt, o => o.Ignore())
        //    .ForMember(dest => dest.CreatedBy, o => o.Ignore());

    }

}