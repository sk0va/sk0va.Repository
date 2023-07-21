using Sample.Core;
using AutoMapper;

namespace Sample.App;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<Person, DbPerson>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PersonName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.PersonAge))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
    }
}