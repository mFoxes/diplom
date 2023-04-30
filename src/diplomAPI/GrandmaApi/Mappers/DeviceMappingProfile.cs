using AutoMapper;
using DTO;
using Domain.Models;

namespace GrandmaApi.Mappers;

public class DeviceMappingProfile : IMapping
{
    public DeviceMappingProfile(Profile profile)
    {
        profile.CreateMap<DeviceModel, DeviceDto>();
        profile.CreateMap<DeviceDto, DeviceModel>();
    }
}