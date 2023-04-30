using AutoMapper;
using DTO;
using Domain.Models;

namespace GrandmaApi.Mappers;

public class BookingMappingProfile : IMapping
{
    public BookingMappingProfile(Profile profile)
    {
        profile.CreateMap<BookingModel, BookingDto>();
        profile.CreateMap<BookingDto, BookingModel>();
        profile.CreateMap<UpdateBookingDto, BookingModel>();
    }
}