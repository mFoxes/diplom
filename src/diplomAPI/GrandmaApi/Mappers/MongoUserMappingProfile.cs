using AutoMapper;
using DTO;
using Domain.Models;
using LdapConnector;

namespace GrandmaApi.Mappers;

public class MongoUserMappingProfile : IMapping
{
    public MongoUserMappingProfile(Profile profile)
    {
        profile.CreateMap<MongoUser, UserCardDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommonName));
        
        profile.CreateMap<MongoUser, ContextUserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommonName));

        profile.CreateMap<UserCardDto, MongoUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CommonName, opt => opt.MapFrom(src => src.Name));

        profile.CreateMap<MongoUser, UserTableItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommonName));
        
        profile.CreateMap<User, MongoUser>();

        profile.CreateMap<EmbeddingsResultDto, MongoUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        
        profile.CreateMap<MongoUser, UsernameDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CommonName));
    }
    
}