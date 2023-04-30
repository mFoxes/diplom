namespace DTO;

public class OverdueBookingsListDto
{
    public IReadOnlyCollection<BookingInfoDto> Bookings { get;} 
    public OverdueBookingsListDto(IReadOnlyCollection<BookingInfoDto> bookings)
    {
        Bookings = bookings;
    }
}