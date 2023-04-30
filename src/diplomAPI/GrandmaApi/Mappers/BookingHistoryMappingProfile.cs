using AutoMapper;
using DTO;
using Domain.Models;

namespace GrandmaApi.Mappers;

public class BookingHistoryMappingProfile : IMapping
{
    public BookingHistoryMappingProfile(Profile profile)
    {
        profile.CreateMap<BookingHistory, HistoryDto>();
    }
}